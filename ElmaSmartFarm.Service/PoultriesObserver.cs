using ElmaSmartFarm.SharedLibrary;
using MQTTnet;
using Serilog;

namespace ElmaSmartFarm.Service;

public partial class Worker
{
    private async Task<string?> ObservePoultriesAsync()
    {
        var IsInPeriod = Poultries.Any(p => p.IsInPeriod);
        var Now = DateTime.Now;
        var Sets = Poultries.SelectMany(p => p.Farms.Select(f => f.Scalars));
        if (Sets != null)
            foreach (var set in Sets)
            {
                if (set.HasSensors)
                {
                    foreach (var sensor in set.Sensors)
                    {
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                if (e.ErrorType == SensorErrorType.InvalidData)
                                {
                                    if (e.DateInformed == null && e.DateHappened.IsElapsed(100)) //first alarm.
                                    {
                                        e.InformCount = 1;
                                        Log.Information($"Informing Alarm of {SensorErrorType.InvalidValue}. Count: {e.InformCount}");
                                        //inform, save to db
                                        e.DateInformed = Now;
                                    }
                                    else if (e.InformCount < 3 && e.DateInformed.IsElapsed(10)) //alarm every.
                                    {
                                        e.InformCount++;
                                        Log.Information($"Informing Alarm of {SensorErrorType.InvalidValue}. Count: {e.InformCount}");
                                        //inform, save to db
                                        e.DateInformed = Now;
                                    }
                                    else if (e.InformCount == 3 && e.DateInformed.IsElapsed(20)) //alarm sleep.
                                    {
                                        e.InformCount = 1;
                                        Log.Information($"Informing Alarm of {SensorErrorType.InvalidValue}. Count: {e.InformCount}");
                                        //inform, save to db
                                        e.DateInformed = Now;
                                    }
                                    if (sensor.IsWatched && e.DateHappened.IsElapsed(100) && (sensor.IsInPeriod || config.system.ObserveAlways))
                                    {
                                        sensor.IsWatched = false;
                                        //inform, save to db
                                        Log.Warning($"Sensor with ID {sensor.Id} has been put out of watch due to persisting error. Error happened date: {e.DateHappened}");
                                    }
                                }
                            }
                        }
                        else if (sensor.IsWatched == false) // sensor is healthy.
                        {
                            if (sensor.IsInPeriod)
                            {
                                var startDate = Poultries.Where(p => p.IsInPeriod).SelectMany(p => p.Farms.Where(f => f.IsInPeriod && f.Scalars.Sensors.Any(s => s.Id == sensor.Id))).FirstOrDefault().Period.StartDate;
                                if (startDate.IsElapsed(sensor.WatchStartDay * 86400)) sensor.IsWatched = true;
                                //Inform the watch, save to db
                            }
                            else if (config.system.ObserveAlways)
                            {
                                sensor.IsWatched = true;
                                //Inform the watch, save to db
                            }
                        }
                    }
                }                    
            }
        Sets = null;
        //var m = new MqttApplicationMessageBuilder().WithTopic("safa").WithPayload("dana").Build();
        //if (mqttClient.IsConnected) await mqttClient.PublishAsync(m);
        return null;
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