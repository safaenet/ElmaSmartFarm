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
        var ScalarSets = Poultries.SelectMany(p => p.Farms.Select(f => f.Scalars));
        if (ScalarSets != null)
            foreach (var set in ScalarSets)
            {
                if (set.HasSensors)
                {
                    foreach (var sensor in set.EnabledSensors)
                    {
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
                        if (!sensor.IsWatched && (sensor.WatchTemperature || sensor.WatchHumidity || sensor.WatchLight || sensor.WatchAmmonia || sensor.WatchCo2)) sensor.IsWatched = CheckToReWatchSensor(sensor, startDate);

                        //Remove expired reads
                    }
                }
            }
        ScalarSets = null;
        //var m = new MqttApplicationMessageBuilder().WithTopic("safa").WithPayload("dana").Build();
        //if (mqttClient.IsConnected) await mqttClient.PublishAsync(m);
        return null;
    }

    private bool CheckToReWatchSensor<T>(T sensor, DateTime? PeriodStartDate) where T : SensorModel
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

    private void CheckForInform<T>(T sensor, bool WatchUnit, SensorErrorModel e, DateTime Now) where T : SensorModel
    {
        if (WatchUnit && e.DateInformed == null && e.DateHappened.IsElapsed(5)) //first alarm.
        {
            e.InformCount = 1;
            Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.InformCount}");
            //inform, save to db
            e.DateInformed = Now;
        }
        else if (WatchUnit && e.InformCount % 3 != 0 && e.DateInformed.IsElapsed(10)) //alarm every.
        {
            e.InformCount++;
            Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.InformCount}");
            //inform, save to db
            e.DateInformed = Now;
        }
        else if (WatchUnit && e.InformCount % 3 == 0 && e.DateInformed.IsElapsed(200)) //alarm sleep.
        {
            e.InformCount++;
            Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.InformCount}");
            //inform, save to db
            e.DateInformed = Now;
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