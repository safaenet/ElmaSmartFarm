using MQTTnet;
using MQTTnet.Client;
using System.Text;

namespace ElmaSmartFarm.Service
{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            string mqtt_broker = SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_broker").Value ?? "192.168.1.106";
            int mqtt_port = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_port").Value ?? "1883");
            string mqtt_username = SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_username").Value ?? "admin";
            string mqtt_password = SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_password").Value ?? "admin";
            MqttFactory mqttFactory = new();
            IMqttClient mqttClient = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(mqtt_broker, mqtt_port)
                .WithCleanSession()
                .Build();
            await mqttClient.ConnectAsync(options);

            var mqttTopicFilterBuilder = new MqttTopicFilterBuilder().WithTopic("safa/#").Build();
            await mqttClient.SubscribeAsync(mqttTopicFilterBuilder);
            mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
            Console.WriteLine("Connected to MQTT");
            while (true) ;

            //var m = new MqttApplicationMessageBuilder().WithTopic("safa").WithPayload("dana").Build();
            //if(mqttClient.IsConnected) await mqttClient.PublishAsync(m);
        }

        private static Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            Console.Write(arg.ApplicationMessage.Topic + ": ");
            Console.WriteLine(Encoding.UTF8.GetString(arg.ApplicationMessage.Payload));
            return Task.CompletedTask;
        }

        private async Task loop()
        {
            while (true) ;
        }
    }

    IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.AddHostedService<Worker>();
        })
        .Build();

    await host.RunAsync();
}