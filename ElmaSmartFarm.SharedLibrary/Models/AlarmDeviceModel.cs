namespace ElmaSmartFarm.SharedLibrary.Models;

public class AlarmDeviceModel
{
    public int Id { get; set; }
    public AlarmDeviceType Type { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public string Descriptions { get; set; }
}