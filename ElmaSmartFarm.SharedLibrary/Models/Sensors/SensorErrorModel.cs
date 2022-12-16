namespace ElmaSmartFarm.SharedLibrary.Models.Sensors;

public class SensorErrorModel : ErrorModel
{
    public int SensorId { get; set; }
    public int LocationId { get; set; }
    public SensorErrorType ErrorType { get; set; }
    public SensorSection Section { get; set; }
}