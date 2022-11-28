using Dapper;
using ElmaSmartFarm.DataLibraryCore.Interfaces;
using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System.Collections.Generic;
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
        private readonly string LoadPoultriesQuery = @$"SELECT l.[Id], l.[Name], l.[Descriptions], l.[IsEnabled] FROM dbo.Locations l WHERE l.Type = {(int)LocationType.Poultry}";
        private readonly string LoadFarmsQuery = $@"SELECT f.Id, f.PoultryId, l.[Name], l.IsEnabled, l.Descriptions, f.FarmNumber, f.MaxCapacity
            FROM dbo.Farms f LEFT JOIN dbo.Locations l ON f.PoultryId = l.Id WHERE l.[Type] = {(int)LocationType.Farm}";
        private const string LoadScalarSensorsQuery = @"SELECT s.Id, s.IsEnabled, s.Descriptions, sd.[Name], sd.LocationId, sd.Section, hsd.OffsetValue FROM dbo.SensorDetails sd LEFT JOIN dbo.Sensors s ON sd.SensorId = s.Id LEFT JOIN dbo.{0} hsd ON s.Id = hsd.SensorId WHERE s.[Type] = {1}";
        private const string LoadNonScalarSensorsQuery = @"SELECT s.Id, s.IsEnabled, s.Descriptions, sd.[Name], sd.LocationId, sd.Section FROM dbo.SensorDetails sd LEFT JOIN dbo.Sensors s ON sd.SensorId = s.Id WHERE s.[Type] = {0}";
        private readonly string LoadFarmTempSensorQuery = string.Format(LoadScalarSensorsQuery, "TemperatureSensorDetails", (int)SensorType.FarmTemperature);
        private readonly string LoadFarmHumidSensorQuery = string.Format(LoadScalarSensorsQuery, "HumiditySensorDetails", (int)SensorType.FarmHumidity);
        private readonly string LoadFarmAmbientSensorQuery = string.Format(LoadScalarSensorsQuery, "AmbientLightSensorDetails", (int)SensorType.FarmAmbientLight);
        private readonly string LoadOutdoorTempSensorQuery = string.Format(LoadScalarSensorsQuery, "TemperatureSensorDetails", (int)SensorType.OutdoorTemperature);
        private readonly string LoadOutdoorHumidSensorQuery = string.Format(LoadScalarSensorsQuery, "HumiditySensorDetails", (int)SensorType.OutdoorHumidity);

        private readonly string LoadFarmFeedSensorQuery = string.Format(LoadScalarSensorsQuery, (int)SensorType.FarmFeed);
        private readonly string LoadFarmCheckupSensorQuery = string.Format(LoadScalarSensorsQuery, (int)SensorType.FarmCheckup);
        private readonly string LoadFarmCommuteSensorQuery = string.Format(LoadScalarSensorsQuery, (int)SensorType.FarmCommute);
        private readonly string LoadFarmPowerSensorQuery = string.Format(LoadScalarSensorsQuery, (int)SensorType.FarmElectricPower);
        private readonly string LoadPoultryMPowerSensorQuery = string.Format(LoadScalarSensorsQuery, (int)SensorType.PoultryMainElectricPower);
        private readonly string LoadPoultryBPowerSensorQuery = string.Format(LoadScalarSensorsQuery, (int)SensorType.PoultryBackupElectricPower);

        public async Task<int> SaveTemperatureToDb(TemperatureModel temp)
        {
            DynamicParameters dp = new();
            dp.Add("@sensorId", temp.SensorId);
            dp.Add("@readDate", temp.ReadDate);
            dp.Add("@sensorValue", temp.Celsius);
            return await DataAccess.SaveDataAsync(SaveTempSensorData, dp);
        }

        public async Task<List<PoultryModel>> LoadPoultries()
        {
            List<PoultryModel> poultries = (await DataAccess.LoadDataAsync<PoultryModel, DynamicParameters>(LoadPoultriesQuery, null)).AsList();
            List<FarmModel> farms = (await DataAccess.LoadDataAsync<FarmModel, DynamicParameters>(LoadFarmsQuery, null)).AsList();
            List<TemperatureSensorModel> farmTempSensors = (await DataAccess.LoadDataAsync<TemperatureSensorModel, DynamicParameters>(LoadFarmTempSensorQuery, null)).AsList();
            List<HumiditySensorModel> farmHumidSensors = (await DataAccess.LoadDataAsync<HumiditySensorModel, DynamicParameters>(LoadFarmHumidSensorQuery, null)).AsList();
            List<AmbientLightSensorModel> farmAmbientSensors = (await DataAccess.LoadDataAsync<AmbientLightSensorModel, DynamicParameters>(LoadFarmAmbientSensorQuery, null)).AsList();
            List<TemperatureSensorModel> outdoorTempSensors = (await DataAccess.LoadDataAsync<TemperatureSensorModel, DynamicParameters>(LoadOutdoorTempSensorQuery, null)).AsList();
            List<HumiditySensorModel> outdoorHumidSensors = (await DataAccess.LoadDataAsync<HumiditySensorModel, DynamicParameters>(LoadOutdoorHumidSensorQuery, null)).AsList();

            List<PushButtonSensorModel> farmFeedSensors = (await DataAccess.LoadDataAsync<PushButtonSensorModel, DynamicParameters>(LoadFarmFeedSensorQuery, null)).AsList();
            List<PushButtonSensorModel> farmCheckupSensors = (await DataAccess.LoadDataAsync<PushButtonSensorModel, DynamicParameters>(LoadFarmCheckupSensorQuery, null)).AsList();
            List<PushButtonSensorModel> farmCommuteSensors = (await DataAccess.LoadDataAsync<PushButtonSensorModel, DynamicParameters>(LoadFarmCommuteSensorQuery, null)).AsList();
            List<BinarySensorModel> farmPowerSensors = (await DataAccess.LoadDataAsync<BinarySensorModel, DynamicParameters>(LoadFarmPowerSensorQuery, null)).AsList();
            List<BinarySensorModel> farmPoultryMPowerSensors = (await DataAccess.LoadDataAsync<BinarySensorModel, DynamicParameters>(LoadPoultryMPowerSensorQuery, null)).AsList();
            List<BinarySensorModel> farmPoultryBPowerSensors = (await DataAccess.LoadDataAsync<BinarySensorModel, DynamicParameters>(LoadPoultryBPowerSensorQuery, null)).AsList();

            if (poultries != null)
            {
                foreach (var p in poultries)
                {

                }
            }
            return poultries;
        }
    }
}