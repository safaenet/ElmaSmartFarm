namespace ElmaSmartFarm.SharedLibrary.Models.Alarm;

public class AlarmBaseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public AlarmDeviceType Type { get; set; }
    public int LocationId { get; set; }
    public bool IsEnabled { get; set; }
    public string Descriptions { get; set; }
}