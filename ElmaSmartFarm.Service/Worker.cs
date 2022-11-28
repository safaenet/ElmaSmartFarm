using ElmaSmartFarm.DataLibraryCore.Interfaces;
using ElmaSmartFarm.SharedLibrary.Config;
using ElmaSmartFarm.SharedLibrary.Models;
using MQTTnet;
using MQTTnet.Client;
using System.Text;

namespace ElmaSmartFarm.Service
{
    public class Worker : BackgroundService
    {
        public Worker(ILogger<Worker> logger, IMqttProcessor mqttProcessor)
        {
            _logger = logger;
            MqttProcessor = mqttProcessor;
        }

        private readonly ILogger<Worker> _logger;
        private readonly IMqttProcessor MqttProcessor;
        private MqttFactory mqttFactory;
        private IMqttClient mqttClient;
        private MqttClientOptions options;
        private string mqtt_broker;
        private int mqtt_port;
        private bool authentication_enabled;
        private string mqtt_username;
        private string mqtt_password;
        private int retry_seconds;
        private string sensor_topic;

        public override async Task<Task> StartAsync(CancellationToken cancellationToken)
        {
            mqtt_broker = Config.MQTT.Broker;
            mqtt_port = Config.MQTT.Port;
            authentication_enabled = Config.MQTT.AuthenticationEnabled;
            mqtt_username = Config.MQTT.Username;
            mqtt_password = Config.MQTT.Password;
            retry_seconds = Config.MQTT.RetryInterval;
            sensor_topic = Config.MQTT.SensorTopic;

            mqttFactory = new();
            mqttClient = mqttFactory.CreateMqttClient();
            options = new MqttClientOptionsBuilder()
                   .WithClientId(Guid.NewGuid().ToString())
                   .WithTcpServer(mqtt_broker, mqtt_port)
                   .WithCleanSession()
                   .Build();
            mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
            mqttClient.ConnectingAsync += MqttClient_ConnectingAsync;
            mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
            await TryReconnectAsync();
            return base.StartAsync(cancellationToken);
        }

        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            _logger.LogWarning("Disconnected from MQTT Broker: {mqtt_broker}, Port: {mqtt_port}, Client Id: {ClientId}.", mqtt_broker, mqtt_port, options.ClientId);
            return Task.CompletedTask;
        }

        private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            _logger.LogInformation("Successfully connected to MQTT Broker: {mqtt_broker}, Port: {mqtt_port}, Client Id: {ClientId}.", mqtt_broker, mqtt_port, options.ClientId);
            return Task.CompletedTask;
        }

        private Task MqttClient_ConnectingAsync(MqttClientConnectingEventArgs arg)
        {
            _logger.LogInformation("Connecting to MQTT Broker: {mqtt_broker}, Port: {mqtt_port}, Client Id: {ClientId}.", mqtt_broker, mqtt_port, options.ClientId);
            return Task.CompletedTask;
        }

        private async Task TryReconnectAsync()
        {
            if (mqttClient.IsConnected) return;
            while (!mqttClient.IsConnected)
            {
                try
                {
                    _ = await mqttClient.ConnectAsync(options);
                    
                    var mqttTopicFilterBuilder = new MqttTopicFilterBuilder().WithTopic(sensor_topic + "/#").Build();
                    await mqttClient.SubscribeAsync(mqttTopicFilterBuilder);
                    _logger.LogInformation("Subscribed to MQTT topic {topic}.", sensor_topic + "/#");
                    //var m = new MqttApplicationMessageBuilder().WithTopic("safa").WithPayload("dana").Build();
                    //if(mqttClient.IsConnected) await mqttClient.PublishAsync(m);
                }
                catch
                {
                    _logger.LogError("Error connecting to MQTT Broker. retrying in {retry_seconds} seconds...", retry_seconds);
                    Task.Delay(retry_seconds * 1000).Wait();
                }
            }
        }

        private async Task MqttLoopAsync()
        {
            if (!mqttClient.IsConnected) await TryReconnectAsync();
        }

        public override async Task<Task> StopAsync(CancellationToken cancellationToken)
        {
            await mqttClient.DisconnectAsync();
            mqttClient.Dispose();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await MqttLoopAsync();
            }
        }

        //Elma/ToServer/Sensors/Temp/{Id}
        //Elma/ToServer/Sensors/Humid/{Id}
        private async Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            MqttMessageModel message = new()
            {
                Topic = arg.ApplicationMessage.Topic,
                Payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload),
                ReadDate = DateTime.Now
            };
            _ = await MqttProcessor.ProcessMqttMessageAsync(message);
        }
    }
}