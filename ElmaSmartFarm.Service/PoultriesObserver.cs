using MQTTnet;
using Serilog;

namespace ElmaSmartFarm.Service
{
    public partial class Worker
    {
        private async Task<string?> ObservePoultries()
        {
            
            //var m = new MqttApplicationMessageBuilder().WithTopic("safa").WithPayload("dana").Build();
            //if (mqttClient.IsConnected) await mqttClient.PublishAsync(m);
            return null;
        }

        private async Task RunObserverTimer()
        {
            while (true)
            {
                try
                {
                    if (CanRunObserver)
                    {
                        if (Poultries.Any(p => p.IsInPeriod) || config.system.ObserveAlways)
                        {
                            var result = await ObservePoultries();
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