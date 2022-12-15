using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.Models.Alarm;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using Serilog;

namespace ElmaSmartFarm.Service;

public partial class Worker
{
    private async Task<string?> ObservePoultriesAsync()
    {
        var IsInPeriod = Poultries.Any(p => p.IsInPeriod);
        var Now = DateTime.Now;

        #region Observe Farm Scalar Sensors.
        var FarmScalarSets = Poultries.SelectMany(p => p.Farms.Select(f => f.Scalars));
        if (FarmScalarSets != null)
            foreach (var set in FarmScalarSets)
            {
                if (set.HasSensors)
                {
                    foreach (var sensor in set.EnabledSensors)
                    {
                        if (sensor.IsWatched) await CheckKeepAliveMessageDate(sensor, config.system.KeepAliveWaitingTimeout, Now);
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                var errorTimes = GetErrorTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.InvalidTemperatureData)
                                {
                                    if (errorTimes.Enable && (sensor.WatchTemperature || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if(sensor.WatchTemperature) sensor.WatchTemperature = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchTemperature, sensor.IsInPeriod, config.system.TempInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidTemperatureValue)
                                {
                                    if (errorTimes.Enable && (sensor.WatchTemperature || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchTemperature) sensor.WatchTemperature = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchTemperature, sensor.IsInPeriod, config.system.TempInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidHumidityData)
                                {
                                    if (errorTimes.Enable && (sensor.WatchHumidity || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchHumidity) sensor.WatchHumidity = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchHumidity, sensor.IsInPeriod, config.system.HumidInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidHumidityValue)
                                {
                                    if (errorTimes.Enable && (sensor.WatchHumidity || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchHumidity) sensor.WatchHumidity = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchHumidity, sensor.IsInPeriod, config.system.HumidInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidLightData)
                                {
                                    if (errorTimes.Enable && (sensor.WatchLight || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchLight) sensor.WatchLight = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchLight, sensor.IsInPeriod, config.system.AmbientLightInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidLightValue)
                                {
                                    if (errorTimes.Enable && (sensor.WatchLight || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchLight) sensor.WatchLight = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchLight, sensor.IsInPeriod, config.system.AmbientLightInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidAmmoniaData)
                                {
                                    if (errorTimes.Enable && (sensor.WatchAmmonia || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchAmmonia) sensor.WatchAmmonia = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchAmmonia, sensor.IsInPeriod, config.system.AmmoniaInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidAmmoniaValue)
                                {
                                    if (errorTimes.Enable && (sensor.WatchAmmonia || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchAmmonia) sensor.WatchAmmonia = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchAmmonia, sensor.IsInPeriod, config.system.AmmoniaInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidCo2Data)
                                {
                                    if (errorTimes.Enable && (sensor.WatchCo2 || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchCo2) sensor.WatchCo2 = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchCo2, sensor.IsInPeriod, config.system.Co2InvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidCo2Value)
                                {
                                    if (errorTimes.Enable && (sensor.WatchCo2 || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchCo2) sensor.WatchCo2 = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchCo2, sensor.IsInPeriod, config.system.Co2InvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    if (errorTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.ScalarNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (errorTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                }
                            }
                            if (sensor.IsWatched && !sensor.WatchTemperature && !sensor.WatchHumidity && !sensor.WatchLight && !sensor.WatchAmmonia && !sensor.WatchCo2) //Sensor is fully damaged.
                            {
                                Log.Information($"All units of the sensor is out of watch; Putting the whole sensor out of watch, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}.");
                                sensor.IsWatched = false;
                                //inform, save to db
                            }
                        }
                        var startDate = Poultries.Where(p => p.IsInPeriod).SelectMany(p => p.Farms.Where(f => f.IsInPeriod && f.Scalars.Sensors.Any(s => s.Id == sensor.Id))).FirstOrDefault()?.Period.StartDate;
                        if (!sensor.WatchTemperature && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidTemperatureData || e.ErrorType == SensorErrorType.InvalidTemperatureValue) == false) //Temp sensor is healthy.
                            sensor.WatchTemperature = CheckToReWatchSensor(sensor, startDate);
                        if (!sensor.WatchHumidity && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidHumidityData || e.ErrorType == SensorErrorType.InvalidHumidityValue) == false) //Humidity sensor is healthy.
                            sensor.WatchHumidity = CheckToReWatchSensor(sensor, startDate);
                        if (!sensor.WatchLight && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidLightData || e.ErrorType == SensorErrorType.InvalidLightValue) == false) //Light sensor is healthy.
                            sensor.WatchLight = CheckToReWatchSensor(sensor, startDate);
                        if (!sensor.WatchAmmonia && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidAmmoniaData || e.ErrorType == SensorErrorType.InvalidAmmoniaValue) == false) //Ammonia sensor is healthy.
                            sensor.WatchAmmonia = CheckToReWatchSensor(sensor, startDate);
                        if (!sensor.WatchCo2 && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidCo2Data || e.ErrorType == SensorErrorType.InvalidCo2Value) == false) //Co2 sensor is healthy.
                            sensor.WatchCo2 = CheckToReWatchSensor(sensor, startDate);
                        if (!sensor.IsWatched && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.NotAlive) == false && (sensor.WatchTemperature || sensor.WatchHumidity || sensor.WatchLight || sensor.WatchAmmonia || sensor.WatchCo2)) sensor.IsWatched = CheckToReWatchSensor(sensor, startDate); //Sensor is alive and healthy (again).

                        //Remove expired reads
                    }
                }
            }
        FarmScalarSets = null;
        #endregion
        #region Observe Commute Sensors.
        var CommuteSets = Poultries.SelectMany(p => p.Farms.Select(f => f.Commutes));
        if (CommuteSets != null)
            foreach (var set in CommuteSets)
            {
                if (set.HasSensors)
                {
                    foreach (var sensor in set.EnabledSensors)
                    {
                        if (sensor.IsWatched) await CheckKeepAliveMessageDate(sensor, config.system.KeepAliveWaitingTimeout, Now);
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                var errorTimes = GetErrorTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.InvalidData)
                                {
                                    if (errorTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidValue)
                                {
                                    if (errorTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    if (errorTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (errorTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < errorTimes.CountInCycle) && e.DateHappened.IsElapsed(errorTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                }
                            }
                        }

                        var startDate = Poultries.Where(p => p.IsInPeriod).SelectMany(p => p.Farms.Where(f => f.IsInPeriod && f.Commutes.Sensors.Any(s => s.Id == sensor.Id))).FirstOrDefault()?.Period.StartDate;
                        if (!sensor.IsWatched && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidData || e.ErrorType == SensorErrorType.InvalidValue || e.ErrorType == SensorErrorType.NotAlive) == false) //Sensor is healthy.
                            sensor.IsWatched = CheckToReWatchSensor(sensor, startDate);

                        //Remove expired reads
                    }
                }
            }
        CommuteSets = null;
        #endregion
        #region Observe PushButton Sensors.
        var CheckupSets = Poultries.SelectMany(p => p.Farms.Select(f => f.Checkups));
        var FeedSets = Poultries.SelectMany(p => p.Farms.Select(f => f.Feeds));
        var PushButtonSets = CheckupSets.Concat(FeedSets);
        if (PushButtonSets != null)
            foreach (var set in PushButtonSets)
            {
                if (set.HasSensors)
                {
                    foreach (var sensor in set.EnabledSensors)
                    {
                        if (sensor.IsWatched) await CheckKeepAliveMessageDate(sensor, config.system.KeepAliveWaitingTimeout, Now);
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    bool AlarmPushButtonNotAliveEnable = false;
                                    int AlarmPushButtonNotAliveFirstTime = 0;
                                    int AlarmPushButtonNotAliveCountInCycle = 0;
                                    int PushButtonNotAliveWatchTimeout = 0;
                                    if (sensor.Type == SensorType.FarmFeed)
                                    {
                                        AlarmPushButtonNotAliveEnable = config.system.AlarmFeedNotAliveEnable;
                                        AlarmPushButtonNotAliveFirstTime = config.system.AlarmFeedNotAliveFirstTime;
                                        AlarmPushButtonNotAliveCountInCycle = config.system.AlarmFeedNotAliveCountInCycle;
                                        PushButtonNotAliveWatchTimeout = config.system.FeedNotAliveWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.FarmCheckup)
                                    {
                                        AlarmPushButtonNotAliveEnable = config.system.AlarmCheckupNotAliveEnable;
                                        AlarmPushButtonNotAliveFirstTime = config.system.AlarmCheckupNotAliveFirstTime;
                                        AlarmPushButtonNotAliveCountInCycle = config.system.AlarmCheckupNotAliveCountInCycle;
                                        PushButtonNotAliveWatchTimeout = config.system.CheckupNotAliveWatchTimeout;
                                    }
                                    if (AlarmPushButtonNotAliveEnable && (sensor.IsWatched || e.AlarmInformCount < AlarmPushButtonNotAliveCountInCycle) && e.DateHappened.IsElapsed(AlarmPushButtonNotAliveFirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, PushButtonNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    bool AlarmPushButtonLowBatteryEnable = false;
                                    int AlarmPushButtonLowBatteryFirstTime = 0;
                                    int AlarmPushButtonLowBatteryCountInCycle = 0;
                                    if (sensor.Type == SensorType.FarmFeed)
                                    {
                                        AlarmPushButtonLowBatteryEnable = config.system.AlarmFeedLowBatteryEnable;
                                        AlarmPushButtonLowBatteryFirstTime = config.system.AlarmFeedLowBatteryFirstTime;
                                        AlarmPushButtonLowBatteryCountInCycle = config.system.AlarmFeedLowBatteryCountInCycle;
                                    }
                                    else if (sensor.Type == SensorType.FarmCheckup)
                                    {
                                        AlarmPushButtonLowBatteryEnable = config.system.AlarmCheckupLowBatteryEnable;
                                        AlarmPushButtonLowBatteryFirstTime = config.system.AlarmCheckupLowBatteryFirstTime;
                                        AlarmPushButtonLowBatteryCountInCycle = config.system.AlarmCheckupLowBatteryCountInCycle;
                                    }
                                    if (AlarmPushButtonLowBatteryEnable && (sensor.IsWatched || e.AlarmInformCount < AlarmPushButtonLowBatteryCountInCycle) && e.DateHappened.IsElapsed(AlarmPushButtonLowBatteryFirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                }
                            }
                        }

                        var startDate = Poultries.Where(p => p.IsInPeriod).SelectMany(p => p.Farms.Where(f => f.IsInPeriod && f.Checkups.Sensors.Any(s => s.Id == sensor.Id))).FirstOrDefault()?.Period.StartDate;
                        if (startDate == null) startDate = Poultries.Where(p => p.IsInPeriod).SelectMany(p => p.Farms.Where(f => f.IsInPeriod && f.Feeds.Sensors.Any(s => s.Id == sensor.Id))).FirstOrDefault()?.Period.StartDate;
                        if (!sensor.IsWatched && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.NotAlive) == false) //Sensor is healthy.
                            sensor.IsWatched = CheckToReWatchSensor(sensor, startDate);

                        //Remove expired reads
                    }
                }
            }
        CheckupSets = null;
        FeedSets = null;
        PushButtonSets = null;
        #endregion
        #region Observe Binary Sensors.
        var BinarySets = Poultries.SelectMany(p => p.Farms.Select(f => f.ElectricPowers));
        if (BinarySets != null)
            foreach (var set in BinarySets)
            {
                if (set.HasSensors)
                {
                    foreach (var sensor in set.EnabledSensors)
                    {
                        if (sensor.IsWatched) await CheckKeepAliveMessageDate(sensor, config.system.KeepAliveWaitingTimeout, Now);
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                if (e.ErrorType == SensorErrorType.InvalidData)
                                {
                                    bool AlarmBinaryInvalidDataEnable = false;
                                    int AlarmBinaryInvalidDataFirstTime = 0;
                                    int AlarmBinaryInvalidDataCountInCycle = 0;
                                    int BinaryInvalidDataWatchTimeout = 0;
                                    if (sensor.Type == SensorType.FarmElectricPower)
                                    {
                                        AlarmBinaryInvalidDataEnable = config.system.AlarmFarmPowerInvalidDataEnable;
                                        AlarmBinaryInvalidDataFirstTime = config.system.AlarmFarmPowerInvalidDataFirstTime;
                                        AlarmBinaryInvalidDataCountInCycle = config.system.AlarmFarmPowerInvalidDataCountInCycle;
                                        BinaryInvalidDataWatchTimeout = config.system.FarmPowerInvalidDataWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.PoultryMainElectricPower)
                                    {
                                        AlarmBinaryInvalidDataEnable = config.system.AlarmMainPowerInvalidDataEnable;
                                        AlarmBinaryInvalidDataFirstTime = config.system.AlarmMainPowerInvalidDataFirstTime;
                                        AlarmBinaryInvalidDataCountInCycle = config.system.AlarmMainPowerInvalidDataCountInCycle;
                                        BinaryInvalidDataWatchTimeout = config.system.MainPowerInvalidDataWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.PoultryBackupElectricPower)
                                    {
                                        AlarmBinaryInvalidDataEnable = config.system.AlarmBackupPowerInvalidDataEnable;
                                        AlarmBinaryInvalidDataFirstTime = config.system.AlarmBackupPowerInvalidDataFirstTime;
                                        AlarmBinaryInvalidDataCountInCycle = config.system.AlarmBackupPowerInvalidDataCountInCycle;
                                        BinaryInvalidDataWatchTimeout = config.system.BackupPowerInvalidDataWatchTimeout;
                                    }
                                    if (AlarmBinaryInvalidDataEnable && (sensor.IsWatched || e.AlarmInformCount < AlarmBinaryInvalidDataCountInCycle) && e.DateHappened.IsElapsed(AlarmBinaryInvalidDataFirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, BinaryInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    bool AlarmBinaryNotAliveEnable = false;
                                    int AlarmBinaryNotAliveFirstTime = 0;
                                    int AlarmBinaryNotAliveCountInCycle = 0;
                                    int BinaryNotAliveWatchTimeout = 0;
                                    if (sensor.Type == SensorType.FarmElectricPower)
                                    {
                                        AlarmBinaryNotAliveEnable = config.system.AlarmFarmPowerNotAliveEnable;
                                        AlarmBinaryNotAliveFirstTime = config.system.AlarmFarmPowerNotAliveFirstTime;
                                        AlarmBinaryNotAliveCountInCycle = config.system.AlarmFarmPowerNotAliveCountInCycle;
                                        BinaryNotAliveWatchTimeout = config.system.FarmPowerNotAliveWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.PoultryMainElectricPower)
                                    {
                                        AlarmBinaryNotAliveEnable = config.system.AlarmMainPowerNotAliveEnable;
                                        AlarmBinaryNotAliveFirstTime = config.system.AlarmMainPowerNotAliveFirstTime;
                                        AlarmBinaryNotAliveCountInCycle = config.system.AlarmMainPowerNotAliveCountInCycle;
                                        BinaryNotAliveWatchTimeout = config.system.MainPowerNotAliveWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.PoultryBackupElectricPower)
                                    {
                                        AlarmBinaryNotAliveEnable = config.system.AlarmBackupPowerNotAliveEnable;
                                        AlarmBinaryNotAliveFirstTime = config.system.AlarmBackupPowerNotAliveFirstTime;
                                        AlarmBinaryNotAliveCountInCycle = config.system.AlarmBackupPowerNotAliveCountInCycle;
                                        BinaryNotAliveWatchTimeout = config.system.BackupPowerInvalidDataWatchTimeout;
                                    }
                                    if (AlarmBinaryNotAliveEnable && (sensor.IsWatched || e.AlarmInformCount < AlarmBinaryNotAliveCountInCycle) && e.DateHappened.IsElapsed(AlarmBinaryNotAliveFirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, BinaryNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    bool AlarmBinaryLowBatteryEnable = false;
                                    int AlarmBinaryLowBatteryFirstTime = 0;
                                    int AlarmBinaryLowBatteryCountInCycle = 0;
                                    if (sensor.Type == SensorType.FarmElectricPower)
                                    {
                                        AlarmBinaryLowBatteryEnable = config.system.AlarmFarmPowerLowBatteryEnable;
                                        AlarmBinaryLowBatteryFirstTime = config.system.AlarmFarmPowerLowBatteryFirstTime;
                                        AlarmBinaryLowBatteryCountInCycle = config.system.AlarmFarmPowerLowBatteryCountInCycle;
                                    }
                                    else if (sensor.Type == SensorType.PoultryMainElectricPower)
                                    {
                                        AlarmBinaryLowBatteryEnable = config.system.AlarmMainPowerLowBatteryEnable;
                                        AlarmBinaryLowBatteryFirstTime = config.system.AlarmMainPowerLowBatteryFirstTime;
                                        AlarmBinaryLowBatteryCountInCycle = config.system.AlarmMainPowerLowBatteryCountInCycle;
                                    }
                                    else if (sensor.Type == SensorType.PoultryBackupElectricPower)
                                    {
                                        AlarmBinaryLowBatteryEnable = config.system.AlarmBackupPowerLowBatteryEnable;
                                        AlarmBinaryLowBatteryFirstTime = config.system.AlarmBackupPowerLowBatteryFirstTime;
                                        AlarmBinaryLowBatteryCountInCycle = config.system.AlarmBackupPowerLowBatteryCountInCycle;
                                    }
                                    if (AlarmBinaryLowBatteryEnable && (sensor.IsWatched || e.AlarmInformCount < AlarmBinaryLowBatteryCountInCycle) && e.DateHappened.IsElapsed(AlarmBinaryLowBatteryFirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                }
                            }

                            var startDate = Poultries.Where(p => p.IsInPeriod).SelectMany(p => p.Farms.Where(f => f.IsInPeriod && f.Checkups.Sensors.Any(s => s.Id == sensor.Id))).FirstOrDefault()?.Period.StartDate;
                            if (startDate == null) startDate = Poultries.Where(p => p.IsInPeriod).SelectMany(p => p.Farms.Where(f => f.IsInPeriod && f.Feeds.Sensors.Any(s => s.Id == sensor.Id))).FirstOrDefault()?.Period.StartDate;
                            if (!sensor.IsWatched && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.NotAlive) == false) sensor.IsWatched = CheckToReWatchSensor(sensor, startDate);//Sensor is healthy.

                            //Remove expired reads
                        }
                    }
                }
            }
        BinarySets = null;
        #endregion
        ProcessAlarmableErrors(Now);
        //var m = new MqttApplicationMessageBuilder().WithTopic("safa").WithPayload("dana").Build();
        //if (mqttClient.IsConnected) await mqttClient.PublishAsync(m);
        return null;
    }

    private AlarmTimesModel GetErrorTimings(SensorErrorType e)
    {
        AlarmTimesModel a = new();
        switch (e)
        {
            case SensorErrorType.LowBattery:
                a.Level = config.system.AlarmLevelLowBattery;
                break;
            case SensorErrorType.NotAlive:
                a.Level = config.system.AlarmLevelNotAlive;
                break;
            case SensorErrorType.InvalidData:
            case SensorErrorType.InvalidTemperatureData:
            case SensorErrorType.InvalidHumidityData:
            case SensorErrorType.InvalidLightData:
            case SensorErrorType.InvalidAmmoniaData:
            case SensorErrorType.InvalidCo2Data:
                a.Level = config.system.AlarmLevelInvalidData;
                break;
            case SensorErrorType.InvalidValue:
            case SensorErrorType.InvalidTemperatureValue:
            case SensorErrorType.InvalidHumidityValue:
            case SensorErrorType.InvalidLightValue:
            case SensorErrorType.InvalidAmmoniaValue:
            case SensorErrorType.InvalidCo2Value:
                a.Level = config.system.AlarmLevelInvalidValue;
                break;
        }
        switch (a.Level)
        {
            case 1:
                a.Enable = config.system.AlarmLevelOneEnable;
                a.FirstTime = config.system.AlarmLevelOneFirstTime;
                a.Every = config.system.AlarmLevelOneEvery;
                a.Snooze = config.system.AlarmLevelOneSnooze;
                a.CountInCycle = config.system.AlarmLevelOneCountInCycle;
                break;
            case 2:
                a.Enable = config.system.AlarmLevelTwoEnable;
                a.FirstTime = config.system.AlarmLevelTwoFirstTime;
                a.Every = config.system.AlarmLevelTwoEvery;
                a.Snooze = config.system.AlarmLevelTwoSnooze;
                a.CountInCycle = config.system.AlarmLevelTwoCountInCycle;
                break;
            case 3:
                a.Enable = config.system.AlarmLevelThreeEnable;
                a.FirstTime = config.system.AlarmLevelThreeFirstTime;
                a.Every = config.system.AlarmLevelThreeEvery;
                a.Snooze = config.system.AlarmLevelThreeSnooze;
                a.CountInCycle = config.system.AlarmLevelThreeCountInCycle;
                break;
            case 4:
                a.Enable = config.system.AlarmLevelFourEnable;
                a.FirstTime = config.system.AlarmLevelFourFirstTime;
                a.Every = config.system.AlarmLevelFourEvery;
                a.Snooze = config.system.AlarmLevelFourSnooze;
                a.CountInCycle = config.system.AlarmLevelFourCountInCycle;
                break;
        }
        return a;
    }

    private async Task CheckKeepAliveMessageDate(SensorModel sensor, int keepAliveTimeout, DateTime Now)
    {
        if (sensor.IsWatched && config.system.IsKeepAliveEnabled && !sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.NotAlive) && sensor.KeepAliveMessageDate.IsElapsed(keepAliveTimeout))
        {
            Log.Warning($"Sensor with ID: {sensor.Id} is not sending KeepAlive message since {sensor.KeepAliveMessageDate}. Location: {sensor.LocationId}, Section: {sensor.Section}");
            var newErr = GenerateSensorError(sensor.AsBaseModel(), SensorErrorType.NotAlive, Now, $"Not Alive since: {sensor.KeepAliveMessageDate}");
            sensor.Errors.AddError(newErr, SensorErrorType.NotAlive, config.system.MaxSensorErrorCount);
            await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
        }
    }

    private bool CheckToReWatchSensor(SensorModel sensor, DateTime? PeriodStartDate)
    {
        if (config.system.ObserveAlways || (sensor.IsInPeriod && PeriodStartDate.IsElapsed(sensor.WatchStartDay * 86400)))
        {
            Log.Information($"Sensor unit is back online. ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}");
            //Inform the watch, save to db
            return true;
        }
        return false;
    }

    private bool CheckToUnWatch(SensorErrorModel e, bool SensorWatch, bool UnitWatch, bool SensorIsInPeriod, int Elapse)
    {
        if (SensorWatch && UnitWatch && e.DateHappened.IsElapsed(Elapse) && (SensorIsInPeriod || config.system.ObserveAlways))
        {
            //inform, save to db
            Log.Warning($"Sensor with ID {e.SensorId} has been unwatched due to persisting error. Error Type: {e.ErrorType}, Error happened date: {e.DateHappened}");
            return true;
        }
        return false;
    }

    private void ProcessAlarmableErrors(DateTime Now)
    {
        foreach (var e in AlarmableSensorErrors.ToList())
        {
            if (e.DateErased.HasValue)
            {
                AlarmableSensorErrors.Remove(e);
                //Disable light/siren alarm if active
                continue;
            }
            var sensor = FindSensorsById(e.SensorId);
            if (sensor == null) continue;
            bool alarmEnable = false;
            int firstInformTime = 0;
            int everyInformTime = 0;
            int snoozeTime = 0;
            int informCycleCount = 0;
            if (sensor.Type == SensorType.FarmScalar)
            {
                if (e.ErrorType == SensorErrorType.InvalidTemperatureData)
                {
                    alarmEnable = config.system.AlarmTempInvalidDataEnable;
                    firstInformTime = config.system.AlarmTempInvalidDataFirstTime;
                    everyInformTime = config.system.AlarmTempInvalidDataEvery;
                    snoozeTime = config.system.AlarmTempInvalidDataSnooze;
                    informCycleCount =  config.system.AlarmTempInvalidDataCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.InvalidTemperatureValue)
                {
                    alarmEnable = config.system.AlarmTempInvalidValueEnable;
                    firstInformTime = config.system.AlarmTempInvalidValueFirstTime;
                    everyInformTime = config.system.AlarmTempInvalidValueEvery;
                    snoozeTime = config.system.AlarmTempInvalidValueSnooze;
                    informCycleCount = config.system.AlarmTempInvalidValueCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.InvalidHumidityData)
                {
                    alarmEnable = config.system.AlarmHumidInvalidDataEnable;
                    firstInformTime = config.system.AlarmHumidInvalidDataFirstTime;
                    everyInformTime = config.system.AlarmHumidInvalidDataEvery;
                    snoozeTime = config.system.AlarmHumidInvalidDataSnooze;
                    informCycleCount = config.system.AlarmHumidInvalidDataCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.InvalidHumidityValue)
                {
                    alarmEnable = config.system.AlarmHumidInvalidValueEnable;
                    firstInformTime = config.system.AlarmHumidInvalidValueFirstTime;
                    everyInformTime = config.system.AlarmHumidInvalidValueEvery;
                    snoozeTime = config.system.AlarmHumidInvalidValueSnooze;
                    informCycleCount = config.system.AlarmHumidInvalidValueCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.InvalidLightData)
                {
                    alarmEnable = config.system.AlarmAmbientLightInvalidDataEnable;
                    firstInformTime = config.system.AlarmAmbientLightInvalidDataFirstTime;
                    everyInformTime = config.system.AlarmAmbientLightInvalidDataEvery;
                    snoozeTime = config.system.AlarmAmbientLightInvalidDataSnooze;
                    informCycleCount = config.system.AlarmAmbientLightInvalidDataCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.InvalidLightValue)
                {
                    alarmEnable = config.system.AlarmAmbientLightInvalidValueEnable;
                    firstInformTime = config.system.AlarmAmbientLightInvalidValueFirstTime;
                    everyInformTime = config.system.AlarmAmbientLightInvalidValueEvery;
                    snoozeTime = config.system.AlarmAmbientLightInvalidValueSnooze;
                    informCycleCount = config.system.AlarmAmbientLightInvalidValueCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.InvalidAmmoniaData)
                {
                    alarmEnable = config.system.AlarmAmmoniaInvalidDataEnable;
                    firstInformTime = config.system.AlarmAmmoniaInvalidDataFirstTime;
                    everyInformTime = config.system.AlarmAmmoniaInvalidDataEvery;
                    snoozeTime = config.system.AlarmAmmoniaInvalidDataSnooze;
                    informCycleCount = config.system.AlarmAmmoniaInvalidDataCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.InvalidAmmoniaValue)
                {
                    alarmEnable = config.system.AlarmAmmoniaInvalidValueEnable;
                    firstInformTime = config.system.AlarmAmmoniaInvalidValueFirstTime;
                    everyInformTime = config.system.AlarmAmmoniaInvalidValueEvery;
                    snoozeTime = config.system.AlarmAmmoniaInvalidValueSnooze;
                    informCycleCount = config.system.AlarmAmmoniaInvalidValueCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.InvalidCo2Data)
                {
                    alarmEnable = config.system.AlarmCo2InvalidDataEnable;
                    firstInformTime = config.system.AlarmCo2InvalidDataFirstTime;
                    everyInformTime = config.system.AlarmCo2InvalidDataEvery;
                    snoozeTime = config.system.AlarmCo2InvalidDataSnooze;
                    informCycleCount = config.system.AlarmCo2InvalidDataCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.InvalidCo2Value)
                {
                    alarmEnable = config.system.AlarmCo2InvalidValueEnable;
                    firstInformTime = config.system.AlarmCo2InvalidValueFirstTime;
                    everyInformTime = config.system.AlarmCo2InvalidValueEvery;
                    snoozeTime = config.system.AlarmCo2InvalidValueSnooze;
                    informCycleCount = config.system.AlarmCo2InvalidValueCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.NotAlive)
                {
                    alarmEnable = config.system.AlarmScalarNotAliveEnable;
                    firstInformTime = config.system.AlarmScalarNotAliveFirstTime;
                    everyInformTime = config.system.AlarmScalarNotAliveEvery;
                    snoozeTime = config.system.AlarmScalarNotAliveSnooze;
                    informCycleCount = config.system.AlarmScalarNotAliveCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.LowBattery)
                {
                    alarmEnable = config.system.AlarmScalarLowBatteryEnable;
                    firstInformTime = config.system.AlarmScalarLowBatteryFirstTime;
                    everyInformTime = config.system.AlarmScalarLowBatteryEvery;
                    snoozeTime = config.system.AlarmScalarLowBatterySnooze;
                    informCycleCount = config.system.AlarmScalarLowBatteryCountInCycle;
                }
            }
            else if(sensor.Type == SensorType.FarmCommute)
            {
                if (e.ErrorType == SensorErrorType.InvalidData)
                {
                    alarmEnable = config.system.AlarmCommuteInvalidDataEnable;
                    firstInformTime = config.system.AlarmCommuteInvalidDataFirstTime;
                    everyInformTime = config.system.AlarmCommuteInvalidDataEvery;
                    snoozeTime = config.system.AlarmCommuteInvalidDataSnooze;
                    informCycleCount = config.system.AlarmCommuteInvalidDataCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.InvalidValue)
                {
                    alarmEnable = config.system.AlarmCommuteInvalidValueEnable;
                    firstInformTime = config.system.AlarmCommuteInvalidValueFirstTime;
                    everyInformTime = config.system.AlarmCommuteInvalidValueEvery;
                    snoozeTime = config.system.AlarmCommuteInvalidValueSnooze;
                    informCycleCount = config.system.AlarmCommuteInvalidValueCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.NotAlive)
                {
                    alarmEnable = config.system.AlarmCommuteNotAliveEnable;
                    firstInformTime = config.system.AlarmCommuteNotAliveFirstTime;
                    everyInformTime = config.system.AlarmCommuteNotAliveEvery;
                    snoozeTime = config.system.AlarmCommuteNotAliveSnooze;
                    informCycleCount = config.system.AlarmCommuteNotAliveCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.LowBattery)
                {
                    alarmEnable = config.system.AlarmCommuteLowBatteryEnable;
                    firstInformTime = config.system.AlarmCommuteLowBatteryFirstTime;
                    everyInformTime = config.system.AlarmCommuteLowBatteryEvery;
                    snoozeTime = config.system.AlarmCommuteLowBatterySnooze;
                    informCycleCount = config.system.AlarmCommuteLowBatteryCountInCycle;
                }
            }
            else if(sensor.Type == SensorType.FarmFeed)
            {
                if (e.ErrorType == SensorErrorType.NotAlive)
                {
                    alarmEnable = config.system.AlarmFeedNotAliveEnable;
                    firstInformTime = config.system.AlarmFeedNotAliveFirstTime;
                    everyInformTime = config.system.AlarmFeedNotAliveEvery;
                    snoozeTime = config.system.AlarmFeedNotAliveSnooze;
                    informCycleCount = config.system.AlarmFeedNotAliveCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.LowBattery)
                {
                    alarmEnable = config.system.AlarmFeedLowBatteryEnable;
                    firstInformTime = config.system.AlarmFeedLowBatteryFirstTime;
                    everyInformTime = config.system.AlarmFeedLowBatteryEvery;
                    snoozeTime = config.system.AlarmFeedLowBatterySnooze;
                    informCycleCount = config.system.AlarmFeedLowBatteryCountInCycle;
                }
            }
            else if(sensor.Type == SensorType.FarmCheckup)
            {
                if (e.ErrorType == SensorErrorType.NotAlive)
                {
                    alarmEnable = config.system.AlarmCheckupNotAliveEnable;
                    firstInformTime = config.system.AlarmCheckupNotAliveFirstTime;
                    everyInformTime = config.system.AlarmCheckupNotAliveEvery;
                    snoozeTime = config.system.AlarmCheckupNotAliveSnooze;
                    informCycleCount = config.system.AlarmCheckupNotAliveCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.LowBattery)
                {
                    alarmEnable = config.system.AlarmCheckupLowBatteryEnable;
                    firstInformTime = config.system.AlarmCheckupLowBatteryFirstTime;
                    everyInformTime = config.system.AlarmCheckupLowBatteryEvery;
                    snoozeTime = config.system.AlarmCheckupLowBatterySnooze;
                    informCycleCount = config.system.AlarmCheckupLowBatteryCountInCycle;
                }
            }
            else if(sensor.Type == SensorType.FarmElectricPower)
            {
                if (e.ErrorType == SensorErrorType.InvalidData)
                {
                    alarmEnable = config.system.AlarmFarmPowerInvalidDataEnable;
                    firstInformTime = config.system.AlarmFarmPowerInvalidDataFirstTime;
                    everyInformTime = config.system.AlarmFarmPowerInvalidDataEvery;
                    snoozeTime = config.system.AlarmFarmPowerInvalidDataSnooze;
                    informCycleCount = config.system.AlarmFarmPowerInvalidDataCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.NotAlive)
                {
                    alarmEnable = config.system.AlarmFarmPowerNotAliveEnable;
                    firstInformTime = config.system.AlarmFarmPowerNotAliveFirstTime;
                    everyInformTime = config.system.AlarmFarmPowerNotAliveEvery;
                    snoozeTime = config.system.AlarmFarmPowerNotAliveSnooze;
                    informCycleCount = config.system.AlarmFarmPowerNotAliveCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.LowBattery)
                {
                    alarmEnable = config.system.AlarmFarmPowerLowBatteryEnable;
                    firstInformTime = config.system.AlarmFarmPowerLowBatteryFirstTime;
                    everyInformTime = config.system.AlarmFarmPowerLowBatteryEvery;
                    snoozeTime = config.system.AlarmFarmPowerLowBatterySnooze;
                    informCycleCount = config.system.AlarmFarmPowerLowBatteryCountInCycle;
                }
            }
            else if(sensor.Type == SensorType.PoultryMainElectricPower)
            {
                if (e.ErrorType == SensorErrorType.InvalidData)
                {
                    alarmEnable = config.system.AlarmMainPowerInvalidDataEnable;
                    firstInformTime = config.system.AlarmMainPowerInvalidDataFirstTime;
                    everyInformTime = config.system.AlarmMainPowerInvalidDataEvery;
                    snoozeTime = config.system.AlarmMainPowerInvalidDataSnooze;
                    informCycleCount = config.system.AlarmMainPowerInvalidDataCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.NotAlive)
                {
                    alarmEnable = config.system.AlarmMainPowerNotAliveEnable;
                    firstInformTime = config.system.AlarmMainPowerNotAliveFirstTime;
                    everyInformTime = config.system.AlarmMainPowerNotAliveEvery;
                    snoozeTime = config.system.AlarmMainPowerNotAliveSnooze;
                    informCycleCount = config.system.AlarmMainPowerNotAliveCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.LowBattery)
                {
                    alarmEnable = config.system.AlarmMainPowerLowBatteryEnable;
                    firstInformTime = config.system.AlarmMainPowerLowBatteryFirstTime;
                    everyInformTime = config.system.AlarmMainPowerLowBatteryEvery;
                    snoozeTime = config.system.AlarmMainPowerLowBatterySnooze;
                    informCycleCount = config.system.AlarmMainPowerLowBatteryCountInCycle;
                }
            }
            else if(sensor.Type == SensorType.PoultryBackupElectricPower)
            {
                if (e.ErrorType == SensorErrorType.InvalidData)
                {
                    alarmEnable = config.system.AlarmBackupPowerInvalidDataEnable;
                    firstInformTime = config.system.AlarmBackupPowerInvalidDataFirstTime;
                    everyInformTime = config.system.AlarmBackupPowerInvalidDataEvery;
                    snoozeTime = config.system.AlarmBackupPowerInvalidDataSnooze;
                    informCycleCount = config.system.AlarmBackupPowerInvalidDataCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.NotAlive)
                {
                    alarmEnable = config.system.AlarmBackupPowerNotAliveEnable;
                    firstInformTime = config.system.AlarmBackupPowerNotAliveFirstTime;
                    everyInformTime = config.system.AlarmBackupPowerNotAliveEvery;
                    snoozeTime = config.system.AlarmBackupPowerNotAliveSnooze;
                    informCycleCount = config.system.AlarmBackupPowerNotAliveCountInCycle;
                }
                else if (e.ErrorType == SensorErrorType.LowBattery)
                {
                    alarmEnable = config.system.AlarmBackupPowerLowBatteryEnable;
                    firstInformTime = config.system.AlarmBackupPowerLowBatteryFirstTime;
                    everyInformTime = config.system.AlarmBackupPowerLowBatteryEvery;
                    snoozeTime = config.system.AlarmBackupPowerLowBatterySnooze;
                    informCycleCount = config.system.AlarmBackupPowerLowBatteryCountInCycle;
                }
            }
            if (alarmEnable)
            {
                if (e.DateAlarmInformed == null && e.DateHappened.IsElapsed(firstInformTime)) //first alarm.
                {
                    e.AlarmInformCount = 1;
                    Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.AlarmInformCount}");
                    if (sensor.IsFarmSensor())
                    {

                    }
                    e.DateAlarmInformed = Now;
                }
                else if (e.AlarmInformCount % informCycleCount != 0 && e.DateAlarmInformed.IsElapsed(everyInformTime)) //alarm every.
                {
                    e.AlarmInformCount++;
                    Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.AlarmInformCount}");
                    //inform, save to db
                    e.DateAlarmInformed = Now;
                }
                else if (e.AlarmInformCount % informCycleCount == 0 && e.DateAlarmInformed.IsElapsed(snoozeTime)) //alarm sleep.
                {
                    e.AlarmInformCount++;
                    Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.AlarmInformCount}");
                    //inform, save to db
                    e.DateAlarmInformed = Now;
                }
            }
        }
    }

    private async Task RunObserverTimerAsync()
    {
        long ticks;
        while (true)
        {
            if (CanRunObserver)
            {
                ticks = DateTime.Now.Ticks;
                if (config.VerboseMode) Log.Information($"===============  Start observation process ===============");
                try
                {
                    if (Poultries != null && Poultries.Count > 0 && (Poultries.Any(p => p.IsInPeriod) || config.system.ObserveAlways))
                    {
                        var result = await ObservePoultriesAsync();
                        if (!string.IsNullOrEmpty(result))
                            Log.Error($"Observation process returned with error: {result}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception thrown in Observer:");
                }
                if (config.VerboseMode) Log.Information($"===============  End of observation process ({TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds} ms) ===============");
                await Task.Delay(TimeSpan.FromSeconds(config.system.ObserverCheckInterval));
            }
        }
    }
}