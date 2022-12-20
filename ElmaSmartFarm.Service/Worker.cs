using ElmaSmartFarm.DataLibraryCore.Interfaces;
using ElmaSmartFarm.SharedLibrary.Config;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Alarm;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using MQTTnet;
using MQTTnet.Client;
using Serilog;
using System.Text;

namespace ElmaSmartFarm.Service;

public partial class Worker : BackgroundService
{
    public Worker(IDbProcessor dbProcessor, Config cfg)
    {
        DbProcessor = dbProcessor;
        config = cfg;
        Task.Run(() => RunObserverTimerAsync());
    }

    private Config config;
    private readonly IDbProcessor DbProcessor;
    private MqttFactory mqttFactory;
    private IMqttClient mqttClient;
    private MqttClientOptions options;
    private PoultryModel Poultry;
    private readonly List<MqttMessageModel> UnknownMqttMessages = new();
    private readonly List<SensorErrorModel> AlarmableSensorErrors = new();
    private readonly List<FarmInPeriodErrorModel> AlarmableFarmPeriodErrors = new();
    private readonly List<PoultryInPeriodErrorModel> AlarmablePoultryPeriodErrors = new();
    private bool CanRunObserver;
    private DateTime SystemUpTime;

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

        Poultry = await DbProcessor.LoadPoultryAsync();
        await TryReconnectAsync();
        SystemUpTime = DateTime.Now;
        CanRunObserver = true;
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
            }
            catch
            {
                Log.Error("Error connecting to MQTT Broker. retrying in {retry_seconds} seconds...", config.mqtt.RetryInterval);
                Task.Delay(config.mqtt.RetryInterval * 1000).Wait();
            }
        }
    }

    public override async Task<Task> StopAsync(CancellationToken cancellationToken)
    {
        await mqttClient.DisconnectAsync();
        mqttClient.Dispose();
        CanRunObserver = false;
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!mqttClient.IsConnected) await TryReconnectAsync();
        }
    }

    private async Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var ticks = DateTime.Now.Ticks;
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
            if (config.VerboseMode) Log.Information($"===============  End of mqtt process {TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds} ms) ===============");
        }
    }
}