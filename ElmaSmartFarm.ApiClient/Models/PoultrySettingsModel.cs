namespace ElmaSmartFarm.ApiClient.Models;

public class PoultrySettingsModel
{
    public string name { get; set; }
    public string api_url { get; set; }
    public string api_username { get; set; }
    public string api_password { get; set; }
    public string mqtt_address { get; set; }
    public int mqtt_port { get; set; }
    public bool mqtt_authentication { get; set; }
    public string mqtt_username { get; set; }
    public string mqtt_password { get; set; }
}