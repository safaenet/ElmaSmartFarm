namespace ElmaSmartFarm.SharedLibrary.Models.Alarm;

public class AlarmBaseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public AlarmDeviceType DeviceType { get; set; }
    public LocationType LocationType { get; set; }
    /// <summary>
    /// The location (Farm/Poultry) to which this alarm device is attached to. This is needed for mqtt message.
    /// </summary>
    public int LocationId { get; set; }
    public bool IsEnabled { get; set; }
    public string Descriptions { get; set; }
}