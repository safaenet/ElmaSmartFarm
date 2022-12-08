namespace ElmaSmartFarm.SharedLibrary.Models.Sensors;

public class SensorBaseModel
{
    public int Id { get; set; }
    public SensorType Type { get; set; }
    public string Name { get; set; }
    public int LocationId { get; set; } //FarmId or PoultryId
    public SensorSection Section { get; set; }
    public bool IsEnabled { get; set; }
    public string Descriptions { get; set; }
}