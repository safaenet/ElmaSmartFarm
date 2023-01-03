using ElmaSmartFarm.ApiClient.Models;
using ElmaSmartFarm.SharedLibrary.Models;
using Serilog;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElmaSmartFarm.ApiClient.DataAccess;
public class HttpProcessor
{
    public static async Task<MqttConnectionSettingsModel> GetMqttConnectionSettings(HttpClient client)
    {
        var mqtt = await ConnectionManager.GetAsync<MqttConnectionSettingsModel>(client, "Mqtt/GetMqttSettings");
        if (mqtt.IsSuccess) return mqtt.Payload;
        else
        {
            Log.Error("Request to get mqtt settings returned with error.");
        }
        return null;
    }
}