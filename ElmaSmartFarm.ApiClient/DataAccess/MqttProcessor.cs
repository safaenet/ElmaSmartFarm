using MQTTnet;
using MQTTnet.Client;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ElmaSmartFarm.ApiClient.DataAccess;

public class MqttProcessor
{
    public static async Task<bool> SendMqttMessage(IMqttClient mqttClient, string Topic, string Payload, int QoS = 2)
    {
        try
        {
            if (string.IsNullOrEmpty(Topic) || string.IsNullOrEmpty(Payload)) return false;
            if (QoS > 2) QoS = 2; else if (QoS < 0) QoS = 0;
            var message = new MqttApplicationMessageBuilder().WithTopic(Topic).WithPayload(Payload).WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)QoS).Build();
            MqttClientPublishResult result = new();
            if (!mqttClient.IsConnected)
            {
                Log.Error("Send mqtt message failed because client is not connected to broker.");
                return false;
            }
            result = await mqttClient.PublishAsync(message);
            if (!result.IsSuccess) Log.Warning("Mqtt message failed to be sent.");
            return result.IsSuccess;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in MqttProcessor");
        }
        return false;
    }
}