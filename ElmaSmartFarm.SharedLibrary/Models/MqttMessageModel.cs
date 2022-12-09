namespace ElmaSmartFarm.SharedLibrary.Models;

public class MqttMessageModel
{
    public string Topic { get; set; }
    public string Payload { get; set; }
    public DateTime ReadDate { get; set; }
    public bool Retained { get; set; }
    public int QoS { get; set; }
}