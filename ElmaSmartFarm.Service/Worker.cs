using ElmaSmartFarm.DataLibraryCore.Interfaces;
using ElmaSmartFarm.SharedLibrary.Config;
using ElmaSmartFarm.SharedLibrary.Models;
using MQTTnet;
using MQTTnet.Client;
using Serilog;
using System.Text;

namespace ElmaSmartFarm.Service
{
    public partial class Worker : BackgroundService
    {
        public Worker(IDbProcessor dbProcessor)
        {
            DbProcessor = dbProcessor;
        }

        private Config config;
        private readonly IDbProcessor DbProcessor;
        private MqttFactory mqttFactory;
        private IMqttClient mqttClient;
        private MqttClientOptions options;
        private List<PoultryModel> Poultries;
        private readonly List<MqttMessageModel> UnknownMqttMessages = new();

        public override async Task<Task> StartAsync(CancellationToken cancellationToken)
        {
            config = new();
            mqttFactory = new();
            mqttClient = mqttFactory.CreateMqttClient();
            options = new MqttClientOptionsBuilder()
                   .WithClientId(Guid.NewGuid().ToString())
                   .WithTcpServer(config.mqtt.Broker, config.mqtt.Port)
                   .WithCleanSession()
                   .Build();
            mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
            mqttClient.ConnectingAsync += MqttClient_ConnectingAsync;
            mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;

            Poultries = await DbProcessor.LoadPoultriesAsync();
            await TryReconnectAsync();
            return base.StartAsync(cancellationToken);
        }

        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Log.Warning("Disconnected from MQTT Broker: {mqtt_broker}, Port: {mqtt_port}, Client Id: {ClientId}.", config.mqtt.Broker, config.mqtt.Port, options.ClientId);
            return Task.CompletedTask;
        }

        private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Log.Information("Successfully connected to MQTT Broker: {mqtt_broker}, Port: {mqtt_port}, Client Id: {ClientId}.", config.mqtt.Broker, config.mqtt.Port, options.ClientId);
            return Task.CompletedTask;
        }

        private Task MqttClient_ConnectingAsync(MqttClientConnectingEventArgs arg)
        {
            Log.Information("Connecting to MQTT Broker: {mqtt_broker}, Port: {mqtt_port}, Client Id: {ClientId}.", config.mqtt.Broker, config.mqtt.Port, options.ClientId);
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
                    
                    var mqttTopicFilterBuilder = new MqttTopicFilterBuilder().WithTopic(config.mqtt.ToServerTopic + "#").Build();
                    await mqttClient.SubscribeAsync(mqttTopicFilterBuilder);
                    Log.Information("Subscribed to MQTT topic {topic}.", config.mqtt.ToServerTopic + "#");
                    //var m = new MqttApplicationMessageBuilder().WithTopic("safa").WithPayload("dana").Build();
                    //if(mqttClient.IsConnected) await mqttClient.PublishAsync(m);
                }
                catch
                {
                    Log.Error("Error connecting to MQTT Broker. retrying in {retry_seconds} seconds...", config.mqtt.RetryInterval);
                    Task.Delay(config.mqtt.RetryInterval * 1000).Wait();
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
            try
            {
                if (config.VerboseMode) Log.Information("===============  Begin mqtt process  ===============");
                MqttMessageModel message = new()
                {
                    Topic = arg.ApplicationMessage.Topic,
                    Payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload),
                    ReadDate = DateTime.Now,
                    Retained = arg.ApplicationMessage.Retain,
                    QoS = (int)arg.ApplicationMessage.QualityOfServiceLevel
                };
                if (config.VerboseMode) Log.Information($"MQTT Message received. Topic: {message.Topic}, Payload: {message.Payload}");
                await Task.Run(() => ProcessMqttMessageAsync(message));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when handling mqtt message.");
            }
            finally
            {
                if (config.VerboseMode) Log.Information("===============  End of mqtt process  ===============");
            }
        }
    }
}