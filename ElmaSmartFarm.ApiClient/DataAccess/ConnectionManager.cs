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

    public static async Task<bool> TryReconnectToMqttAsync(IMqttClient mqttClient, MqttClientOptions mqttOptions)
    {
        if (mqttClient.IsConnected) return true;
        int retryCount = 1;
        while (!mqttClient.IsConnected && retryCount <= Config.Config.mqtt_retry_times)
        {
            try
            {
                _ = await mqttClient.ConnectAsync(mqttOptions);
                if (mqttClient.IsConnected) return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Attemp {retryCount}: Error connecting to MQTT Broker. retrying in {Config.Config.mqtt_retry_interval} seconds for {Config.Config.mqtt_retry_times} times...");
                retryCount++;
                Task.Delay(Config.Config.mqtt_retry_interval * 1000).Wait();
            }
        }
        Log.Error($"Failed to connect to Mqtt broker after {Config.Config.mqtt_retry_times} attemps");
        return false;
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