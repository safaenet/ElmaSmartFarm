using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Alarm;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using Serilog;

namespace ElmaSmartFarm.Service;

public partial class Worker
{
    private async Task<string?> ObservePoultryAsync()
    {
        var Now = DateTime.Now;

        #region Observe Farm Scalar Sensors.
        var FarmScalarSets = Poultry.Farms.Select(f => f.Scalars);
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
                                var alarmTimes = GetAlarmTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.InvalidTemperatureData)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchTemperature) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchTemperature) sensor.WatchTemperature = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchTemperature, sensor.IsInPeriod, config.system.TempInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidTemperatureValue)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchTemperature) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchTemperature) sensor.WatchTemperature = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchTemperature, sensor.IsInPeriod, config.system.TempInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidHumidityData)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchHumidity) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchHumidity) sensor.WatchHumidity = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchHumidity, sensor.IsInPeriod, config.system.HumidInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidHumidityValue)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchHumidity) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchHumidity) sensor.WatchHumidity = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchHumidity, sensor.IsInPeriod, config.system.HumidInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidLightData)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchLight) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchLight) sensor.WatchLight = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchLight, sensor.IsInPeriod, config.system.AmbientLightInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidLightValue)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchLight) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchLight) sensor.WatchLight = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchLight, sensor.IsInPeriod, config.system.AmbientLightInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidAmmoniaData)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchAmmonia) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchAmmonia) sensor.WatchAmmonia = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchAmmonia, sensor.IsInPeriod, config.system.AmmoniaInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidAmmoniaValue)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchAmmonia) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchAmmonia) sensor.WatchAmmonia = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchAmmonia, sensor.IsInPeriod, config.system.AmmoniaInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidCo2Data)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchCo2) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchCo2) sensor.WatchCo2 = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchCo2, sensor.IsInPeriod, config.system.Co2InvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidCo2Value)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchCo2) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchCo2) sensor.WatchCo2 = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchCo2, sensor.IsInPeriod, config.system.Co2InvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.ScalarNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                }
                            }
                            if (sensor.IsWatched && !sensor.WatchTemperature && !sensor.WatchHumidity && !sensor.WatchLight && !sensor.WatchAmmonia && !sensor.WatchCo2) //Sensor is fully damaged.
                            {
                                Log.Information($"All units of the sensor is out of watch; Putting the whole sensor out of watch, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}.");
                                sensor.IsWatched = false;
                                //inform, save to db
                            }
                        }
                        var startDate = Poultry.Farms.Where(f => f.IsInPeriod && f.Scalars.Sensors.Any(s => s.Id == sensor.Id)).FirstOrDefault()?.Period.StartDate;
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
        var CommuteSets = Poultry.Farms.Select(f => f.Commutes);
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
                                var alarmTimes = GetAlarmTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.InvalidData)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidValue)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                }
                            }
                        }

                        var startDate = Poultry.Farms.Where(f => f.IsInPeriod && f.Commutes.Sensors.Any(s => s.Id == sensor.Id)).FirstOrDefault()?.Period.StartDate;
                        if (!sensor.IsWatched && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidData || e.ErrorType == SensorErrorType.InvalidValue || e.ErrorType == SensorErrorType.NotAlive) == false) //Sensor is healthy.
                            sensor.IsWatched = CheckToReWatchSensor(sensor, startDate);

                        //Remove expired reads

                        if (sensor.IsInPeriod && ((sensor.LastCommuteDate == null && SystemUpTime.IsElapsed(config.system.FarmCheckupInterval)) || (sensor.LastCommuteDate != null && sensor.LastCommuteDate.IsElapsed(config.system.FarmCheckupInterval))))
                        {
                            var farm = FindFarmBySensorId(sensor.Id);
                            if (farm == null) Log.Error($"Farm for the indoor sensor not detected. Sensor ID: {sensor.Id} (System Error)");
                            else await AddFarmError(farm, sensor, sensor.LastRead == null ? SystemUpTime : sensor.LastRead.ReadDate, FarmInPeriodErrorType.LongLeave, Now);
                        }
                    }
                }
            }
        CommuteSets = null;
        #endregion
        #region Observe PushButton Sensors.
        var CheckupSets = Poultry.Farms.Select(f => f.Checkups);
        var FeedSets = Poultry.Farms.Select(f => f.Feeds);
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
                                var alarmTimes = GetAlarmTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    int PushButtonNotAliveWatchTimeout = 0;
                                    if (sensor.Type == SensorType.FarmFeed) PushButtonNotAliveWatchTimeout = config.system.FeedNotAliveWatchTimeout;
                                    else if (sensor.Type == SensorType.FarmCheckup) PushButtonNotAliveWatchTimeout = config.system.CheckupNotAliveWatchTimeout;
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, PushButtonNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                }
                            }
                        }

                        var startDate = Poultry.Farms.Where(f => f.IsInPeriod && f.Checkups.Sensors.Any(s => s.Id == sensor.Id)).FirstOrDefault()?.Period.StartDate;
                        if (startDate == null) startDate = Poultry.Farms.Where(f => f.IsInPeriod && f.Feeds.Sensors.Any(s => s.Id == sensor.Id)).FirstOrDefault()?.Period.StartDate;
                        if (!sensor.IsWatched && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.NotAlive) == false) //Sensor is healthy.
                            sensor.IsWatched = CheckToReWatchSensor(sensor, startDate);

                        //Remove expired reads

                        if (sensor.IsInPeriod)
                        {
                            if (sensor.Type == SensorType.FarmFeed)
                            {
                                if ((sensor.LastRead == null && SystemUpTime.IsElapsed(config.system.FeedInterval)) || (sensor.LastRead != null && sensor.LastRead.ReadDate.IsElapsed(config.system.FeedInterval)))
                                {
                                    var farm = FindFarmBySensorId(sensor.Id);
                                    if (farm == null) Log.Error($"Farm for the indoor sensor not detected. Sensor ID: {sensor.Id} (System Error)");
                                    else await AddFarmError(farm, sensor, sensor.LastRead == null ? SystemUpTime : sensor.LastRead.ReadDate, FarmInPeriodErrorType.LongNoFeed, Now);
                                }
                            }
                            else if (sensor.Type == SensorType.FarmCheckup)
                            {
                                if ((sensor.LastRead == null && SystemUpTime.IsElapsed(config.system.FarmCheckupInterval)) || (sensor.LastRead != null && sensor.LastRead.ReadDate.IsElapsed(config.system.FarmCheckupInterval)))
                                {
                                    var farm = FindFarmBySensorId(sensor.Id);
                                    if (farm == null) Log.Error($"Farm for the indoor sensor not detected. Sensor ID: {sensor.Id} (System Error)");
                                    else await AddFarmError(farm, sensor, sensor.LastRead == null ? SystemUpTime : sensor.LastRead.ReadDate, FarmInPeriodErrorType.LongLeave, Now);
                                }
                            }
                        }
                    }
                }
            }
        CheckupSets = null;
        FeedSets = null;
        PushButtonSets = null;
        #endregion
        #region Observe Binary Sensors.
        var BinarySets = Poultry.Farms.Select(f => f.ElectricPowers);
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
                                var alarmTimes = GetAlarmTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.InvalidData)
                                {
                                    int BinaryInvalidDataWatchTimeout = 0;
                                    if (sensor.Type == SensorType.FarmElectricPower)
                                    {
                                        BinaryInvalidDataWatchTimeout = config.system.FarmPowerInvalidDataWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.PoultryMainElectricPower)
                                    {
                                        BinaryInvalidDataWatchTimeout = config.system.MainPowerInvalidDataWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.PoultryBackupElectricPower)
                                    {
                                        BinaryInvalidDataWatchTimeout = config.system.BackupPowerInvalidDataWatchTimeout;
                                    }
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, BinaryInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    int BinaryNotAliveWatchTimeout = 0;
                                    if (sensor.Type == SensorType.FarmElectricPower)
                                    {
                                        BinaryNotAliveWatchTimeout = config.system.FarmPowerNotAliveWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.PoultryMainElectricPower)
                                    {
                                        BinaryNotAliveWatchTimeout = config.system.MainPowerNotAliveWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.PoultryBackupElectricPower)
                                    {
                                        BinaryNotAliveWatchTimeout = config.system.BackupPowerInvalidDataWatchTimeout;
                                    }
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, BinaryNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                }
                            }
                        }

                        var startDate = Poultry.Farms.Where(f => f.IsInPeriod && f.Checkups.Sensors.Any(s => s.Id == sensor.Id)).FirstOrDefault()?.Period.StartDate;
                        if (startDate == null) startDate = Poultry.Farms.Where(f => f.IsInPeriod && f.Feeds.Sensors.Any(s => s.Id == sensor.Id)).FirstOrDefault()?.Period.StartDate;
                        if (!sensor.IsWatched && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.NotAlive) == false) sensor.IsWatched = CheckToReWatchSensor(sensor, startDate);//Sensor is healthy.

                        //Remove expired reads
                                                
                    }
                }
            }
        BinarySets = null;
        #endregion

        #region Check for Alarmable Poultry/Farm Errors
        if (Poultry.IsInPeriod)
        {
            foreach (var error in Poultry.Farms.Where(f => f.HasPeriodError).SelectMany(f => f.InPeriodErrors).Where(e => e.DateErased == null))
            {
                var errorTimings = GetAlarmTimings(error.ErrorType);
                if (errorTimings.Enable && error.DateHappened.IsElapsed(errorTimings.RaiseTime) && !AlarmableFarmPeriodErrors.Contains(error)) AlarmableFarmPeriodErrors.Add(error);
            }
            foreach (var error in Poultry.InPeriodErrors.Where(e => e.DateErased == null))
            {
                var errorTimings = GetAlarmTimings(error.ErrorType);
                if (errorTimings.Enable && error.DateHappened.IsElapsed(errorTimings.RaiseTime) && !AlarmablePoultryPeriodErrors.Contains(error)) AlarmablePoultryPeriodErrors.Add(error);
            }
        }
        #endregion

        ProcessAlarmableErrors(Now);
        //var m = new MqttApplicationMessageBuilder().WithTopic("safa").WithPayload("dana").Build();
        //if (mqttClient.IsConnected) await mqttClient.PublishAsync(m);
        return null;
    }

    private async Task AddFarmError(FarmModel farm , SensorModel sensor, DateTime? ReadDate, FarmInPeriodErrorType ErrorType, DateTime Now)
    {
        if (config.VerboseMode) Log.Warning($"{ErrorType} detected in one of farms. sensor ID: {sensor.Id}");
        if (farm != null)
        {
            if (config.VerboseMode) Log.Warning($"{ErrorType} detected in farm ID: {farm.Id}, Name: {farm.Name}. sensor ID: {sensor.Id}");
            var newErr = GenerateFarmError(sensor, ErrorType, Now, farm.Period?.Id ?? 0, $"{ErrorType} since: {(ReadDate == null ? SystemUpTime : ReadDate)}, Detected by: {sensor.Type}");
            if (farm.InPeriodErrors == null) farm.InPeriodErrors = new();
            if (farm.InPeriodErrors.AddError(newErr, ErrorType, config.system.MaxFarmErrorCount))
            {
                var newId = await DbProcessor.WriteFarmErrorToDbAsync(newErr, Now);
                if (newId > 0) newErr.Id = newId;
            }
        }
    }
    private AlarmTimesModel GetAlarmTimings(SensorErrorType e)
    {
        int level = 0;
        switch (e)
        {
            case SensorErrorType.LowBattery:
                level = config.system.AlarmLevelLowBattery;
                break;
            case SensorErrorType.NotAlive:
                level = config.system.AlarmLevelNotAlive;
                break;
            case SensorErrorType.InvalidData:
            case SensorErrorType.InvalidTemperatureData:
            case SensorErrorType.InvalidHumidityData:
            case SensorErrorType.InvalidLightData:
            case SensorErrorType.InvalidAmmoniaData:
            case SensorErrorType.InvalidCo2Data:
                level = config.system.AlarmLevelInvalidData;
                break;
            case SensorErrorType.InvalidValue:
            case SensorErrorType.InvalidTemperatureValue:
            case SensorErrorType.InvalidHumidityValue:
            case SensorErrorType.InvalidLightValue:
            case SensorErrorType.InvalidAmmoniaValue:
            case SensorErrorType.InvalidCo2Value:
                level = config.system.AlarmLevelInvalidValue;
                break;
        }
        return GetAlarmTimingByLevel(level);
    }

    private AlarmTimesModel GetAlarmTimings(FarmInPeriodErrorType e)
    {
        int level = 0;
        switch (e)
        {
            case FarmInPeriodErrorType.HighTemperature:
                level = config.system.AlarmLevelHighTemperature;
                break;
            case FarmInPeriodErrorType.LowTemperature:
                level = config.system.AlarmLevelLowTemperature;
                break;
            case FarmInPeriodErrorType.HighHumidity:
                level = config.system.AlarmLevelHighHumid;
                break;
            case FarmInPeriodErrorType.LowHumidity:
                level = config.system.AlarmLevelLowHumid;
                break;
            case FarmInPeriodErrorType.HighAmmonia:
                level = config.system.AlarmLevelHighAmmonia;
                break;
            case FarmInPeriodErrorType.HighCo2:
                level = config.system.AlarmLevelHighCo2;
                break;
            case FarmInPeriodErrorType.LongTimeBright:
                level = config.system.AlarmLevelLongTimeBright;
                break;
            case FarmInPeriodErrorType.LongTimeDark:
                level = config.system.AlarmLevelLongTimeDark;
                break;
            case FarmInPeriodErrorType.HighBrightness:
                level = config.system.AlarmLevelHighBrightness;
                break;
            case FarmInPeriodErrorType.LowBrightness:
                level = config.system.AlarmLevelLowBrightness;
                break;
            case FarmInPeriodErrorType.LongNoFeed:
                level = config.system.AlarmLevelLongNoFeed;
                break;
            case FarmInPeriodErrorType.LongLeave:
                level = config.system.AlarmLevelLongLeave;
                break;
            case FarmInPeriodErrorType.NoPower:
                level = config.system.AlarmLevelNoPower;
                break;
        }
        return GetAlarmTimingByLevel(level);
    }

    private AlarmTimesModel GetAlarmTimings(PoultryInPeriodErrorType e)
    {
        int level = 0;
        switch (e)
        {
            case PoultryInPeriodErrorType.NoMainPower:
                level = config.system.AlarmLevelNoMainPower;
                break;
            case PoultryInPeriodErrorType.NoBackupPower:
                level = config.system.AlarmLevelNoBackupPower;
                break;
        }
        return GetAlarmTimingByLevel(level);
    }

    private AlarmTimesModel GetAlarmTimingByLevel(int level)
    {
        AlarmTimesModel a = new();
        a.Level = level;
        switch (a.Level)
        {
            case 1:
                a.Enable = config.system.AlarmLevelOneEnable;
                a.RaiseTime = config.system.AlarmLevelOneRaiseTime;
                a.FarmAlarmEnable = config.system.AlarmLevelOneFarmAlarmEnable;
                a.FarmAlarmRaiseTimeOffset = config.system.AlarmLevelOneFarmAlarmRaiseTimeOffset;
                a.FarmAlarmEvery = config.system.AlarmLevelOneFarmAlarmEvery;
                a.FarmAlarmSnooze = config.system.AlarmLevelOneFarmAlarmSnooze;
                a.FarmAlarmCountInCycle = config.system.AlarmLevelOneFarmAlarmCountInCycle;

                a.SmsEnable = config.system.AlarmLevelOneSmsEnable;
                a.SmsRaiseTimeOffset = config.system.AlarmLevelOneSmsRaiseTimeOffset;
                a.SmsEvery = config.system.AlarmLevelOneSmsEvery;
                a.SmsSnooze = config.system.AlarmLevelOneSmsSnooze;
                a.SmsCountInCycle = config.system.AlarmLevelOneSmsCountInCycle;

                a.PoultryLightAlarmEnable = config.system.AlarmLevelOnePoultryLightAlarmEnable;
                a.PoultryLightAlarmRaiseTimeOffset = config.system.AlarmLevelOnePoultryLightAlarmRaiseTimeOffset;
                a.PoultryLightAlarmEvery = config.system.AlarmLevelOnePoultryLightAlarmEvery;
                a.PoultryLightAlarmSnooze = config.system.AlarmLevelOnePoultryLightAlarmSnooze;
                a.PoultryLightAlarmCountInCycle = config.system.AlarmLevelOnePoultryLightAlarmCountInCycle;

                a.PoultrySirenAlarmEnable = config.system.AlarmLevelOnePoultryLightAlarmEnable;
                a.PoultrySirenAlarmRaiseTimeOffset = config.system.AlarmLevelOnePoultryLightAlarmRaiseTimeOffset;
                a.PoultrySirenAlarmEvery = config.system.AlarmLevelOnePoultryLightAlarmEvery;
                a.PoultrySirenAlarmSnooze = config.system.AlarmLevelOnePoultryLightAlarmSnooze;
                a.PoultrySirenAlarmCountInCycle = config.system.AlarmLevelOnePoultryLightAlarmCountInCycle;
                break;
            case 2:
                a.Enable = config.system.AlarmLevelTwoEnable;
                a.RaiseTime = config.system.AlarmLevelTwoRaiseTime;
                a.FarmAlarmEnable = config.system.AlarmLevelTwoFarmAlarmEnable;
                a.FarmAlarmRaiseTimeOffset = config.system.AlarmLevelTwoFarmAlarmRaiseTimeOffset;
                a.FarmAlarmEvery = config.system.AlarmLevelTwoFarmAlarmEvery;
                a.FarmAlarmSnooze = config.system.AlarmLevelTwoFarmAlarmSnooze;
                a.FarmAlarmCountInCycle = config.system.AlarmLevelTwoFarmAlarmCountInCycle;

                a.SmsEnable = config.system.AlarmLevelTwoSmsEnable;
                a.SmsRaiseTimeOffset = config.system.AlarmLevelTwoSmsRaiseTimeOffset;
                a.SmsEvery = config.system.AlarmLevelTwoSmsEvery;
                a.SmsSnooze = config.system.AlarmLevelTwoSmsSnooze;
                a.SmsCountInCycle = config.system.AlarmLevelTwoSmsCountInCycle;

                a.PoultryLightAlarmEnable = config.system.AlarmLevelTwoPoultryLightAlarmEnable;
                a.PoultryLightAlarmRaiseTimeOffset = config.system.AlarmLevelTwoPoultryLightAlarmRaiseTimeOffset;
                a.PoultryLightAlarmEvery = config.system.AlarmLevelTwoPoultryLightAlarmEvery;
                a.PoultryLightAlarmSnooze = config.system.AlarmLevelTwoPoultryLightAlarmSnooze;
                a.PoultryLightAlarmCountInCycle = config.system.AlarmLevelTwoPoultryLightAlarmCountInCycle;

                a.PoultrySirenAlarmEnable = config.system.AlarmLevelTwoPoultryLightAlarmEnable;
                a.PoultrySirenAlarmRaiseTimeOffset = config.system.AlarmLevelTwoPoultryLightAlarmRaiseTimeOffset;
                a.PoultrySirenAlarmEvery = config.system.AlarmLevelTwoPoultryLightAlarmEvery;
                a.PoultrySirenAlarmSnooze = config.system.AlarmLevelTwoPoultryLightAlarmSnooze;
                a.PoultrySirenAlarmCountInCycle = config.system.AlarmLevelTwoPoultryLightAlarmCountInCycle;
                break;
            case 3:
                a.Enable = config.system.AlarmLevelThreeEnable;
                a.RaiseTime = config.system.AlarmLevelThreeRaiseTime;
                a.FarmAlarmEnable = config.system.AlarmLevelThreeFarmAlarmEnable;
                a.FarmAlarmRaiseTimeOffset = config.system.AlarmLevelThreeFarmAlarmRaiseTimeOffset;
                a.FarmAlarmEvery = config.system.AlarmLevelThreeFarmAlarmEvery;
                a.FarmAlarmSnooze = config.system.AlarmLevelThreeFarmAlarmSnooze;
                a.FarmAlarmCountInCycle = config.system.AlarmLevelThreeFarmAlarmCountInCycle;

                a.SmsEnable = config.system.AlarmLevelThreeSmsEnable;
                a.SmsRaiseTimeOffset = config.system.AlarmLevelThreeSmsRaiseTimeOffset;
                a.SmsEvery = config.system.AlarmLevelThreeSmsEvery;
                a.SmsSnooze = config.system.AlarmLevelThreeSmsSnooze;
                a.SmsCountInCycle = config.system.AlarmLevelThreeSmsCountInCycle;

                a.PoultryLightAlarmEnable = config.system.AlarmLevelThreePoultryLightAlarmEnable;
                a.PoultryLightAlarmRaiseTimeOffset = config.system.AlarmLevelThreePoultryLightAlarmRaiseTimeOffset;
                a.PoultryLightAlarmEvery = config.system.AlarmLevelThreePoultryLightAlarmEvery;
                a.PoultryLightAlarmSnooze = config.system.AlarmLevelThreePoultryLightAlarmSnooze;
                a.PoultryLightAlarmCountInCycle = config.system.AlarmLevelThreePoultryLightAlarmCountInCycle;

                a.PoultrySirenAlarmEnable = config.system.AlarmLevelThreePoultryLightAlarmEnable;
                a.PoultrySirenAlarmRaiseTimeOffset = config.system.AlarmLevelThreePoultryLightAlarmRaiseTimeOffset;
                a.PoultrySirenAlarmEvery = config.system.AlarmLevelThreePoultryLightAlarmEvery;
                a.PoultrySirenAlarmSnooze = config.system.AlarmLevelThreePoultryLightAlarmSnooze;
                a.PoultrySirenAlarmCountInCycle = config.system.AlarmLevelThreePoultryLightAlarmCountInCycle;
                break;
            case 4:
                a.Enable = config.system.AlarmLevelFourEnable;
                a.RaiseTime = config.system.AlarmLevelFourRaiseTime;
                a.FarmAlarmEnable = config.system.AlarmLevelFourFarmAlarmEnable;
                a.FarmAlarmRaiseTimeOffset = config.system.AlarmLevelFourFarmAlarmRaiseTimeOffset;
                a.FarmAlarmEvery = config.system.AlarmLevelFourFarmAlarmEvery;
                a.FarmAlarmSnooze = config.system.AlarmLevelFourFarmAlarmSnooze;
                a.FarmAlarmCountInCycle = config.system.AlarmLevelFourFarmAlarmCountInCycle;

                a.SmsEnable = config.system.AlarmLevelFourSmsEnable;
                a.SmsRaiseTimeOffset = config.system.AlarmLevelFourSmsRaiseTimeOffset;
                a.SmsEvery = config.system.AlarmLevelFourSmsEvery;
                a.SmsSnooze = config.system.AlarmLevelFourSmsSnooze;
                a.SmsCountInCycle = config.system.AlarmLevelFourSmsCountInCycle;

                a.PoultryLightAlarmEnable = config.system.AlarmLevelFourPoultryLightAlarmEnable;
                a.PoultryLightAlarmRaiseTimeOffset = config.system.AlarmLevelFourPoultryLightAlarmRaiseTimeOffset;
                a.PoultryLightAlarmEvery = config.system.AlarmLevelFourPoultryLightAlarmEvery;
                a.PoultryLightAlarmSnooze = config.system.AlarmLevelFourPoultryLightAlarmSnooze;
                a.PoultryLightAlarmCountInCycle = config.system.AlarmLevelFourPoultryLightAlarmCountInCycle;

                a.PoultrySirenAlarmEnable = config.system.AlarmLevelFourPoultryLightAlarmEnable;
                a.PoultrySirenAlarmRaiseTimeOffset = config.system.AlarmLevelFourPoultryLightAlarmRaiseTimeOffset;
                a.PoultrySirenAlarmEvery = config.system.AlarmLevelFourPoultryLightAlarmEvery;
                a.PoultrySirenAlarmSnooze = config.system.AlarmLevelFourPoultryLightAlarmSnooze;
                a.PoultrySirenAlarmCountInCycle = config.system.AlarmLevelFourPoultryLightAlarmCountInCycle;
                break;
        }
        return a;
    }

    private FarmModel? FindFarmBySensorId(int Id)
    {
        return (from f in Poultry.Farms where Poultry.Farms.Select(pf => pf.Scalars).SelectMany(sc => sc.ActiveSensors).Any(As => As.Id == Id) select f).FirstOrDefault();
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
            var alarmTimes = GetAlarmTimings(e.ErrorType);
            ProcessAlarms(alarmTimes, e, Now);
        }

        foreach (var e in AlarmableFarmPeriodErrors.ToList())
        {
            if (e.DateErased.HasValue)
            {
                AlarmableFarmPeriodErrors.Remove(e);
                //Disable light/siren alarm if active
                continue;
            }
            var alarmTimes = GetAlarmTimings(e.ErrorType);
            ProcessAlarms(alarmTimes, e, Now);
        }

        foreach (var e in AlarmablePoultryPeriodErrors.ToList())
        {
            if (e.DateErased.HasValue)
            {
                AlarmablePoultryPeriodErrors.Remove(e);
                //Disable light/siren alarm if active
                continue;
            }
            var alarmTimes = GetAlarmTimings(e.ErrorType);
            ProcessAlarms(alarmTimes, e, Now);
        }
    }

    private void ProcessAlarms(AlarmTimesModel alarmTimes, SensorErrorModel e, DateTime Now)
    {
        if (alarmTimes.Enable)
        {
            if (alarmTimes.FarmAlarmEnable)
            {
                if (e.DateFarmAlarmRaised == null && e.DateHappened.IsElapsed(alarmTimes.FarmAlarmRaiseTime)) //first alarm.
                {
                    e.FarmAlarmRaisedCount = 1;
                    Log.Information($"Informing Alarm. Count: {e.FarmAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.FarmLight, e.LocationId));
                    e.DateFarmAlarmRaised = Now;
                }
                else if (e.FarmAlarmRaisedCount % alarmTimes.FarmAlarmCountInCycle != 0 && e.DateFarmAlarmRaised.IsElapsed(alarmTimes.FarmAlarmEvery)) //alarm every.
                {
                    e.FarmAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.FarmAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.FarmLight, e.LocationId));
                    e.DateFarmAlarmRaised = Now;
                }
                else if (e.FarmAlarmRaisedCount % alarmTimes.FarmAlarmCountInCycle == 0 && e.DateFarmAlarmRaised.IsElapsed(alarmTimes.FarmAlarmSnooze)) //alarm sleep.
                {
                    e.FarmAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.FarmAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.FarmLight, e.LocationId));
                    e.DateFarmAlarmRaised = Now;
                }
            }
            if (alarmTimes.SmsEnable)
            {
                if (e.DateSmsRaised == null && e.DateHappened.IsElapsed(alarmTimes.SmsRaiseTime)) //first alarm.
                {
                    e.SmsRaisedCount = 1;
                    Log.Information($"Informing Alarm. Count: {e.SmsRaisedCount}");
                    //send sms, save to db
                    e.DateSmsRaised = Now;
                }
                else if (e.SmsRaisedCount % alarmTimes.SmsCountInCycle != 0 && e.DateSmsRaised.IsElapsed(alarmTimes.SmsEvery)) //alarm every.
                {
                    e.SmsRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.SmsRaisedCount}");
                    //send sms, save to db
                    e.DateSmsRaised = Now;
                }
                else if (e.SmsRaisedCount % alarmTimes.SmsCountInCycle == 0 && e.DateSmsRaised.IsElapsed(alarmTimes.SmsSnooze)) //alarm sleep.
                {
                    e.SmsRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.SmsRaisedCount}");
                    //send sms, save to db
                    e.DateSmsRaised = Now;
                }
            }
            if (alarmTimes.PoultryLightAlarmEnable)
            {
                if (e.DatePoultryAlarmRaised == null && e.DateHappened.IsElapsed(alarmTimes.PoultryLightAlarmRaiseTime)) //first alarm.
                {
                    e.PoultryAlarmRaisedCount = 1;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultryLight));
                    e.DatePoultryAlarmRaised = Now;
                }
                else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultryLightAlarmCountInCycle != 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultryLightAlarmEvery)) //alarm every.
                {
                    e.PoultryAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultryLight));
                    e.DatePoultryAlarmRaised = Now;
                }
                else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultryLightAlarmCountInCycle == 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultryLightAlarmSnooze)) //alarm sleep.
                {
                    e.PoultryAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultryLight));
                    e.DatePoultryAlarmRaised = Now;
                }
            }
            if (alarmTimes.PoultrySirenAlarmEnable)
            {
                if (e.DatePoultryAlarmRaised == null && e.DateHappened.IsElapsed(alarmTimes.PoultrySirenAlarmRaiseTime)) //first alarm.
                {
                    e.PoultryAlarmRaisedCount = 1;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultrySiren));
                    e.DatePoultryAlarmRaised = Now;
                }
                else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultrySirenAlarmCountInCycle != 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultrySirenAlarmEvery)) //alarm every.
                {
                    e.PoultryAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultrySiren));
                    e.DatePoultryAlarmRaised = Now;
                }
                else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultrySirenAlarmCountInCycle == 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultrySirenAlarmSnooze)) //alarm sleep.
                {
                    e.PoultryAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultrySiren));
                    e.DatePoultryAlarmRaised = Now;
                }
            }
        }
    }

    private void ProcessAlarms(AlarmTimesModel alarmTimes, FarmInPeriodErrorModel e, DateTime Now)
    {
        if (alarmTimes.Enable)
        {
            if (alarmTimes.FarmAlarmEnable)
            {
                if (e.DateFarmAlarmRaised == null && e.DateHappened.IsElapsed(alarmTimes.FarmAlarmRaiseTime)) //first alarm.
                {
                    e.FarmAlarmRaisedCount = 1;
                    Log.Information($"Informing Alarm. Count: {e.FarmAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e,AlarmDeviceType.FarmLight, e.FarmId));
                    e.DateFarmAlarmRaised = Now;
                }
                else if (e.FarmAlarmRaisedCount % alarmTimes.FarmAlarmCountInCycle != 0 && e.DateFarmAlarmRaised.IsElapsed(alarmTimes.FarmAlarmEvery)) //alarm every.
                {
                    e.FarmAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.FarmAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.FarmLight, e.FarmId));
                    e.DateFarmAlarmRaised = Now;
                }
                else if (e.FarmAlarmRaisedCount % alarmTimes.FarmAlarmCountInCycle == 0 && e.DateFarmAlarmRaised.IsElapsed(alarmTimes.FarmAlarmSnooze)) //alarm sleep.
                {
                    e.FarmAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.FarmAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.FarmLight, e.FarmId));
                    e.DateFarmAlarmRaised = Now;
                }
            }
            if (alarmTimes.SmsEnable)
            {
                if (e.DateSmsRaised == null && e.DateHappened.IsElapsed(alarmTimes.SmsRaiseTime)) //first alarm.
                {
                    e.SmsRaisedCount = 1;
                    Log.Information($"Informing Alarm. Count: {e.SmsRaisedCount}");
                    //send sms, save to db
                    e.DateSmsRaised = Now;
                }
                else if (e.SmsRaisedCount % alarmTimes.SmsCountInCycle != 0 && e.DateSmsRaised.IsElapsed(alarmTimes.SmsEvery)) //alarm every.
                {
                    e.SmsRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.SmsRaisedCount}");
                    //send sms, save to db
                    e.DateSmsRaised = Now;
                }
                else if (e.SmsRaisedCount % alarmTimes.SmsCountInCycle == 0 && e.DateSmsRaised.IsElapsed(alarmTimes.SmsSnooze)) //alarm sleep.
                {
                    e.SmsRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.SmsRaisedCount}");
                    //send sms, save to db
                    e.DateSmsRaised = Now;
                }
            }
            if (alarmTimes.PoultryLightAlarmEnable)
            {
                if (e.DatePoultryAlarmRaised == null && e.DateHappened.IsElapsed(alarmTimes.PoultryLightAlarmRaiseTime)) //first alarm.
                {
                    e.PoultryAlarmRaisedCount = 1;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultryLight));
                    e.DatePoultryAlarmRaised = Now;
                }
                else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultryLightAlarmCountInCycle != 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultryLightAlarmEvery)) //alarm every.
                {
                    e.PoultryAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultryLight));
                    e.DatePoultryAlarmRaised = Now;
                }
                else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultryLightAlarmCountInCycle == 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultryLightAlarmSnooze)) //alarm sleep.
                {
                    e.PoultryAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultryLight));
                    e.DatePoultryAlarmRaised = Now;
                }
            }
            if (alarmTimes.PoultrySirenAlarmEnable)
            {
                if (e.DatePoultryAlarmRaised == null && e.DateHappened.IsElapsed(alarmTimes.PoultrySirenAlarmRaiseTime)) //first alarm.
                {
                    e.PoultryAlarmRaisedCount = 1;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultrySiren));
                    e.DatePoultryAlarmRaised = Now;
                }
                else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultrySirenAlarmCountInCycle != 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultrySirenAlarmEvery)) //alarm every.
                {
                    e.PoultryAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultrySiren));
                    e.DatePoultryAlarmRaised = Now;
                }
                else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultrySirenAlarmCountInCycle == 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultrySirenAlarmSnooze)) //alarm sleep.
                {
                    e.PoultryAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultrySiren));
                    e.DatePoultryAlarmRaised = Now;
                }
            }
        }
    }

    private void ProcessAlarms(AlarmTimesModel alarmTimes, PoultryInPeriodErrorModel e, DateTime Now)
    {
        if (alarmTimes.Enable)
        {
            //if (alarmTimes.FarmAlarmEnable)
            //{
            //    if (e.DateFarmAlarmRaised == null && e.DateHappened.IsElapsed(alarmTimes.FarmAlarmRaiseTime)) //first alarm.
            //    {
            //        e.FarmAlarmRaisedCount = 1;
            //        Log.Information($"Informing Alarm. Count: {e.FarmAlarmRaisedCount}");
            //        //save to db
            //        Task.Run(() => TriggerAlarm(e, AlarmDeviceType.FarmLight));
            //        e.DateFarmAlarmRaised = Now;
            //    }
            //    else if (e.FarmAlarmRaisedCount % alarmTimes.FarmAlarmCountInCycle != 0 && e.DateFarmAlarmRaised.IsElapsed(alarmTimes.FarmAlarmEvery)) //alarm every.
            //    {
            //        e.FarmAlarmRaisedCount++;
            //        Log.Information($"Informing Alarm. Count: {e.FarmAlarmRaisedCount}");
            //        //save to db
            //        Task.Run(() => TriggerAlarm(e, AlarmDeviceType.FarmLight));
            //        e.DateFarmAlarmRaised = Now;
            //    }
            //    else if (e.FarmAlarmRaisedCount % alarmTimes.FarmAlarmCountInCycle == 0 && e.DateFarmAlarmRaised.IsElapsed(alarmTimes.FarmAlarmSnooze)) //alarm sleep.
            //    {
            //        e.FarmAlarmRaisedCount++;
            //        Log.Information($"Informing Alarm. Count: {e.FarmAlarmRaisedCount}");
            //        //save to db
            //        Task.Run(() => TriggerAlarm(e, AlarmDeviceType.FarmLight));
            //        e.DateFarmAlarmRaised = Now;
            //    }
            //}
            if (alarmTimes.SmsEnable)
            {
                if (e.DateSmsRaised == null && e.DateHappened.IsElapsed(alarmTimes.SmsRaiseTime)) //first alarm.
                {
                    e.SmsRaisedCount = 1;
                    Log.Information($"Informing Alarm. Count: {e.SmsRaisedCount}");
                    //send sms, save to db
                    e.DateSmsRaised = Now;
                }
                else if (e.SmsRaisedCount % alarmTimes.SmsCountInCycle != 0 && e.DateSmsRaised.IsElapsed(alarmTimes.SmsEvery)) //alarm every.
                {
                    e.SmsRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.SmsRaisedCount}");
                    //send sms, save to db
                    e.DateSmsRaised = Now;
                }
                else if (e.SmsRaisedCount % alarmTimes.SmsCountInCycle == 0 && e.DateSmsRaised.IsElapsed(alarmTimes.SmsSnooze)) //alarm sleep.
                {
                    e.SmsRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.SmsRaisedCount}");
                    //send sms, save to db
                    e.DateSmsRaised = Now;
                }
            }
            if (alarmTimes.PoultryLightAlarmEnable)
            {
                if (e.DatePoultryAlarmRaised == null && e.DateHappened.IsElapsed(alarmTimes.PoultryLightAlarmRaiseTime)) //first alarm.
                {
                    e.PoultryAlarmRaisedCount = 1;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultryLight));
                    e.DatePoultryAlarmRaised = Now;
                }
                else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultryLightAlarmCountInCycle != 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultryLightAlarmEvery)) //alarm every.
                {
                    e.PoultryAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultryLight));
                    e.DatePoultryAlarmRaised = Now;
                }
                else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultryLightAlarmCountInCycle == 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultryLightAlarmSnooze)) //alarm sleep.
                {
                    e.PoultryAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultryLight));
                    e.DatePoultryAlarmRaised = Now;
                }
            }
            if (alarmTimes.PoultrySirenAlarmEnable)
            {
                if (e.DatePoultryAlarmRaised == null && e.DateHappened.IsElapsed(alarmTimes.PoultrySirenAlarmRaiseTime)) //first alarm.
                {
                    e.PoultryAlarmRaisedCount = 1;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultrySiren));
                    e.DatePoultryAlarmRaised = Now;
                }
                else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultrySirenAlarmCountInCycle != 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultrySirenAlarmEvery)) //alarm every.
                {
                    e.PoultryAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultrySiren));
                    e.DatePoultryAlarmRaised = Now;
                }
                else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultrySirenAlarmCountInCycle == 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultrySirenAlarmSnooze)) //alarm sleep.
                {
                    e.PoultryAlarmRaisedCount++;
                    Log.Information($"Informing Alarm. Count: {e.PoultryAlarmRaisedCount}");
                    //save to db
                    Task.Run(() => TriggerAlarm(e, AlarmDeviceType.PoultrySiren));
                    e.DatePoultryAlarmRaised = Now;
                }
            }
        }
    }

    private void TriggerAlarm(ErrorModel e, AlarmDeviceType type, int AlarmLocationId = 0)
    {
        var alarms = AlarmDevices?.Where(a => a.DeviceType == type && a.LocationId == AlarmLocationId);
        if (alarms != null)
        {
            if (config.VerboseMode) Log.Information($"Activating alarm. Alarm type: {type}, Alarm location ID: {AlarmLocationId}, Error: {e.Descriptions}");
            var start = DateTime.Now;
            while (e.DateErased == null && !start.IsElapsed(AlarmLocationId == 0 ? config.system.PoultryAlarmDuration : config.system.FarmAlarmDuration))
            {
                foreach (var alarm in alarms)
                {
                    if (!alarm.IsActive)
                    {
                        if (type != AlarmDeviceType.PoultrySiren || (type == AlarmDeviceType.PoultrySiren && !alarm.IsSnoozed) || (type == AlarmDeviceType.PoultrySiren && alarm.IsSnoozed && alarm.SnoozedTime.IsElapsed(config.system.SirenSnoozeDuration)))
                        {
                            if (config.VerboseMode) Log.Information($"Reactivating alarm. Alarm type: {type}, Alarm location ID: {AlarmLocationId}, Error: {e.Descriptions}");
                            //Send active mqtt to alarm.
                            alarm.IsSnoozed = false;
                            alarm.IsActive = true;
                        }
                    }
                }
            }
            if (config.VerboseMode) Log.Information($"Deactivating alarm. Alarm type: {type}, Alarm location ID: {AlarmLocationId}, Error: {e.Descriptions}");
            foreach (var alarm in alarms)
            {
                //Send deactive mqtt to alarm.
                alarm.IsActive = false;
            }
        }
        else Log.Warning($"Alarm needs to be activated but no alarm device is found for the error. Alarm type: {type}, Alarm location ID: {AlarmLocationId}");
    }

    private void SnoozeSirens()
    {
        var alarms = AlarmDevices?.Where(a => a.DeviceType == AlarmDeviceType.PoultrySiren);
        if (alarms != null && alarms.Any())
            foreach (var alarm in alarms)
            {
                alarm.IsActive = false;
                alarm.IsSnoozed = true;
                alarm.SnoozedTime = DateTime.Now;
                //Send deactive mqtt to alarm.
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
                //if (config.VerboseMode) Log.Information($"===============  Start observation process ===============");
                try
                {
                    if (Poultry != null && (Poultry.IsInPeriod || config.system.ObserveAlways))
                    {
                        var result = await ObservePoultryAsync();
                        if (!string.IsNullOrEmpty(result))
                            Log.Error($"Observation process returned with error: {result}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception thrown in Observer:");
                }
                //if (config.VerboseMode) Log.Information($"===============  End of observation process ({TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds} ms) ===============");
                await Task.Delay(TimeSpan.FromSeconds(config.system.ObserverCheckInterval));
            }
        }
    }
}