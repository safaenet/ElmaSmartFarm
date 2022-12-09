namespace ElmaSmartFarm.SharedLibrary.Models;

public class AlarmModel
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string Name { get; set; }
    public AlarmDeviceType Type { get; set; }
    public bool IsEnabled { get; set; }
    public string Descriptions { get; set; }
}