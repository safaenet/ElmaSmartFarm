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

        private const string LoadSensorErrors = "SELECT * FROM dbo.SensorErrorLogs WHERE DateErased = NULL;";
        private const string LoadFarmInPeriodErrors = "SELECT * FROM FarmInPeriodErrorLogs WHERE DateErased = NULL;";
        private const string LoadPoultryInPeriodErrors = "SELECT * FROM PoultryInPeriodErrorLogs WHERE DateErased = NULL;";

        private readonly string WriteScalarSensorValueCmd = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [{0}]) + 1;
            INSERT INTO {0}(Id, LocationId, Section, SensorId, ReadDate{1})
            VALUES(@newId, @LocationId, @Section, @SensorId, @ReadDate{2});
            SELECT @Id = @newId;";
        private readonly string WriteSensorErrorCmd = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [SensorErrorLogs]) + 1;
            INSERT INTO SensorErrorLogs (Id, SensorId, LocationId, Section, ErrorType, DateHappened, Descriptions)
            VALUES (@newId, @SensorId, @LocationId, @Section, @ErrorType, @DateHappened, @Descriptions); SELECT @Id = @newId";
        private readonly string EraseSensorErrorCmd = @"UPDATE SensorErrorLogs SET DateErased = @DateErased WHERE DateErased IS NULL AND SensorId = @SensorId AND LocationId = @LocationId AND Section = @Section AND ErrorType = @ErrorType;";
        private readonly string EraseSensorErrorCmd2 = @"UPDATE SensorErrorLogs SET DateErased = @DateErased WHERE DateErased IS NULL AND SensorId = @SensorId AND ErrorType = @ErrorType;";

        public async Task<int> WriteSensorValueToDbAsync(SensorModel sensor, double value, DateTime now, double offset = 0)
        {
            try
            {
                if (config.VerboseMode) Log.Information($"Writing sensor value in database. Sensor ID: {sensor.Id}, LocationID: {sensor.LocationId}, Section: {sensor.Section}, Value: {value}");
                DynamicParameters dp = new();
                dp.Add("@Id", 0, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
                dp.Add("@LocationId", sensor.LocationId);
                dp.Add("@Section", sensor.Section);
                dp.Add("@SensorId", sensor.Id);
                dp.Add("@ReadDate", now);
                if (!sensor.Type.IsPushButtonSensor())
                    dp.Add("@SensorValue", value + offset);
                string sql = string.Empty;
                if (sensor.Type.IsTemperatureSensor()) sql = string.Format(WriteScalarSensorValueCmd, "TemperatureValues", ", SensorValue", ", @SensorValue");
                else if (sensor.Type.IsHumiditySensor()) sql = string.Format(WriteScalarSensorValueCmd, "HumidityValues", ", SensorValue", ", @SensorValue");
                else if (sensor.Type == SensorType.FarmAmbientLight) sql = string.Format(WriteScalarSensorValueCmd, "AmbientLightValues", ", SensorValue", ", @SensorValue");
                else if (sensor.Type == SensorType.FarmCommute) sql = string.Format(WriteScalarSensorValueCmd, "CommuteValues", ", SensorValue", ", @SensorValue");
                else if (sensor.Type.IsPushButtonSensor()) sql = string.Format(WriteScalarSensorValueCmd, "PushButtonSensorValues", "", "");
                else if (sensor.Type.IsBinarySensor()) sql = string.Format(WriteScalarSensorValueCmd, "BinarySensorValues", ", SensorValue", ", @SensorValue");
                _ = await DataAccess.SaveDataAsync(sql, dp);
                var newId = dp.Get<int>("@Id");
                return newId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when writing sensor error in database. Sensor ID: {sensor.Id}, LocationID: {sensor.LocationId}, Section: {sensor.Section}, Value: {value}");
            }
            return 0;
        }

        public async Task<int> WriteSensorErrorToDbAsync(SensorErrorModel error, DateTime now)
        {
            try
            {
                if (config.VerboseMode) Log.Information($"Writing sensor error in database. Sensor ID: {error.SensorId}, LocationID: {error.LocationId}, Section: {error.Section}, Error Type: {error.ErrorType}");
                DynamicParameters dp = new();
                dp.Add("@Id", 0, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
                dp.Add("@SensorId", error.SensorId);
                dp.Add("@LocationId", error.LocationId);
                dp.Add("@Section", error.Section);
                dp.Add("@ErrorType", error.ErrorType);
                dp.Add("@DateHappened", now);
                dp.Add("@Descriptions", error.Descriptions);
                _ = await DataAccess.SaveDataAsync(WriteSensorErrorCmd, dp);
                var newId = dp.Get<int>("@Id");
                if (newId == 0) Log.Error($"Error when writing sensor error. Sensor ID: {error.SensorId}, Location: {error.LocationId}, Section: {error.Section}, Error Type: {error.ErrorType}. (System Error)");
                return newId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when writing sensor error in database. Sensor ID: {error.SensorId}, LocationID: {error.LocationId}, Section: {error.Section}, Error Type: {error.ErrorType}");
            }
            return 0;
        }

        public async Task<bool> EraseSensorErrorFromDbAsync(int sensorId, SensorErrorType type, DateTime eraseDate)
        {
            try
            {
                if (config.VerboseMode) Log.Information($"Updating sensor(s) error in database, if existed. Sensor ID: {sensorId}, Error Type: {type}");
                DynamicParameters dp = new();
                dp.Add("@SensorId", sensorId);
                dp.Add("@ErrorType", type);
                dp.Add("@DateErased", eraseDate);
                var i = await DataAccess.SaveDataAsync(EraseSensorErrorCmd2, dp);
                //if (i == 0) Log.Error($"Error when erasing sensor error. Sensor Type: {sensor.Type}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}, ErrorType: {type}. (System Error)");
                return i > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when updating sensor error in database. Sensor ID: {sensorId}, Error Type: {type}");
            }
            return false;
        }

        public async Task<bool> EraseSensorErrorFromDbAsync(SensorBaseModel sensor, SensorErrorType type, DateTime eraseDate)
        {
            try
            {
                if (config.VerboseMode) Log.Information($"Updating sensor error in database. Sensor ID: {sensor.Id}, LocationID: {sensor.LocationId}, Section: {sensor.Section}, Error Type: {type}");
                DynamicParameters dp = new();
                dp.Add("@SensorId", sensor.Id);
                dp.Add("@LocationId", sensor.LocationId);
                dp.Add("@Section", sensor.Section);
                dp.Add("@ErrorType", type);
                dp.Add("@DateErased", eraseDate);
                var i = await DataAccess.SaveDataAsync(EraseSensorErrorCmd, dp);
                //if (i == 0) Log.Error($"Error when erasing sensor error. Sensor Type: {sensor.Type}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}, ErrorType: {type}. (System Error)");
                return i > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when updating sensor error in database. Sensor ID: {sensor.Id}, LocationID: {sensor.LocationId}, Section: {sensor.Section}, Error Type: {type}");
            }
            return false;
        }

        public async Task<List<PoultryModel>> LoadPoultriesAsync()
        {
            try
            {
                if (config.VerboseMode) Log.Information($"Loading poultries...");
                var poultries = (await DataAccess.LoadDataAsync<PoultryModel, DynamicParameters>(LoadPoultriesQuery, null)).ToList();
                if (poultries != null && poultries.Any())
                {
                    var farms = await DataAccess.LoadDataAsync<FarmModel, DynamicParameters>(LoadFarmsQuery, null);
                    var outdoorTempSensors = await DataAccess.LoadDataAsync<TemperatureSensorModel, DynamicParameters>(LoadOutdoorTempSensorQuery, null);
                    var outdoorHumidSensors = await DataAccess.LoadDataAsync<HumiditySensorModel, DynamicParameters>(LoadOutdoorHumidSensorQuery, null);
                    var poultryInPeriodErrors = await DataAccess.LoadDataAsync<PoultryInPeriodErrorModel, DynamicParameters>(LoadPoultryInPeriodErrors, null);
                    var periods = await LoadActivePeriodsAsync();
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
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when loading poultries.");
            }
            return null;
        }

        private async Task<IEnumerable<PeriodModel>> LoadActivePeriodsAsync()
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