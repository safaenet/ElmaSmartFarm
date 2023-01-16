using ElmaSmartFarm.ApiClient.Models;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ElmaSmartFarm.ApiClient.Config;

public static class Config
{
    public static bool verbose_mode => SettingsDataAccess.GetValue<bool>(nameof(verbose_mode));
    public static int mqtt_retry_interval => SettingsDataAccess.GetValue<int>(nameof(mqtt_retry_interval));
    public static int mqtt_retry_times => SettingsDataAccess.GetValue<int>(nameof(mqtt_retry_times));

    public static int GetPoultriesCount()
    {
        int count = -1;
        int index = 0;
        while (!string.IsNullOrEmpty(SettingsDataAccess.GetSection($"poultries:{index}:name").Value))
        {
            index++;
            if (count == -1) count = 1;
            else count++;
        }
        if (count > 0) return count;
        else return 0;
    }

    public static bool PoultriesExist() => GetPoultriesCount() > 0;

    public static PoultrySettingsModel GetPoultrySettings(int index)
    {
        try
        {
            var section = SettingsDataAccess.GetSection($"poultries:{index}");
            var settings = section.Get<PoultrySettingsModel>();
            //settings.api_url += "/";
            return settings;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, $"Error in {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType}");
        }
        return null;
    }
}