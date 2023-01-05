using ElmaSmartFarm.ApiClient.Models;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using MQTTnet.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
    public PoultryModel Poultry { get; private set; }
    public List<MqttMessageModel> UnknownMqttMessages { get; private set; }
    public List<SensorErrorModel> AlarmableSensorErrors { get; private set; }
    public List<FarmInPeriodErrorModel> AlarmableFarmPeriodErrors { get; private set; }
    public List<PoultryInPeriodErrorModel> AlarmablePoultryPeriodErrors { get; private set; }
    public DateTime SystemUpTime { get; private set; }

    private async Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var ticks = DateTime.Now.Ticks;
        try
        {
            var Payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            var poultryDto = JsonSerializer.Deserialize<PoultryDtoModel>(Payload);
            Poultry = poultryDto.Poultry;
            UnknownMqttMessages = poultryDto.UnknownMqttMessages;
            AlarmableSensorErrors = poultryDto.AlarmableSensorErrors;
            AlarmableFarmPeriodErrors = poultryDto.AlarmableFarmPeriodErrors;
            AlarmablePoultryPeriodErrors = poultryDto.AlarmablePoultryPeriodErrors;
            if (!IsInitialized) IsInitialized = true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error in {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType}");
        }
        finally
        {
            if (Config.Config.verbose_mode) Log.Information($"===============  End of mqtt process {TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds} ms) ===============");
        }
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