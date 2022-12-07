﻿using ElmaSmartFarm.SharedLibrary;
using MQTTnet;
using Serilog;

namespace ElmaSmartFarm.Service;

public partial class Worker
{
    private async Task<string?> ObservePoultriesAsync()
    {
        var IsInPeriod = Poultries != null && Poultries.Count > 0 && Poultries.Any(p => p.IsInPeriod);
        if (IsInPeriod == false && config.system.ObserveAlways == false) return null;

        var Now = DateTime.Now;
        var Sets = Poultries?.SelectMany(p => p.Farms.Select(f => f.Temperatures));
        if (Sets != null)
            foreach (var set in Sets)
            {
                if (set.HasSensors)
                    foreach (var sensor in set.Sensors)
                    {
                        if (sensor.HasError)
                        {
                            if (sensor.ActiveErrors != null && sensor.ActiveErrors.Any())
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
                                        if (e.DateHappened.IsElapsed(100) && sensor.IsInPeriod) sensor.IsWatched = false;
                                    }
                                }
                        }
                    }
            }
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
                    if (Poultries.Any(p => p.IsInPeriod) || config.system.ObserveAlways)
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