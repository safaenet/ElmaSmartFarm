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
        PoultrySettings = Config.Config.GetPoultrySettings(poultryIndex);
        httpClient = ConnectionManager.CreateHttpClient(PoultrySettings);
    }

    private PoultrySettingsModel PoultrySettings;
    private MqttConnectionSettingsModel MqttConnectionSettings;
    private MqttClientOptions mqttOptions;
    private HttpClient httpClient;
    private IMqttClient mqttClient;
    private bool IsInitialized;
    private PoultryModel poultry;
    private List<MqttMessageModel> unknownMqttMessages;
    private List<SensorErrorModel> alarmableSensorErrors;
    private List<FarmInPeriodErrorModel> alarmableFarmPeriodErrors;
    private List<PoultryInPeriodErrorModel> alarmablePoultryPeriodErrors;
    private DateTime systemStartupTime;

    public event EventHandler OnDataChanged;
    public event EventHandler OnMqttReceived;
    public event EventHandler OnHttpReceived;
    public bool IsRunning { get; set; }

    public PoultryModel Poultry
    {
        get => poultry; set
        {
            poultry = value;
        }
    }

    public List<MqttMessageModel> UnknownMqttMessages
    {
        get => unknownMqttMessages; set
        {
            unknownMqttMessages = value;
        }
    }

    public List<SensorErrorModel> AlarmableSensorErrors
    {
        get => alarmableSensorErrors; set
        {
            alarmableSensorErrors = value;
        }
    }

    public List<FarmInPeriodErrorModel> AlarmableFarmPeriodErrors
    {
        get => alarmableFarmPeriodErrors; set
        {
            alarmableFarmPeriodErrors = value;
        }
    }

    public List<PoultryInPeriodErrorModel> AlarmablePoultryPeriodErrors
    {
        get => alarmablePoultryPeriodErrors; set
        {
            alarmablePoultryPeriodErrors = value;
        }
    }

    public DateTime SystemStartupTime
    {
        get => systemStartupTime; set
        {
            systemStartupTime = value;
        }
    }

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
            OnMqttReceived?.Invoke(this, EventArgs.Empty);
            OnDataChanged?.Invoke(this, EventArgs.Empty);
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
        try
        {
            MqttConnectionSettings = await HttpProcessor.GetMqttConnectionSettings(httpClient);
            mqttOptions = ConnectionManager.BuildMqttClientOptions(MqttConnectionSettings.mqtt_address, MqttConnectionSettings.mqtt_port, MqttConnectionSettings.mqtt_authentication, MqttConnectionSettings.mqtt_username, MqttConnectionSettings.mqtt_password);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error in {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType}");
        }
    }

    public async Task RequestPoultryOverHttp()
    {
        var p = await HttpProcessor.RequestPoultry(httpClient);
        Poultry = p.Poultry;
        UnknownMqttMessages = p.UnknownMqttMessages;
        AlarmableSensorErrors = p.AlarmableSensorErrors;
        AlarmableFarmPeriodErrors = p.AlarmableFarmPeriodErrors;
        AlarmablePoultryPeriodErrors = p.AlarmablePoultryPeriodErrors;
        SystemStartupTime = p.SystemStartupTime;
        OnHttpReceived?.Invoke(this, EventArgs.Empty);
        OnDataChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task ConnectAsync()
    {
        await GetMqttConnectionSettings();
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
        //httpClient?.Dispose();
        //await ConnectionManager.UnsubscribeFromMqttTopic(mqttClient, MqttConnectionSettings.mqtt_subscribe_topic);
        if (mqttClient != null && mqttClient.IsConnected) await mqttClient.DisconnectAsync();
        mqttClient?.Dispose();
        Poultry = null;
    }
}