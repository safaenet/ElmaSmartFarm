namespace ElmaSmartFarm.SharedLibrary.Models.Sensors;

public class ScalarSensorReadModel : SensorReadModel
{
    public double? Temperature { get; set; }
    public int? Humidity { get; set; }
    public int? Light { get; set; }
    public double? Ammonia { get; set; }
    public double? Co2 { get; set; }
}