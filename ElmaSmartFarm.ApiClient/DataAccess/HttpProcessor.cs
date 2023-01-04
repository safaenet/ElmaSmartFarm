using ElmaSmartFarm.SharedLibrary.Models;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElmaSmartFarm.ApiClient.DataAccess;
public class HttpProcessor
{
    public static async Task<MqttConnectionSettingsModel> GetMqttConnectionSettings(HttpClient client)
    {
        try
        {
            var response = await ConnectionManager.GetAsync<MqttConnectionSettingsModel>(client, "Mqtt/GetMqttSettings");
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in HttpProcessor");
        }
        return null;
    }

    public static async Task<PoultryModel> RequestPoultry(HttpClient client)
    {
        try
        {
            var response = await ConnectionManager.GetAsync<PoultryModel>(client, "Poultry/Instance");
            if (response == null)
            {
                Log.Error("Poultry instance request returned NULL response.");
                return null;
            }
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in HttpProcessor");
        }
        return null;
    }
}