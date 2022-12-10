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
                                if (sensor.WatchTemperature && (e.ErrorType == SensorErrorType.InvalidTemperatureData || e.ErrorType == SensorErrorType.InvalidTemperatureValue))
                                {
                                    CheckForInform(sensor, sensor.WatchTemperature, e, Now);
                                    sensor.WatchTemperature = !CheckToPutOutOfWatch(e, sensor.IsWatched, sensor.WatchTemperature, sensor.IsInPeriod, 5);
                                }
                                else if (sensor.WatchHumidity && (e.ErrorType == SensorErrorType.InvalidHumidityData || e.ErrorType == SensorErrorType.InvalidHumidityValue))
                                {
                                    CheckForInform(sensor, sensor.WatchHumidity, e, Now);
                                    sensor.WatchHumidity = !CheckToPutOutOfWatch(e, sensor.IsWatched, sensor.WatchHumidity, sensor.IsInPeriod, 5);
                                }
                                else if (sensor.WatchLight && (e.ErrorType == SensorErrorType.InvalidLightData || e.ErrorType == SensorErrorType.InvalidLightValue))
                                {
                                    CheckForInform(sensor, sensor.WatchLight, e, Now);
                                    sensor.WatchLight = !CheckToPutOutOfWatch(e, sensor.IsWatched, sensor.WatchLight, sensor.IsInPeriod, 5);
                                }
                                else if (sensor.WatchAmmonia && (e.ErrorType == SensorErrorType.InvalidAmmoniaData || e.ErrorType == SensorErrorType.InvalidAmmoniaValue))
                                {
                                    CheckForInform(sensor, sensor.WatchAmmonia, e, Now);
                                    sensor.WatchAmmonia = !CheckToPutOutOfWatch(e, sensor.IsWatched, sensor.WatchAmmonia, sensor.IsInPeriod, 5);
                                }
                                else if (sensor.WatchCo2 && (e.ErrorType == SensorErrorType.InvalidCo2Data || e.ErrorType == SensorErrorType.InvalidCo2Value))
                                {
                                    CheckForInform(sensor, sensor.WatchCo2, e, Now);
                                    sensor.WatchCo2 = !CheckToPutOutOfWatch(e, sensor.IsWatched, sensor.WatchCo2, sensor.IsInPeriod, 5);
                                }
                                else if (sensor.IsWatched && (e.ErrorType == SensorErrorType.NotAlive))
                                {
                                    CheckForInform(sensor, sensor.IsWatched, e, Now);
                                    sensor.IsWatched = !CheckToPutOutOfWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, 5);
                                }
                                else if (sensor.IsWatched && (e.ErrorType == SensorErrorType.LowBattery))
                                {
                                    CheckForInform(sensor, sensor.IsWatched, e, Now);
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
                        if (sensor.WatchTemperature == false && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidTemperatureData || e.ErrorType == SensorErrorType.InvalidTemperatureValue) == false) //Temp sensor is healthy.
                            sensor.WatchTemperature = CheckToReWatchSensor(sensor, startDate);
                        if (sensor.WatchHumidity == false && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidHumidityData || e.ErrorType == SensorErrorType.InvalidHumidityValue) == false) //Humidity sensor is healthy.
                            sensor.WatchHumidity = CheckToReWatchSensor(sensor, startDate);
                        if (sensor.WatchLight == false && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidLightData || e.ErrorType == SensorErrorType.InvalidLightValue) == false) //Light sensor is healthy.
                            sensor.WatchLight = CheckToReWatchSensor(sensor, startDate);
                        if (sensor.WatchAmmonia == false && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidAmmoniaData || e.ErrorType == SensorErrorType.InvalidAmmoniaValue) == false) //Ammonia sensor is healthy.
                            sensor.WatchAmmonia = CheckToReWatchSensor(sensor, startDate);
                        if (sensor.WatchCo2 == false && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidCo2Data || e.ErrorType == SensorErrorType.InvalidCo2Value) == false) //Co2 sensor is healthy.
                            sensor.WatchCo2 = CheckToReWatchSensor(sensor, startDate);
                        if (sensor.IsWatched == false && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.NotAlive) == false && (sensor.WatchTemperature || sensor.WatchHumidity || sensor.WatchLight || sensor.WatchAmmonia || sensor.WatchCo2)) sensor.IsWatched = CheckToReWatchSensor(sensor, startDate); //Sensor is alive and healthy (again).

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
                                if (sensor.IsWatched && (e.ErrorType == SensorErrorType.InvalidData || e.ErrorType == SensorErrorType.InvalidValue))
                                {
                                    CheckForInform(sensor, sensor.IsWatched, e, Now);
                                    sensor.IsWatched = !CheckToPutOutOfWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, 5);
                                }
                                else if (sensor.IsWatched && (e.ErrorType == SensorErrorType.NotAlive))
                                {
                                    CheckForInform(sensor, sensor.IsWatched, e, Now);
                                    sensor.IsWatched = !CheckToPutOutOfWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, 5);
                                }
                                else if (sensor.IsWatched && (e.ErrorType == SensorErrorType.LowBattery))
                                {
                                    CheckForInform(sensor, sensor.IsWatched, e, Now);
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
                                    CheckForInform(sensor, sensor.IsWatched, e, Now);
                                    sensor.IsWatched = !CheckToPutOutOfWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, 5);
                                }
                                else if (sensor.IsWatched && (e.ErrorType == SensorErrorType.LowBattery))
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
                                if (sensor.IsWatched && (e.ErrorType == SensorErrorType.InvalidData || e.ErrorType == SensorErrorType.InvalidValue))
                                {
                                    CheckForInform(sensor, sensor.IsWatched, e, Now);
                                    sensor.IsWatched = !CheckToPutOutOfWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, 5);
                                }
                                else if (sensor.IsWatched && (e.ErrorType == SensorErrorType.NotAlive))
                                {
                                    CheckForInform(sensor, sensor.IsWatched, e, Now);
                                    sensor.IsWatched = !CheckToPutOutOfWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, 5);
                                }
                                else if (sensor.IsWatched && (e.ErrorType == SensorErrorType.LowBattery))
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

    private async Task CheckKeepAliveMessageDate(SensorModel sensor, int keepAliveTimeout, DateTime Now)
    {

        if (sensor.IsWatched && config.system.KeepAliveInterval > 0 && sensor.KeepAliveMessageDate.IsElapsed(keepAliveTimeout))
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

    private bool CheckToPutOutOfWatch(SensorErrorModel e, bool SensorWatch, bool UnitWatch, bool SensorIsInPeriod, int Elapse)
    {
        if (SensorWatch && UnitWatch && e.DateHappened.IsElapsed(Elapse) && (SensorIsInPeriod || config.system.ObserveAlways))
        {
            //inform, save to db
            Log.Warning($"Sensor with ID {e.SensorId} has been put out of watch due to persisting error. Error Type: {e.ErrorType}, Error happened date: {e.DateHappened}");
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