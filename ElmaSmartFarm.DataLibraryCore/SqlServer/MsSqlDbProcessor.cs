using Dapper;
using ElmaSmartFarm.DataLibraryCore.Interfaces;
using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.Config;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Alarm;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.SqlServer;

public class MsSqlDbProcessor : IDbProcessor
{
    public MsSqlDbProcessor(IDataAccess dataAccess, Config cfg)
    {
        DataAccess = dataAccess;
        config = cfg;
    }

    private readonly IDataAccess DataAccess;
    private readonly Config config;
    private readonly string LoadFarmsQuery = $@"SELECT * FROM Farms;";
    private const string LoadScalarSensorsQuery = @"SELECT s.*, ssd.*, s.Section FROM dbo.ScalarSensorDetails ssd LEFT JOIN dbo.Sensors s ON ssd.Id = s.Id WHERE s.[Type] = {0};";
    private const string LoadNonScalarSensorsQuery = @"SELECT s.* FROM dbo.Sensors s WHERE s.[Type] = {0};";

    private readonly string LoadFarmScalarSensorsQuery = string.Format(LoadScalarSensorsQuery, (int)SensorType.FarmScalar);
    private readonly string LoadOutdoorScalarSensorsQuery = string.Format(LoadScalarSensorsQuery, (int)SensorType.OutdoorScalar);

    private readonly string LoadFarmFeedSensorsQuery = string.Format(LoadNonScalarSensorsQuery, (int)SensorType.FarmFeed);
    private readonly string LoadFarmCheckupSensorsQuery = string.Format(LoadNonScalarSensorsQuery, (int)SensorType.FarmCheckup);
    private readonly string LoadFarmCommuteSensorsQuery = string.Format(LoadNonScalarSensorsQuery, (int)SensorType.FarmCommute);
    private readonly string LoadFarmPowerSensorsQuery = string.Format(LoadNonScalarSensorsQuery, (int)SensorType.FarmElectricPower);
    private readonly string LoadPoultryMPowerSensorsQuery = string.Format(LoadNonScalarSensorsQuery, (int)SensorType.PoultryMainElectricPower);
    private readonly string LoadPoultryBPowerSensorsQuery = string.Format(LoadNonScalarSensorsQuery, (int)SensorType.PoultryBackupElectricPower);

    private const string LoadActivePeriodsQuery = @"SELECT * FROM dbo.[Periods] p WHERE p.EndDate IS NULL;
            SELECT * FROM dbo.ChickenLosses cl WHERE cl.PeriodId IN (SELECT p.Id FROM dbo.[Periods] p WHERE p.EndDate IS NULL);
            SELECT * FROM dbo.Feeds f WHERE f.PeriodId IN (SELECT p.Id FROM dbo.[Periods] p WHERE p.EndDate IS NULL);";

    private const string LoadSensorErrors = "SELECT * FROM dbo.SensorErrorLogs WHERE DateErased IS NULL;";
    private const string LoadFarmErrors = "SELECT * FROM FarmErrorLogs WHERE DateErased IS NULL;";
    private const string LoadPoultryErrors = "SELECT * FROM PoultryErrorLogs WHERE DateErased IS NULL;";

    private readonly string WriteScalarSensorValueCmd = @"DECLARE @newId INT;
            DECLARE @newTId INT; SET @newTId = (SELECT ISNULL(MAX([Id]), 0) FROM [TemperatureValues]) + 1;
            DECLARE @newHId INT; SET @newHId = (SELECT ISNULL(MAX([Id]), 0) FROM [HumidityValues]) + 1;
            DECLARE @newLId INT; SET @newLId = (SELECT ISNULL(MAX([Id]), 0) FROM [LightValues]) + 1;
            DECLARE @newAId INT; SET @newAId = (SELECT ISNULL(MAX([Id]), 0) FROM [AmmoniaValues]) + 1;
            DECLARE @newCId INT; SET @newCId = (SELECT ISNULL(MAX([Id]), 0) FROM [Co2Values]) + 1;
            SELECT @newId = MAX(val) FROM (VALUES(@newTId),(@newHId),(@newLId),(@newAId),(@newCId)) X(val);
            IF(@Temperature IS NOT NULL) INSERT INTO TemperatureValues(Id, LocationId, Section, SensorId, ReadDate, ReadValue) VALUES(@newId, @LocationId, @Section, @SensorId, @ReadDate, @Temperature);
            IF(@Humidity IS NOT NULL) INSERT INTO HumidityValues(Id, LocationId, Section, SensorId, ReadDate, ReadValue) VALUES(@newId, @LocationId, @Section, @SensorId, @ReadDate, @Humidity);
            IF(@Light IS NOT NULL) INSERT INTO LightValues(Id, LocationId, Section, SensorId, ReadDate, ReadValue) VALUES(@newId, @LocationId, @Section, @SensorId, @ReadDate, @Light);
            IF(@Ammonia IS NOT NULL) INSERT INTO AmmoniaValues(Id, LocationId, Section, SensorId, ReadDate, ReadValue) VALUES(@newId, @LocationId, @Section, @SensorId, @ReadDate, @Ammonia);
            IF(@Co2 IS NOT NULL) INSERT INTO Co2Values(Id, LocationId, Section, SensorId, ReadDate, ReadValue) VALUES(@newId, @LocationId, @Section, @SensorId, @ReadDate, @Co2);
            IF(@Temperature IS NULL AND @Humidity IS NULL AND @Light IS NULL AND @Ammonia IS NULL AND @Co2 IS NULL) SELECT @Id = 0; ELSE SELECT @Id = @newId;";

    private readonly string WriteSensorValueCmd = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [{0}]) + 1;
            INSERT INTO {0}(Id, LocationId, Section, SensorId, ReadDate{1})
            VALUES(@newId, @LocationId, @Section, @SensorId, @ReadDate{2});
            SELECT @Id = @newId;";

    private readonly string WriteSensorErrorCmd = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [SensorErrorLogs]) + 1;
            INSERT INTO SensorErrorLogs (Id, SensorId, LocationId, Section, ErrorType, DateHappened, Descriptions)
            VALUES (@newId, @SensorId, @LocationId, @Section, @ErrorType, @DateHappened, @Descriptions); SELECT @Id = @newId";
    private readonly string WriteFarmErrorCmd = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [FarmErrorLogs]) + 1;
            INSERT INTO FarmErrorLogs (Id, FarmId, PeriodId, ErrorType, DateHappened, CausedSensorId, Descriptions)
            VALUES (@newId, @FarmId, @PeriodId, @ErrorType, @DateHappened, @CausedSensorId, @Descriptions); SELECT @Id = @newId";
    private readonly string WritePoultryErrorCmd = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [PoultryErrorLogs]) + 1;
            INSERT INTO PoultryErrorLogs (Id, ErrorType, DateHappened, Descriptions) VALUES (@newId, @ErrorType, @DateHappened, @Descriptions); SELECT @Id = @newId";
    private readonly string EraseSensorErrorCmd = @"UPDATE SensorErrorLogs SET DateErased = @DateErased WHERE DateErased IS NULL AND SensorId = @SensorId AND ErrorType IN {0};";
    private readonly string EraseFarmErrorCmd = @"UPDATE FarmErrorLogs SET DateErased = @DateErased WHERE DateErased IS NULL AND FarmId = @FarmId AND ErrorType IN {0};";
    private readonly string ErasePoultryErrorCmd = @"UPDATE PoultryErrorLogs SET DateErased = @DateErased WHERE DateErased IS NULL AND ErrorType IN {0};";
    private readonly string LoadAlarmsQuery = @"SELECT * FROM AlarmDevices;";

    public async Task<int> WriteScalarSensorValueToDbAsync(SensorModel sensor, ScalarSensorReadModel value)
    {
        try
        {
            if (config.VerboseMode) Log.Information($"Writing sensor value in database. Sensor ID: {sensor.Id}, LocationID: {sensor.LocationId}, Section: {sensor.Section}");
            DynamicParameters dp = new();
            dp.Add("@Id", 0, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            dp.Add("@LocationId", sensor.LocationId);
            dp.Add("@Section", sensor.Section);
            dp.Add("@SensorId", sensor.Id);
            dp.Add("@ReadDate", value.ReadDate);
            dp.Add("@Temperature", value.Temperature);
            dp.Add("@Humidity", value.Humidity);
            dp.Add("@Light", value.Light);
            dp.Add("@Ammonia", value.Ammonia);
            dp.Add("@Co2", value.Co2);
            _ = await DataAccess.SaveDataAsync(WriteScalarSensorValueCmd, dp);
            var newId = dp.Get<int?>("@Id");
            return newId == null ? 0 : newId.Value;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error when writing sensor value in database. Sensor ID: {sensor.Id}, LocationID: {sensor.LocationId}, Section: {sensor.Section}");
        }
        return 0;
    }

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
            if (sensor.Type == SensorType.FarmCommute) sql = string.Format(WriteSensorValueCmd, "CommuteValues", ", SensorValue", ", @SensorValue");
            else if (sensor.Type.IsPushButtonSensor()) sql = string.Format(WriteSensorValueCmd, "PushButtonSensorValues", "", "");
            else if (sensor.Type.IsBinarySensor()) sql = string.Format(WriteSensorValueCmd, "BinarySensorValues", ", SensorValue", ", @SensorValue");
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

    public async Task<int> WriteFarmErrorToDbAsync(FarmInPeriodErrorModel error, DateTime now)
    {
        try
        {
            if (config.VerboseMode) Log.Information($"Writing farm error in database. Farm ID: {error.FarmId}, Period ID: {error.PeriodId}, Error Type: {error.ErrorType}, Caused sensor ID: {error.CausedSensorId}");
            DynamicParameters dp = new();
            dp.Add("@Id", 0, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            dp.Add("@FarmId", error.FarmId);
            dp.Add("@PeriodId", error.PeriodId);
            dp.Add("@ErrorType", error.ErrorType);
            dp.Add("@DateHappened", now);
            dp.Add("@CausedSensorId", error.CausedSensorId);
            dp.Add("@Descriptions", error.Descriptions);
            _ = await DataAccess.SaveDataAsync(WriteFarmErrorCmd, dp);
            var newId = dp.Get<int>("@Id");
            if (newId == 0) Log.Error($"Error when writing farm error. Farm ID: {error.FarmId}, Period ID: {error.PeriodId}, Error Type: {error.ErrorType}, Caused sensor ID: {error.CausedSensorId}. (System Error)");
            return newId;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error when writing farm error in database. Farm ID: {error.FarmId}, Period ID: {error.PeriodId}, Error Type: {error.ErrorType}, Caused sensor ID: {error.CausedSensorId}");
        }
        return 0;
    }

    public async Task<int> WritePoultryErrorToDbAsync(PoultryInPeriodErrorModel error, DateTime now)
    {
        try
        {
            if (config.VerboseMode) Log.Information($"Writing poultry error in database. Error Type: {error.ErrorType}");
            DynamicParameters dp = new();
            dp.Add("@Id", 0, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            dp.Add("@ErrorType", error.ErrorType);
            dp.Add("@DateHappened", now);
            dp.Add("@Descriptions", error.Descriptions);
            _ = await DataAccess.SaveDataAsync(WritePoultryErrorCmd, dp);
            var newId = dp.Get<int>("@Id");
            if (newId == 0) Log.Error($"Error when writing poultry error. Error Type: {error.ErrorType}. (System Error)");
            return newId;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error when writing poultry error in database. Error Type: {error.ErrorType}");
        }
        return 0;
    }

    public async Task<bool> EraseSensorErrorFromDbAsync(int sensorId, DateTime eraseDate, params SensorErrorType[] types)
    {
        try
        {
            if (types == null || types.Length == 0) return false;
            if (config.VerboseMode) Log.Information($"Updating sensor error in database, if existed. Sensor ID: {sensorId}, Error Types: {types[0]}...");
            DynamicParameters dp = new();
            dp.Add("@SensorId", sensorId);
            dp.Add("@DateErased", eraseDate);
            string errors = "";
            foreach (var t in types) errors += ((int)t).ToString() + ",";
            errors = errors.Remove(errors.Length - 1, 1);
            errors = "(" + errors + ")";
            var sql = string.Format(EraseSensorErrorCmd, errors);
            var i = await DataAccess.SaveDataAsync(sql, dp);
            return i > 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error when updating sensor error in database. Sensor ID: {sensorId}, Error Types: {types[0]}...");
        }
        return false;
    }

    public async Task<bool> EraseFarmErrorFromDbAsync(int farmId, DateTime eraseDate, params FarmInPeriodErrorType[] types)
    {
        try
        {
            if (types == null || types.Length == 0) return false;
            if (config.VerboseMode) Log.Information($"Updating farm error in database, if existed. Farm ID: {farmId}, Error Types: {types[0]}...");
            DynamicParameters dp = new();
            dp.Add("@FarmId", farmId);
            dp.Add("@DateErased", eraseDate);
            string errors = "";
            foreach (var t in types) errors += ((int)t).ToString() + ",";
            errors = errors.Remove(errors.Length - 1, 1);
            errors = "(" + errors + ")";
            var sql = string.Format(EraseFarmErrorCmd, errors);
            var i = await DataAccess.SaveDataAsync(sql, dp);
            return i > 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error when updating farm error in database. Farm ID: {farmId}, Error Types: {types[0]}...");
        }
        return false;
    }

    public async Task<bool> ErasePoultryErrorFromDbAsync(DateTime eraseDate, params PoultryInPeriodErrorType[] types)
    {
        try
        {
            if (types == null || types.Length == 0) return false;
            if (config.VerboseMode) Log.Information($"Updating poultry error in database, if existed. Error Types: {types[0]}...");
            DynamicParameters dp = new();
            dp.Add("@DateErased", eraseDate);
            string errors = "";
            foreach (var t in types) errors += ((int)t).ToString() + ",";
            errors = errors.Remove(errors.Length - 1, 1);
            errors = "(" + errors + ")";
            var sql = string.Format(ErasePoultryErrorCmd, errors);
            var i = await DataAccess.SaveDataAsync(sql, dp);
            return i > 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error when updating poultry error in database. Error Types: {types[0]}...");
        }
        return false;
    }

    public async Task<PoultryModel> LoadPoultryAsync()
    {
        try
        {
            var ticks = DateTime.Now.Ticks;
            Log.Information($"Loading poultry...");
            PoultryModel poultry = new();
            poultry.Name = config.PoultryName;
            poultry.AlarmDevices = (await DataAccess.LoadDataAsync<AlarmModel>(LoadAlarmsQuery)).ToList();
            var farms = await DataAccess.LoadDataAsync<FarmModel>(LoadFarmsQuery);
            var poultryScalarSensor = (await DataAccess.LoadDataAsync<ScalarSensorModel>(LoadOutdoorScalarSensorsQuery)).FirstOrDefault();
            var poultryMPowerSensor = (await DataAccess.LoadDataAsync<BinarySensorModel>(LoadPoultryMPowerSensorsQuery)).FirstOrDefault();
            var poultryBPowerSensor = (await DataAccess.LoadDataAsync<BinarySensorModel>(LoadPoultryBPowerSensorsQuery)).FirstOrDefault();
            var poultryInPeriodErrors = await DataAccess.LoadDataAsync<PoultryInPeriodErrorModel>(LoadPoultryErrors);
            var periods = await LoadActivePeriodsAsync();
            if (farms != null && farms.Any())
            {
                var farmScalarSensors = await DataAccess.LoadDataAsync<ScalarSensorModel>(LoadFarmScalarSensorsQuery);
                var farmFeedSensors = await DataAccess.LoadDataAsync<PushButtonSensorModel>(LoadFarmFeedSensorsQuery);
                var farmCheckupSensors = await DataAccess.LoadDataAsync<PushButtonSensorModel>(LoadFarmCheckupSensorsQuery);
                var farmCommuteSensors = await DataAccess.LoadDataAsync<CommuteSensorModel>(LoadFarmCommuteSensorsQuery);
                var farmPowerSensors = await DataAccess.LoadDataAsync<BinarySensorModel>(LoadFarmPowerSensorsQuery);
                var farmInPeriodErrors = await DataAccess.LoadDataAsync<FarmInPeriodErrorModel>(LoadFarmErrors);
                var sensorErrors = await DataAccess.LoadDataAsync<SensorErrorModel>(LoadSensorErrors);
                if (sensorErrors != null && sensorErrors.Any())
                {
                    if (farmScalarSensors != null) foreach (var s in farmScalarSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                    if (farmFeedSensors != null) foreach (var s in farmFeedSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                    if (farmCheckupSensors != null) foreach (var s in farmCheckupSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                    if (farmCommuteSensors != null) foreach (var s in farmCommuteSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                    if (farmPowerSensors != null) foreach (var s in farmPowerSensors) s.Errors = sensorErrors.Where(se => se.SensorId == s.Id).ToList();
                    if (poultryMPowerSensor != null) poultryMPowerSensor.Errors = sensorErrors.Where(se => se.SensorId == poultryMPowerSensor.Id).ToList();
                    if (poultryBPowerSensor != null) poultryBPowerSensor.Errors = sensorErrors.Where(se => se.SensorId == poultryBPowerSensor.Id).ToList();
                }
                foreach (var f in farms)
                {
                    f.Scalars.Sensors = farmScalarSensors?.Where(s => s.LocationId == f.Id).ToList();
                    f.Feeds.Sensors = farmFeedSensors?.Where(s => s.LocationId == f.Id).ToList();
                    f.Checkups.Sensors = farmCheckupSensors?.Where(s => s.LocationId == f.Id).ToList();
                    f.Commutes.Sensors = farmCommuteSensors?.Where(s => s.LocationId == f.Id).ToList();
                    f.ElectricPowers.Sensors = farmPowerSensors?.Where(s => s.LocationId == f.Id).ToList();
                    f.Period = periods?.Where(p => p.FarmId == f.Id)?.FirstOrDefault();
                    if (periods != null && periods.Any()) f.InPeriodErrors = farmInPeriodErrors?.Where(e => e.FarmId == f.Id).ToList();
                }
            }
            poultry.Farms = farms?.ToList();
            poultry.Scalar = poultryScalarSensor;
            poultry.MainElectricPower = poultryMPowerSensor;
            poultry.BackupElectricPower = poultryBPowerSensor;
            if (periods != null && periods.Any()) poultry.InPeriodErrors = poultryInPeriodErrors?.ToList();
            Log.Information($"Poultry loaded. ({TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds} ms)");
            return poultry;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error when loading poultry.");
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