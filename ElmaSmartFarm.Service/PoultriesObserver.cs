using MQTTnet;
using Serilog;

namespace ElmaSmartFarm.Service
{
    public partial class Worker
    {
        private async Task<string?> ObservePoultriesAsync()
        {
            var IsInPeriod = Poultries != null && Poultries.Count > 0 && Poultries.Any(p => p.IsInPeriod);
            if (IsInPeriod == false && config.system.ObserveAlways == false) return null;

            //var m = new MqttApplicationMessageBuilder().WithTopic("safa").WithPayload("dana").Build();
            //if (mqttClient.IsConnected) await mqttClient.PublishAsync(m);
            return null;
        }

        private async Task RunObserverTimerAsync()
        {
            while (true)
            {
                try
                {
                    if (CanRunObserver)
                    {
                        if (Poultries.Any(p => p.IsInPeriod) || config.system.ObserveAlways)
                        {
                            var result = await ObservePoultriesAsync();
                            if (!string.IsNullOrEmpty(result))
                                Log.Error($"Observation process returned with error: {result}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception thrown in Observer:");
                }
                await Task.Delay(TimeSpan.FromSeconds(config.system.ObserverCheckInterval));
            }
        }
    }
}