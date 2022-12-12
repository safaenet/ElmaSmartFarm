using ElmaSmartFarm.SharedLibrary;
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
                        await CheckKeepAliveMessageDate(sensor, config.system.KeepAliveWaitingTimeout, Now);
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                if (e.ErrorType == SensorErrorType.InvalidTemperatureData)
                                {
                                    if (config.system.AlarmTempInvalidDataEnable && (sensor.WatchTemperature || e.AlarmInformCount < config.system.AlarmTempInvalidDataCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmTempInvalidDataFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.WatchTemperature = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchTemperature, sensor.IsInPeriod, config.system.TempInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidTemperatureValue)
                                {
                                    if (config.system.AlarmTempInvalidValueEnable && (sensor.WatchTemperature || e.AlarmInformCount < config.system.AlarmTempInvalidValueCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmTempInvalidValueFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.WatchTemperature = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchTemperature, sensor.IsInPeriod, config.system.TempInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidHumidityData)
                                {
                                    if (config.system.AlarmHumidInvalidDataEnable && (sensor.WatchHumidity || e.AlarmInformCount < config.system.AlarmHumidInvalidDataCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmHumidInvalidDataFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.WatchHumidity = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchHumidity, sensor.IsInPeriod, config.system.HumidInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidHumidityValue)
                                {
                                    if (config.system.AlarmHumidInvalidValueEnable && (sensor.WatchHumidity || e.AlarmInformCount < config.system.AlarmHumidInvalidValueCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmHumidInvalidValueFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.WatchHumidity = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchHumidity, sensor.IsInPeriod, config.system.HumidInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidLightData)
                                {
                                    if (config.system.AlarmAmbientLightInvalidDataEnable && (sensor.WatchLight || e.AlarmInformCount < config.system.AlarmAmbientLightInvalidDataCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmAmbientLightInvalidDataFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.WatchLight = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchLight, sensor.IsInPeriod, config.system.AmbientLightInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidLightValue)
                                {
                                    if (config.system.AlarmAmbientLightInvalidValueEnable && (sensor.WatchLight || e.AlarmInformCount < config.system.AlarmAmbientLightInvalidValueCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmAmbientLightInvalidValueFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.WatchLight = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchLight, sensor.IsInPeriod, config.system.AmbientLightInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidAmmoniaData)
                                {
                                    if (config.system.AlarmAmmoniaInvalidDataEnable && (sensor.WatchAmmonia || e.AlarmInformCount < config.system.AlarmAmmoniaInvalidDataCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmAmmoniaInvalidDataFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.WatchAmmonia = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchAmmonia, sensor.IsInPeriod, config.system.AmmoniaInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidAmmoniaValue)
                                {
                                    if (config.system.AlarmAmmoniaInvalidValueEnable && (sensor.WatchAmmonia || e.AlarmInformCount < config.system.AlarmAmmoniaInvalidValueCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmAmmoniaInvalidValueFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.WatchAmmonia = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchAmmonia, sensor.IsInPeriod, config.system.AmmoniaInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidCo2Data)
                                {
                                    if (config.system.AlarmCo2InvalidDataEnable && (sensor.WatchCo2 || e.AlarmInformCount < config.system.AlarmCo2InvalidDataCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmCo2InvalidDataFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.WatchCo2 = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchCo2, sensor.IsInPeriod, config.system.Co2InvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidCo2Value)
                                {
                                    if (config.system.AlarmCo2InvalidValueEnable && (sensor.WatchCo2 || e.AlarmInformCount < config.system.AlarmCo2InvalidValueCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmCo2InvalidValueFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.WatchCo2 = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchCo2, sensor.IsInPeriod, config.system.Co2InvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    if (config.system.AlarmScalarNotAliveEnable && (sensor.IsWatched || e.AlarmInformCount < config.system.AlarmScalarNotAliveCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmScalarNotAliveFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.ScalarNotAliveWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (config.system.AlarmScalarLowBatteryEnable && (sensor.IsWatched || e.AlarmInformCount < config.system.AlarmScalarLowBatteryCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmScalarLowBatteryFirstTime)) AlarmableSensorErrors.Add(e);
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
                        await CheckKeepAliveMessageDate(sensor, config.system.KeepAliveWaitingTimeout, Now);
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                if (e.ErrorType == SensorErrorType.InvalidData)
                                {
                                    if (config.system.AlarmCommuteInvalidDataEnable && (sensor.IsWatched || e.AlarmInformCount < config.system.AlarmCommuteInvalidDataCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmCommuteInvalidDataFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidValue)
                                {
                                    if (config.system.AlarmCommuteInvalidValueEnable && (sensor.IsWatched || e.AlarmInformCount < config.system.AlarmCommuteInvalidValueCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmCommuteInvalidValueFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    if (config.system.AlarmCommuteNotAliveEnable && (sensor.IsWatched || e.AlarmInformCount < config.system.AlarmCommuteNotAliveCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmCommuteNotAliveFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteNotAliveWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (config.system.AlarmCommuteLowBatteryEnable && (sensor.IsWatched || e.AlarmInformCount < config.system.AlarmCommuteLowBatteryCountInCycle) && e.DateHappened.IsElapsed(config.system.AlarmCommuteLowBatteryFirstTime)) AlarmableSensorErrors.Add(e);
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
                        await CheckKeepAliveMessageDate(sensor, config.system.KeepAliveWaitingTimeout, Now);
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                if (sensor.IsWatched && (e.ErrorType == SensorErrorType.NotAlive))
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
                                    if (AlarmPushButtonNotAliveEnable && (sensor.IsWatched || e.AlarmInformCount < AlarmPushButtonNotAliveCountInCycle) && e.DateHappened.IsElapsed(AlarmPushButtonNotAliveFirstTime)) AlarmableSensorErrors.Add(e);
                                    sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, PushButtonNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && (e.ErrorType == SensorErrorType.LowBattery))
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
                                    if (AlarmPushButtonLowBatteryEnable && (sensor.IsWatched || e.AlarmInformCount < AlarmPushButtonLowBatteryCountInCycle) && e.DateHappened.IsElapsed(AlarmPushButtonLowBatteryFirstTime)) AlarmableSensorErrors.Add(e);
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
                        await CheckKeepAliveMessageDate(sensor, config.system.KeepAliveWaitingTimeout, Now);
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                if (e.ErrorType == SensorErrorType.InvalidData)
                                {
                                    CheckForInform(sensor, sensor.IsWatched, e, Now);
                                    sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, 5);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidValue)
                                {
                                    CheckForInform(sensor, sensor.IsWatched, e, Now);
                                    sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, 5);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    CheckForInform(sensor, sensor.IsWatched, e, Now);
                                    sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, 5);
                                }
                                else if (e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    CheckForInform(sensor, sensor.IsWatched, e, Now);
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
        BinarySets = null;
        #endregion
        //var m = new MqttApplicationMessageBuilder().WithTopic("safa").WithPayload("dana").Build();
        //if (mqttClient.IsConnected) await mqttClient.PublishAsync(m);
        return null;
    }

    private void AddSensorErrorToAlarmable(SensorErrorModel e)
    {
        if (!AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
    }

    private async Task CheckKeepAliveMessageDate(SensorModel sensor, int keepAliveTimeout, DateTime Now)
    {

        if (sensor.IsWatched && config.system.IsKeepAliveEnabled && sensor.KeepAliveMessageDate.IsElapsed(keepAliveTimeout))
        {
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

    private void CheckForInform(SensorModel sensor, bool WatchUnit, SensorErrorModel e, int firstInformTime, int everyInformTime, int snoozeTime, int informCycleCount, DateTime Now)
    {
        if (WatchUnit || e.AlarmInformCount < informCycleCount)
        {
            if (e.DateAlarmInformed == null && e.DateHappened.IsElapsed(firstInformTime)) //first alarm.
            {
                e.AlarmInformCount = 1;
                Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.AlarmInformCount}");
                //inform, save to db
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