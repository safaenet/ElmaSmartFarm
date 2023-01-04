using ElmaSmartFarm.ApiClient.Models;
using ElmaSmartFarm.SharedLibrary.Models;
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

    public static async Task<T> GetAsync<T>(HttpClient httpClient, string Key) where T : class
    {
        try
        {
            var Url = $"{Key}";
            var response = await httpClient.GetAsync(Url);
            return await ReadResponse<T>(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ConnectionManager");
        }
        return null;
    }

    public static async Task<U> PostAsync<T, U>(HttpClient httpClient, string Key, T model) where U : class
    {
        try
        {
            var Url = $"{Key}";
            var response = await httpClient.PostAsJsonAsync(Url, model);
            return await ReadResponse<U>(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ConnectionManager");
        }
        return null;
    }

    public static async Task<U> PutAsync<T, U>(HttpClient httpClient, string Key, T model) where U : class
    {
        try
        {
            var Url = $"{Key}";
            var response = await httpClient.PutAsJsonAsync(Url, model);
            return await ReadResponse<U>(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ConnectionManager");
        }
        return null;
    }

    public static async Task<bool> DeleteAsync(HttpClient httpClient, string Key)
    {
        try
        {
            var Url = $"{Key}";
            var response = await httpClient.DeleteAsync(Url);
            if (!response.IsSuccessStatusCode)
            {
                Log.Error($"Request returned with status code {(int)response.StatusCode}:{response.StatusCode}");
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ConnectionManager");
        }
        return false;
    }

    private static async Task<T> ReadResponse<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            Log.Error($"Request returned with status code {(int)response.StatusCode}:{response.StatusCode}");
            return default;
        }
        return await response.Content.ReadAsAsync<T>();
    }
}