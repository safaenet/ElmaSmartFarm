using Dapper;
using ElmaSmartFarm.DataLibraryCore.Interfaces;
using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.Config;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.SqlServer
{
    public class MsSqlDbProcessor : IDbProcessor
    {
        public MsSqlDbProcessor(IDataAccess dataAccess, Config cfg)
        {
            DataAccess = dataAccess;
            config = cfg;
        }

        private readonly IDataAccess DataAccess;
        private readonly Config config;
        private readonly string SaveTempSensorData = @"
            DECLARE @isEnabled bit; SET @isEnabled = (SELECT [IsEnabled] FROM [TemperatureSensors] WHERE [Id] = @sensorId);
            IF ISNULL(@isEnabled, 0) = 1 BEGIN
                DECLARE @offset decimal(5, 2); SET @offset = (SELECT [OffsetValue] FROM [TemperatureSensors] WHERE [Id] = @sensorId);
                DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [TemperatureValues]) + 1;
                SET @sensorValue = @sensorValue + @offset;
                INSERT INTO [TemperatureValues] ([Id], [SensorId], [ReadDate], [SensorValue]) VALUES (@newId, @sensorId, @readDate, @sensorValue);
            END";
        private readonly string LoadPoultriesQuery = @$"SELECT l.* FROM dbo.Locations l WHERE l.Type = {(int)LocationType.Poultry};";
        private readonly string LoadFarmsQuery = $@"SELECT f.*, l.[Name], l.IsEnabled, l.Descriptions FROM Farms f LEFT JOIN dbo.Locations l ON f.Id = l.Id WHERE f.PoultryId IN (SELECT p.Id FROM Locations p WHERE p.[Type] = {(int)LocationType.Poultry});";
        private const string LoadScalarSensorsQuery = @"SELECT s.*, sd.[Name], sd.LocationId, sd.Section, hsd.OffsetValue FROM dbo.SensorDetails sd LEFT JOIN dbo.Sensors s ON sd.SensorId = s.Id LEFT JOIN dbo.{0} hsd ON s.Id = hsd.SensorId WHERE s.[Type] = {1};";
        private const string LoadNonScalarSensorsQuery = @"SELECT s.*, sd.[Name], sd.LocationId, sd.Section FROM dbo.SensorDetails sd LEFT JOIN dbo.Sensors s ON sd.SensorId = s.Id WHERE s.[Type] = {0};";
        private readonly string LoadFarmTempSensorQuery = string.Format(LoadScalarSensorsQuery, "TemperatureSensorDetails", (int)SensorType.FarmTemperature);
        private readonly string LoadFarmHumidSensorQuery = string.Format(LoadScalarSensorsQuery, "HumiditySensorDetails", (int)SensorType.FarmHumidity);
        private readonly string LoadFarmAmbientSensorQuery = string.Format(LoadScalarSensorsQuery, "AmbientLightSensorDetails", (int)SensorType.FarmAmbientLight);
        private readonly string LoadOutdoorTempSensorQuery = string.Format(LoadScalarSensorsQuery, "TemperatureSensorDetails", (int)SensorType.OutdoorTemperature);
        private readonly string LoadOutdoorHumidSensorQuery = string.Format(LoadScalarSensorsQuery, "HumiditySensorDetails", (int)SensorType.OutdoorHumidity);

        private readonly string LoadFarmFeedSensorQuery = string.Format(LoadNonScalarSensorsQuery, (int)SensorType.FarmFeed);
        private readonly string LoadFarmCheckupSensorQuery = string.Format(LoadNonScalarSensorsQuery, (int)SensorType.FarmCheckup);
        private readonly string LoadFarmCommuteSensorQuery = string.Format(LoadNonScalarSensorsQuery, (int)SensorType.FarmCommute);
        private readonly string LoadFarmPowerSensorQuery = string.Format(LoadNonScalarSensorsQuery, (int)SensorType.FarmElectricPower);
        private readonly string LoadPoultryMPowerSensorQuery = string.Format(LoadNonScalarSensorsQuery, (int)SensorType.PoultryMainElectricPower);
        private readonly string LoadPoultryBPowerSensorQuery = string.Format(LoadNonScalarSensorsQuery, (int)SensorType.PoultryBackupElectricPower);

        private const string LoadActivePeriodsQuery = @"SELECT * FROM dbo.[Periods] p WHERE p.EndDate = NULL;
            SELECT * FROM dbo.ChickenLosses cl WHERE cl.PeriodId IN (SELECT p.Id FROM dbo.[Periods] p WHERE p.EndDate = NULL);
            SELECT * FROM dbo.Feeds f WHERE f.PeriodId IN (SELECT p.Id FROM dbo.[Periods] p WHERE p.EndDate = NULL);";

        private const string LoadSensorErrors = "SELECT * FROM dbo.SensorErrorLogs WHERE DateErased = NULL";
        private const string LoadFarmInPeriodErrors = "SELECT * FROM FarmInPeriodErrorLogs WHERE DateErased = NULL";
        private const string LoadPoultryInPeriodErrors = "SELECT * FROM PoultryInPeriodErrorLogs WHERE DateErased = NULL";

        private readonly string WriteScalarSensorValueToDbCmd = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [{0}]) + 1;
            INSERT INTO {0}(Id, LocationId, Section, SensorId, ReadDate, SensorValue)
            VALUES(@newId, @locationId, @section, @sensorId, @readDate, @sensorValue);
            SELECT @id = @newId;";

        public async Task<int> SaveSensorValueToDbAsync(TemperatureSensorModel sensor, double value)
        {
            DynamicParameters dp = new();
            dp.Add("@id", 0, System.Data.DbType.Int32,System.Data.ParameterDirection.Output);
            dp.Add("@locationId", sensor.LocationId);
            dp.Add("@section", sensor.Section);
            dp.Add("@sensorId", sensor.Id);
            dp.Add("@readDate", DateTime.Now);
            dp.Add("@sensorValue", value + sensor.Offset);
            var sql = string.Format(WriteScalarSensorValueToDbCmd, "TemperatureValues");
            _ = await DataAccess.SaveDataAsync(sql, dp);
            var newId = dp.Get<int>("@id");
            return newId;
        }

        public async Task<List<PoultryModel>> LoadPoultries()
        {
            var poultries = (await DataAccess.LoadDataAsync<PoultryModel, DynamicParameters>(LoadPoultriesQuery, null)).ToList();
            if (poultries != null && poultries.Any())
            {
                var farms = await DataAccess.LoadDataAsync<FarmModel, DynamicParameters>(LoadFarmsQuery, null);
                var outdoorTempSensors = await DataAccess.LoadDataAsync<TemperatureSensorModel, DynamicParameters>(LoadOutdoorTempSensorQuery, null);
                var outdoorHumidSensors = await DataAccess.LoadDataAsync<HumiditySensorModel, DynamicParameters>(LoadOutdoorHumidSensorQuery, null);
                var poultryInPeriodErrors = await DataAccess.LoadDataAsync<PoultryInPeriodErrorModel, DynamicParameters>(LoadPoultryInPeriodErrors, null);
                var periods = await LoadActivePeriods();
                if (farms != null && farms.Any())
                {
                    var farmTempSensors = await DataAccess.LoadDataAsync<TemperatureSensorModel, DynamicParameters>(LoadFarmTempSensorQuery, null);
                    var farmHumidSensors = await DataAccess.LoadDataAsync<HumiditySensorModel, DynamicParameters>(LoadFarmHumidSensorQuery, null);
                    var farmAmbientSensors = await DataAccess.LoadDataAsync<AmbientLightSensorModel, DynamicParameters>(LoadFarmAmbientSensorQuery, null);
                    var farmFeedSensors = await DataAccess.LoadDataAsync<PushButtonSensorModel, DynamicParameters>(LoadFarmFeedSensorQuery, null);
                    var farmCheckupSensors = await DataAccess.LoadDataAsync<PushButtonSensorModel, DynamicParameters>(LoadFarmCheckupSensorQuery, null);
                    var farmCommuteSensors = await DataAccess.LoadDataAsync<CommuteSensorModel, DynamicParameters>(LoadFarmCommuteSensorQuery, null);
                    var farmPowerSensors = await DataAccess.LoadDataAsync<BinarySensorModel, DynamicParameters>(LoadFarmPowerSensorQuery, null);
                    var farmPoultryMPowerSensors = await DataAccess.LoadDataAsync<BinarySensorModel, DynamicParameters>(LoadPoultryMPowerSensorQuery, null);
                    var farmPoultryBPowerSensors = await DataAccess.LoadDataAsync<BinarySensorModel, DynamicParameters>(LoadPoultryBPowerSensorQuery, null);
                    var sensorErrors = await DataAccess.LoadDataAsync<SensorErrorModel, DynamicParameters>(LoadSensorErrors, null);
                    var farmInPeriodErrors = await DataAccess.LoadDataAsync<FarmInPeriodErrorModel, DynamicParameters>(LoadFarmInPeriodErrors, null);
                    if (sensorErrors != null && sensorErrors.Any())
                    {
                        if (farmTempSensors != null) foreach (var s in farmTempSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                        if (farmHumidSensors != null) foreach (var s in farmHumidSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                        if (farmAmbientSensors != null) foreach (var s in farmAmbientSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                        if (farmFeedSensors != null) foreach (var s in farmFeedSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                        if (farmCheckupSensors != null) foreach (var s in farmCheckupSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                        if (farmCommuteSensors != null) foreach (var s in farmCommuteSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                        if (farmPowerSensors != null) foreach (var s in farmPowerSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                        if (farmPoultryMPowerSensors != null) foreach (var s in farmPoultryMPowerSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                        if (farmPoultryBPowerSensors != null) foreach (var s in farmPoultryBPowerSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                    }
                    foreach (var f in farms)
                    {
                        f.Temperatures.Sensors = farmTempSensors?.Where(s => s.LocationId == f.Id).ToList();
                        f.Humidities.Sensors = farmHumidSensors?.Where(s => s.LocationId == f.Id).ToList();
                        f.AmbientLights.Sensors = farmAmbientSensors?.Where(s => s.LocationId == f.Id).ToList();
                        f.Feeds.Sensors = farmFeedSensors?.Where(s => s.LocationId == f.Id).ToList();
                        f.Checkups.Sensors = farmCheckupSensors?.Where(s => s.LocationId == f.Id).ToList();
                        f.Commutes.Sensors = farmCommuteSensors?.Where(s => s.LocationId == f.Id).ToList();
                        f.ElectricPowers.Sensors = farmPowerSensors?.Where(s => s.LocationId == f.Id).ToList();
                        f.Period = periods?.Where(p => p.FarmId == f.Id)?.FirstOrDefault();
                        if (periods != null && periods.Any()) f.InPeriodErrors = farmInPeriodErrors?.Where(e => e.FarmId == f.Id).ToList();
                    }
                }
                foreach (var p in poultries)
                {
                    p.Farms = farms?.Where(f => f.PoultryId == p.Id).ToList();
                    p.OutdoorTemperature = outdoorTempSensors?.Where(s => s.LocationId == p.Id)?.FirstOrDefault();
                    p.OutdoorHumidity = outdoorHumidSensors?.Where(s => s.LocationId == p.Id)?.FirstOrDefault();
                    if (periods != null && periods.Any()) p.InPeriodErrors = poultryInPeriodErrors?.Where(e => e.PoultryId == p.Id).ToList();
                }
            }
            Log.Information("Poultries loaded.");
            return poultries;
        }

        private async Task<IEnumerable<PeriodModel>> LoadActivePeriods()
        {
            using System.Data.IDbConnection conn = new SqlConnection(config.DefaultConnectionString);
            var reader = await conn.QueryMultipleAsync(LoadActivePeriodsQuery, null);
            var periods = await reader.ReadAsync<PeriodModel>();
            if (periods == null) return null;
            var chickelLosses = (await reader.ReadAsync<ChickenLossModel>()).GroupBy(cl => cl.PeriodId).ToDictionary(g => g.Key, g => g.AsEnumerable());
            var feeds = (await reader.ReadAsync<FeedModel>()).GroupBy(cl => cl.PeriodId).ToDictionary(g => g.Key, g => g.AsEnumerable());
            foreach (var p in periods)
            {
                if (chickelLosses.TryGetValue(p.Id, out IEnumerable<ChickenLossModel> cls)) p.ChickenStatistics.ChickenLosses = cls.ToList();
                if (feeds.TryGetValue(p.Id, out IEnumerable<FeedModel> fs)) p.FoodStatistics.Feeds = fs.ToList();
            }
            return periods;
        }
    }
}