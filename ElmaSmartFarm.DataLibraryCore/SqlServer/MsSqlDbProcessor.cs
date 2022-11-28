using Dapper;
using ElmaSmartFarm.DataLibraryCore.Interfaces;
using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.SqlServer
{
    public class MsSqlDbProcessor : IDbProcessor
    {
        public MsSqlDbProcessor(IDataAccess dataAccess)
        {
            DataAccess = dataAccess;
        }

        private readonly IDataAccess DataAccess;
        private readonly string SaveTempSensorData = @"
            DECLARE @isEnabled bit; SET @isEnabled = (SELECT [IsEnabled] FROM [TemperatureSensors] WHERE [Id] = @sensorId);
            IF ISNULL(@isEnabled, 0) = 1 BEGIN
                DECLARE @offset decimal(5, 2); SET @offset = (SELECT [OffsetValue] FROM [TemperatureSensors] WHERE [Id] = @sensorId);
                DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [TemperatureValues]) + 1;
                SET @sensorValue = @sensorValue + @offset;
                INSERT INTO [TemperatureValues] ([Id], [SensorId], [ReadDate], [SensorValue]) VALUES (@newId, @sensorId, @readDate, @sensorValue);
            END";
        private readonly string LoadPoultriesQuery = @$"SELECT l.* FROM dbo.Locations l WHERE l.Type = {(int)LocationType.Poultry}";
        private readonly string LoadFarmsQuery = $@"SELECT f.*, l.[Name], l.IsEnabled, l.Descriptions FROM dbo.Farms f LEFT JOIN dbo.Locations l ON f.PoultryId = l.Id WHERE l.[Type] = {(int)LocationType.Farm}";
        private const string LoadScalarSensorsQuery = @"SELECT s.*, sd.[Name], sd.LocationId, sd.Section, hsd.OffsetValue FROM dbo.SensorDetails sd LEFT JOIN dbo.Sensors s ON sd.SensorId = s.Id LEFT JOIN dbo.{0} hsd ON s.Id = hsd.SensorId WHERE s.[Type] = {1}";
        private const string LoadNonScalarSensorsQuery = @"SELECT s.*, sd.[Name], sd.LocationId, sd.Section FROM dbo.SensorDetails sd LEFT JOIN dbo.Sensors s ON sd.SensorId = s.Id WHERE s.[Type] = {0}";
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

        private const string LoadActivePeriodsQuery = @"SELECT * FROM dbo.[Periods] p WHERE p.EndDate != NULL;
            SELECT * FROM dbo.ChickenLosses cl WHERE cl.PeriodId IN (SELECT p.Id FROM dbo.[Periods] p WHERE p.EndDate != NULL);
            SELECT * FROM dbo.Feeds f WHERE f.PeriodId IN (SELECT p.Id FROM dbo.[Periods] p WHERE p.EndDate != NULL);";

        public async Task<int> SaveTemperatureToDb(TemperatureModel temp)
        {
            DynamicParameters dp = new();
            dp.Add("@sensorId", temp.SensorId);
            dp.Add("@readDate", temp.ReadDate);
            dp.Add("@sensorValue", temp.Celsius);
            return await DataAccess.SaveDataAsync(SaveTempSensorData, dp);
        }

        public async Task<IEnumerable<PoultryModel>> LoadPoultries()
        {
            IEnumerable<PoultryModel> poultries = await DataAccess.LoadDataAsync<PoultryModel, DynamicParameters>(LoadPoultriesQuery, null);
            if (poultries != null && poultries.Any())
            {
                IEnumerable<FarmModel> farms = await DataAccess.LoadDataAsync<FarmModel, DynamicParameters>(LoadFarmsQuery, null);
                IEnumerable<TemperatureSensorModel> outdoorTempSensors = await DataAccess.LoadDataAsync<TemperatureSensorModel, DynamicParameters>(LoadOutdoorTempSensorQuery, null);
                IEnumerable<HumiditySensorModel> outdoorHumidSensors = await DataAccess.LoadDataAsync<HumiditySensorModel, DynamicParameters>(LoadOutdoorHumidSensorQuery, null);
                if (farms != null && farms.Any())
                {
                    IEnumerable<TemperatureSensorModel> farmTempSensors = await DataAccess.LoadDataAsync<TemperatureSensorModel, DynamicParameters>(LoadFarmTempSensorQuery, null);
                    IEnumerable<HumiditySensorModel> farmHumidSensors = await DataAccess.LoadDataAsync<HumiditySensorModel, DynamicParameters>(LoadFarmHumidSensorQuery, null);
                    IEnumerable<AmbientLightSensorModel> farmAmbientSensors = await DataAccess.LoadDataAsync<AmbientLightSensorModel, DynamicParameters>(LoadFarmAmbientSensorQuery, null);
                    IEnumerable<PushButtonSensorModel> farmFeedSensors = await DataAccess.LoadDataAsync<PushButtonSensorModel, DynamicParameters>(LoadFarmFeedSensorQuery, null);
                    IEnumerable<PushButtonSensorModel> farmCheckupSensors = await DataAccess.LoadDataAsync<PushButtonSensorModel, DynamicParameters>(LoadFarmCheckupSensorQuery, null);
                    IEnumerable<CommuteSensorModel> farmCommuteSensors = await DataAccess.LoadDataAsync<CommuteSensorModel, DynamicParameters>(LoadFarmCommuteSensorQuery, null);
                    IEnumerable<BinarySensorModel> farmPowerSensors = await DataAccess.LoadDataAsync<BinarySensorModel, DynamicParameters>(LoadFarmPowerSensorQuery, null);
                    IEnumerable<BinarySensorModel> farmPoultryMPowerSensors = await DataAccess.LoadDataAsync<BinarySensorModel, DynamicParameters>(LoadPoultryMPowerSensorQuery, null);
                    IEnumerable<BinarySensorModel> farmPoultryBPowerSensors = await DataAccess.LoadDataAsync<BinarySensorModel, DynamicParameters>(LoadPoultryBPowerSensorQuery, null);
                    IEnumerable<PeriodModel> periods = await LoadActivePeriods();
                    foreach (var f in farms)
                    {
                        f.Temperatures.Sensors = farmTempSensors?.Where(s => s.LocationId == f.Id);
                        f.Humidities.Sensors = farmHumidSensors?.Where(s => s.LocationId == f.Id);
                        f.AmbientLights.Sensors = farmAmbientSensors?.Where(s => s.LocationId == f.Id);
                        f.Feeds.Sensors = farmFeedSensors?.Where(s => s.LocationId == f.Id);
                        f.Checkups.Sensors = farmCheckupSensors?.Where(s => s.LocationId == f.Id);
                        f.Commutes.Sensors = farmCommuteSensors?.Where(s => s.LocationId == f.Id);
                        f.ElectricPowers.Sensors = farmPowerSensors?.Where(s => s.LocationId == f.Id);
                        f.Period = periods?.Where(p => p.FarmId == f.Id)?.FirstOrDefault();
                    }
                }
                foreach (var p in poultries)
                {
                    p.Farms = farms?.Where(f => f.PoultryId == p.Id);
                    p.OutdoorTemperature = outdoorTempSensors?.Where(s => s.LocationId == p.Id)?.FirstOrDefault();
                    p.OutdoorHumidity = outdoorHumidSensors?.Where(s => s.LocationId == p.Id)?.FirstOrDefault();
                }
            }
            return poultries;
        }

        private async Task<IEnumerable<PeriodModel>> LoadActivePeriods()
        {
            var reader = await DataAccess.LoadMultipleDataAsync<DynamicParameters>(LoadActivePeriodsQuery, null);
            var periods = await reader.ReadAsync<PeriodModel>();
            if (periods == null) return null;
            var chickelLosses = (await reader.ReadAsync<ChickenLossModel>()).GroupBy(cl => cl.PeriodId).ToDictionary(g => g.Key, g => g.AsEnumerable());
            var feeds = (await reader.ReadAsync<FeedModel>()).GroupBy(cl => cl.PeriodId).ToDictionary(g => g.Key, g => g.AsEnumerable());
            foreach (var p in periods)
            {
                if (chickelLosses.TryGetValue(p.Id, out IEnumerable<ChickenLossModel> cls)) p.ChickenStatistics.ChickenLosses = cls;
                if (feeds.TryGetValue(p.Id, out IEnumerable<FeedModel> fs)) p.FoodStatistics.Feeds = fs;
            }
            return periods;
        }
    }
}