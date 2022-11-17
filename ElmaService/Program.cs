
using MQTTnet;
using MQTTnet.Client;

public class Program
{
    public async Task Main(string[] args)
    {
        var mqttFactory = new MqttFactory();
        IMqttClient mqttClient=mqttFactory.CreateMqttClient();
        var options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer("192.168.1.106", 1883)
            .WithCleanSession()
            .Build();
        await mqttClient.ConnectAsync(options);
    }
}