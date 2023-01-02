using MQTTnet;
using MQTTnet.Client;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ElmaSmartFarm.ApiClient.MqttManager;

public class MqttManager
{
    public static async Task<bool> SendMqttMessage(IMqttClient mqttClient, string Topic, string Payload = "1", int QoS = 2)
    {
        try
        {
            if (string.IsNullOrEmpty(Topic) || string.IsNullOrEmpty(Payload)) return false;
            if (QoS > 2) QoS = 2; else if (QoS < 0) QoS = 0;
            var message = new MqttApplicationMessageBuilder().WithTopic(Topic).WithPayload(Payload).WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)QoS).Build();
            MqttClientPublishResult result = new();
            if (mqttClient.IsConnected) result = await mqttClient.PublishAsync(message);
            if (!result.IsSuccess) Log.Warning("Mqtt message failed to be sent.");
            return result.IsSuccess;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SendMqttMessage");
        }
        return false;
    }

    public static MqttClientOptions BuildMqttClientOptions(string address, int port, bool authenticated, string username, string password)
    {
        var options = new MqttClientOptionsBuilder()
               .WithClientId(Guid.NewGuid().ToString())
               .WithTcpServer(address, port)
               .WithCleanSession();
        if (authenticated) options.WithCredentials(username, password);
        var result = options.Build();
        return result;
    }

    public static async Task<bool> ConnectToMqttBroker(IMqttClient mqttClient, MqttClientOptions options)
    {
        if (mqttClient.IsConnected) return true;
        try
        {
            _ = await mqttClient.ConnectAsync(options);
            return mqttClient.IsConnected;
        }
        catch
        {
            Log.Error("Error connecting to MQTT Broker.");
        }
        return false;
    }
}