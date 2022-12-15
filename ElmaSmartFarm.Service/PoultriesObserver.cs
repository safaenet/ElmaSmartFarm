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
                                var alarmTimes = GetAlarmTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.InvalidTemperatureData)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchTemperature || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if(sensor.WatchTemperature) sensor.WatchTemperature = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchTemperature, sensor.IsInPeriod, config.system.TempInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidTemperatureValue)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchTemperature || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchTemperature) sensor.WatchTemperature = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchTemperature, sensor.IsInPeriod, config.system.TempInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidHumidityData)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchHumidity || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchHumidity) sensor.WatchHumidity = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchHumidity, sensor.IsInPeriod, config.system.HumidInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidHumidityValue)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchHumidity || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchHumidity) sensor.WatchHumidity = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchHumidity, sensor.IsInPeriod, config.system.HumidInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidLightData)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchLight || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchLight) sensor.WatchLight = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchLight, sensor.IsInPeriod, config.system.AmbientLightInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidLightValue)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchLight || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchLight) sensor.WatchLight = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchLight, sensor.IsInPeriod, config.system.AmbientLightInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidAmmoniaData)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchAmmonia || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchAmmonia) sensor.WatchAmmonia = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchAmmonia, sensor.IsInPeriod, config.system.AmmoniaInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidAmmoniaValue)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchAmmonia || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchAmmonia) sensor.WatchAmmonia = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchAmmonia, sensor.IsInPeriod, config.system.AmmoniaInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidCo2Data)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchCo2 || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchCo2) sensor.WatchCo2 = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchCo2, sensor.IsInPeriod, config.system.Co2InvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidCo2Value)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchCo2 || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchCo2) sensor.WatchCo2 = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchCo2, sensor.IsInPeriod, config.system.Co2InvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.ScalarNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
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
                                var alarmTimes = GetAlarmTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.InvalidData)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidValue)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
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
                                var alarmTimes = GetAlarmTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    int PushButtonNotAliveWatchTimeout = 0;
                                    if (sensor.Type == SensorType.FarmFeed) PushButtonNotAliveWatchTimeout = config.system.FeedNotAliveWatchTimeout;
                                    else if (sensor.Type == SensorType.FarmCheckup) PushButtonNotAliveWatchTimeout = config.system.CheckupNotAliveWatchTimeout;
                                    if (alarmTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, PushButtonNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
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
                                    if (alarmTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
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
                                    if (alarmTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, BinaryNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched || e.AlarmInformCount < alarmTimes.CountInCycle) && e.DateHappened.IsElapsed(alarmTimes.FirstTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
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

    private AlarmTimesModel GetAlarmTimings(SensorErrorType e)
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
            if (sensor == null)
            {
                Log.Error($"No sensor found related to the Error. Sensor ID: {e.SensorId}, Error Type: {e.ErrorType}, Date Happened: {e.DateHappened}");
                continue;
            }
            var alarmTimes = GetAlarmTimings(e.ErrorType);
            if (alarmTimes.Enable)
            {
                if (e.DateAlarmInformed == null && e.DateHappened.IsElapsed(alarmTimes.FirstTime)) //first alarm.
                {
                    e.AlarmInformCount = 1;
                    Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.AlarmInformCount}");
                    if (sensor.IsFarmSensor())
                    {

                    }
                    e.DateAlarmInformed = Now;
                }
                else if (e.AlarmInformCount % alarmTimes.CountInCycle != 0 && e.DateAlarmInformed.IsElapsed(alarmTimes.Every)) //alarm every.
                {
                    e.AlarmInformCount++;
                    Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.AlarmInformCount}");
                    //inform, save to db
                    e.DateAlarmInformed = Now;
                }
                else if (e.AlarmInformCount % alarmTimes.CountInCycle == 0 && e.DateAlarmInformed.IsElapsed(alarmTimes.Snooze)) //alarm sleep.
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