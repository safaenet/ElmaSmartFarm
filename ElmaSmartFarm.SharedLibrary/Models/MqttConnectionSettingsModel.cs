namespace ElmaSmartFarm.SharedLibrary.Models;

public class MqttConnectionSettingsModel
{
    public string mqtt_address { get; set; }
    public int mqtt_port { get; set; }
    public bool mqtt_authentication { get; set; }
    public string mqtt_username { get; set; }
    public string mqtt_password { get; set; }
    public string mqtt_subscribe_topic { get; set; }
    public string mqtt_request_poultry_topic { get; set; }
}