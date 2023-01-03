using ElmaSmartFarm.ApiClient.Models;
using ElmaSmartFarm.SharedLibrary.Models;
using MQTTnet;
using MQTTnet.Client;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElmaSmartFarm.ApiClient.DataAccess;
public class PoultryManager
{
    public PoultryManager(int poultryIndex)
    {
        Index = poultryIndex;
        PoultrySettings = Config.Config.GetPoultrySettings(poultryIndex);
        GetMqttConnectionSettings().ConfigureAwait(true);
    }

    private int Index;
    private PoultrySettingsModel PoultrySettings;
    private MqttConnectionSettingsModel MqttConnectionSettings;
    private MqttClientOptions mqttOptions;
    private HttpClient httpClient;
    private IMqttClient mqttClient;
    private bool IsRunning;
    public PoultryModel Poultry { get; set; }

    private async Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        
    }

    private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        Log.Warning("Disconnected from MQTT Broker: {mqtt_broker}, Port: {mqtt_port}, Client Id: {ClientId}.");
        return Task.CompletedTask;
    }

    private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        Log.Information("Successfully connected to MQTT Broker: {mqtt_broker}, Port: {mqtt_port}, Client Id: {ClientId}.");
        return Task.CompletedTask;
    }

    private Task MqttClient_ConnectingAsync(MqttClientConnectingEventArgs arg)
    {
        Log.Information("Connecting to MQTT Broker: {mqtt_broker}, Port: {mqtt_port}, Client Id: {ClientId}.");
        return Task.CompletedTask;
    }

    private async Task GetMqttConnectionSettings()
    {
        MqttConnectionSettings = await HttpProcessor.GetMqttConnectionSettings(httpClient);
        mqttOptions = ConnectionManager.BuildMqttClientOptions(MqttConnectionSettings.mqtt_address, MqttConnectionSettings.mqtt_port, MqttConnectionSettings.mqtt_authentication, MqttConnectionSettings.mqtt_username, MqttConnectionSettings.mqtt_password);
    }

    public async Task ConnectAsync()
    {
        httpClient = ConnectionManager.CreateHttpClient(PoultrySettings);
        mqttClient = ConnectionManager.CreateMqttClient(MqttClient_ApplicationMessageReceivedAsync, MqttClient_ConnectingAsync, MqttClient_ConnectedAsync, MqttClient_DisconnectedAsync);
        await TryReconnectToMqttAsync();
    }

    public async Task DisconnectAsync()
    {
        IsRunning = false;
        httpClient?.Dispose();
        if (mqttClient != null && mqttClient.IsConnected) await mqttClient.DisconnectAsync();
        mqttClient?.Dispose();
    }

    private async Task TryReconnectToMqttAsync()
    {
        if (mqttClient.IsConnected)
        {
            IsRunning = true;
            return;
        }
        int retryCount = 1;
        while (!mqttClient.IsConnected && retryCount <= Config.Config.mqtt_retry_times)
        {
            try
            {
                _ = await mqttClient.ConnectAsync(mqttOptions);

                var mqttTopicFilterBuilder = new MqttTopicFilterBuilder().WithTopic(MqttConnectionSettings.mqtt_subscribe_topic).Build();
                await mqttClient.SubscribeAsync(mqttTopicFilterBuilder);
                IsRunning = true;
                Log.Information("Subscribed to MQTT topic {topic}.", MqttConnectionSettings.mqtt_subscribe_topic);
            }
            catch(Exception ex)
            {
                IsRunning = false;
                Log.Error(ex, $"Attemp {retryCount}: Error connecting to MQTT Broker. retrying in {Config.Config.mqtt_retry_interval} seconds for {Config.Config.mqtt_retry_times} times...");
                retryCount++;
                Task.Delay(Config.Config.mqtt_retry_interval * 1000).Wait();
            }
        }
        IsRunning = false;
        Log.Error($"Failed to connect to Mqtt broker after {Config.Config.mqtt_retry_times} attemps");
    }
}