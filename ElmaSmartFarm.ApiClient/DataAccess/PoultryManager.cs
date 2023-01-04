using ElmaSmartFarm.ApiClient.Models;
using ElmaSmartFarm.SharedLibrary.Models;
using MQTTnet.Client;
using Serilog;
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

    private readonly int Index;
    private PoultrySettingsModel PoultrySettings;
    private MqttConnectionSettingsModel MqttConnectionSettings;
    private MqttClientOptions mqttOptions;
    private HttpClient httpClient;
    private IMqttClient mqttClient;
    private bool IsRunning;
    private bool IsInitialized;
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
        //var poultry = await HttpProcessor.RequestPoultry(httpClient);
        //if (poultry == null)
        //{
        //    await DisconnectAsync();
        //    IsInitialized = false;
        //    return;
        //}
        //Poultry = poultry;
        mqttClient = ConnectionManager.CreateMqttClient(MqttClient_ApplicationMessageReceivedAsync, MqttClient_ConnectingAsync, MqttClient_ConnectedAsync, MqttClient_DisconnectedAsync);
        if (!await ConnectionManager.TryReconnectToMqttAsync(mqttClient, mqttOptions))
        {
            await DisconnectAsync();
            IsRunning = false;
            IsInitialized = false;
            Poultry = null;
            Log.Error("Failed to connect to server.");
            return;
        }
        await ConnectionManager.SubscribeToMqttTopic(mqttClient, MqttConnectionSettings.mqtt_subscribe_topic);
        if (!await ConnectionManager.SendMqttMessage(mqttClient, MqttConnectionSettings.mqtt_request_poultry_topic, "1"))
        {
            Log.Error($"Failed to publish mqtt message. Topic: {MqttConnectionSettings.mqtt_request_poultry_topic}, Payload: 1");
            IsRunning = false;
            IsInitialized = false;
        }
        else IsRunning = true;
    }

    public async Task DisconnectAsync()
    {
        IsRunning = false;
        IsInitialized = false;
        httpClient?.Dispose();
        await ConnectionManager.UnsubscribeFromMqttTopic(mqttClient, MqttConnectionSettings.mqtt_subscribe_topic);
        if (mqttClient != null && mqttClient.IsConnected) await mqttClient.DisconnectAsync();
        mqttClient?.Dispose();
        Poultry = null;
    }
}