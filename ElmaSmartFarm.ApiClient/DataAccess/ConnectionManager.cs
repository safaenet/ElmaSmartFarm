using ElmaSmartFarm.ApiClient.Models;
using MQTTnet;
using MQTTnet.Client;
using Serilog;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ElmaSmartFarm.ApiClient.DataAccess;

public class ConnectionManager
{
    public static async Task<bool> SendMqttMessage(IMqttClient mqttClient, string Topic, string Payload = "1", int QoS = 2)
    {
        try
        {
            if (string.IsNullOrEmpty(Topic) || string.IsNullOrEmpty(Payload)) return false;
            if (QoS > 2) QoS = 2; else if (QoS < 0) QoS = 0;
            var message = new MqttApplicationMessageBuilder().WithTopic(Topic).WithPayload(Payload).WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)QoS).Build();
            MqttClientPublishResult result = new();
            if (mqttClient.IsConnected) result = await mqttClient.PublishAsync(message);
            if (!result.IsSuccess) Log.Warning("Mqtt message failed to be sent.");
            return result.IsSuccess;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ConnectionManager");
        }
        return false;
    }

    public static MqttClientOptions BuildMqttClientOptions(string address, int port, bool authenticated, string username, string password)
    {
        var options = new MqttClientOptionsBuilder()
               .WithClientId(Guid.NewGuid().ToString())
               .WithTcpServer(address, port)
               .WithCleanSession();
        if (authenticated) options.WithCredentials(username, password);
        var result = options.Build();
        return result;
    }

    public static IMqttClient CreateMqttClient(Func<MqttApplicationMessageReceivedEventArgs, Task> received, Func<MqttClientConnectingEventArgs, Task> connecting, Func<MqttClientConnectedEventArgs, Task> connected, Func<MqttClientDisconnectedEventArgs, Task> disconnected)
    {
        MqttFactory mqttFactory = new();
        IMqttClient client;
        client = mqttFactory.CreateMqttClient();
        client.ApplicationMessageReceivedAsync += received;
        client.ConnectingAsync += connecting;
        client.ConnectedAsync += connected;
        client.DisconnectedAsync += disconnected;
        return client;
    }

    public static HttpClient CreateHttpClient(PoultrySettingsModel settings)
    {
        HttpClient client = new();
        client.BaseAddress = new Uri(settings.api_url);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //Get token and set to HttpClient
        return client;
    }

    public static async Task<HttpResponseModel<T>> GetAsync<T>(HttpClient httpClient, string Key) where T : class
    {
        try
        {
            var Url = $"{Key}";
            var response = await httpClient.GetAsync(Url);
            HttpResponseModel<T> result = new();
            result.StatusCode = (int)response.StatusCode;
            result.Payload = response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ConnectionManager");
        }
        return new() { StatusCode = 400 };
    }

    public static async Task<HttpResponseModel<U>> PostAsync<T, U>(HttpClient httpClient, string Key, T model) where U : class
    {
        try
        {
            var Url = $"{Key}";
            var response = await httpClient.PostAsJsonAsync(Url, model);
            HttpResponseModel<U> result = new();
            result.StatusCode = (int)response.StatusCode;
            result.Payload = response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<U>() : null;
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ConnectionManager");
        }
        return new() { StatusCode = 400 };
    }

    public static async Task<HttpResponseModel<U>> PutAsync<T, U>(HttpClient httpClient, string Key, T model) where U : class
    {
        try
        {
            var Url = $"{Key}";
            var response = await httpClient.PutAsJsonAsync(Url, model);
            HttpResponseModel<U> result = new();
            result.StatusCode = (int)response.StatusCode;
            result.Payload = response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<U>() : null;
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ConnectionManager");
        }
        return new() { StatusCode = 400 };
    }

    public static async Task<HttpResponseBaseModel> DeleteAsync(HttpClient httpClient, string Key)
    {
        try
        {
            var Url = $"{Key}";
            var response = await httpClient.DeleteAsync(Url);
            HttpResponseBaseModel result = new();
            result.StatusCode= (int)response.StatusCode;
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ConnectionManager");
        }
        return new() { StatusCode = 400 };
    }
}