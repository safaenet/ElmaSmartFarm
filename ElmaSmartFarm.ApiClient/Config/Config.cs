using Serilog;

namespace ElmaSmartFarm.ApiClient.Config;

public static class Config
{
    public static bool verbose_mode => SettingsDataAccess.GetValue<bool>("verbose_mode");
    public static int mqtt_retry_interval => SettingsDataAccess.GetValue<int>("mqtt_retry_interval");
    public static int mqtt_retry_times => SettingsDataAccess.GetValue<int>("mqtt_retry_times");

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
            var result = new PoultrySettingsModel()
            {
                name = SettingsDataAccess.GetSection($"poultries:{index}:name").Value,
                api_url = SettingsDataAccess.GetSection($"poultries:{index}:api_url").Value,
                api_username = SettingsDataAccess.GetSection($"poultries:{index}:api_username").Value,
                api_password = SettingsDataAccess.GetSection($"poultries:{index}:api_password").Value,
                mqtt_address = SettingsDataAccess.GetSection($"poultries:{index}:mqtt_address").Value,
                mqtt_port = int.Parse(SettingsDataAccess.GetSection($"poultries:{index}:mqtt_port").Value),
                mqtt_authentication = bool.Parse(SettingsDataAccess.GetSection($"poultries:{index}:mqtt_authentication").Value),
                mqtt_username = SettingsDataAccess.GetSection($"poultries:{index}:mqtt_username").Value,
                mqtt_password = SettingsDataAccess.GetSection($"poultries:{index}:mqtt_password").Value
            };
            return result;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "Error in Config");
        }
        return null;
    }
}