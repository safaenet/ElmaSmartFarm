using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.DALModels;
using MQTTnet;
using MQTTnet.Client;
using System.Text;

namespace ElmaSmartFarm.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;                    
        }

        private MqttFactory mqttFactory;
        private IMqttClient mqttClient;
        private MqttClientOptions options;
        private string mqtt_broker;
        private int mqtt_port;
        private string mqtt_username;
        private string mqtt_password;
        private int retry_seconds;
        private string sensor_topic;

        public override async Task<Task> StartAsync(CancellationToken cancellationToken)
        {
            mqtt_broker = SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_broker").Value ?? "192.168.1.106";
            mqtt_port = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_port").Value ?? "1883");
            mqtt_username = SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_username").Value ?? "admin";
            mqtt_password = SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_password").Value ?? "admin";
            retry_seconds = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("mqtt:retry_seconds").Value ?? "2");
            sensor_topic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:sensor_topic").Value ?? "Elma/Sensors/#";

            mqttFactory = new();
            mqttClient = mqttFactory.CreateMqttClient();
            options = new MqttClientOptionsBuilder()
                   .WithClientId(Guid.NewGuid().ToString())
                   .WithTcpServer(mqtt_broker, mqtt_port)
                   .WithCleanSession()
                   .Build();

            await mqttClient.ConnectAsync(options);
            if (mqttClient.IsConnected) _logger.LogInformation("Connected to MQTT Broker {mqtt_broker}, Port {mqtt_port}.", mqtt_broker, mqtt_port);
            else _logger.LogError("Cannot connect to MQTT Broker, Retrying in {retry_seconds} seconds...", retry_seconds);
            mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
            return base.StartAsync(cancellationToken);
        }

        public override async Task<Task> StopAsync(CancellationToken cancellationToken)
        {
            await mqttClient.DisconnectAsync();
            mqttClient.Dispose();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var mqttTopicFilterBuilder = new MqttTopicFilterBuilder().WithTopic(sensor_topic).Build();
            await mqttClient.SubscribeAsync(mqttTopicFilterBuilder);
            //var m = new MqttApplicationMessageBuilder().WithTopic("safa").WithPayload("dana").Build();
            //if(mqttClient.IsConnected) await mqttClient.PublishAsync(m);

            while (!stoppingToken.IsCancellationRequested) ;
        }

        private static Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            MqttMessageModel message = new()
            {
                ClientId = arg.ClientId,
                Topic = arg.ApplicationMessage.Topic,
                Payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload),
                DateCreated = DateTime.Now
            };
            
            return Task.CompletedTask;
        }
    }
}