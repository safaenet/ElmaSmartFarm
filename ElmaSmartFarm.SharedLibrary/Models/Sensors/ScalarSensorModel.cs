namespace ElmaSmartFarm.SharedLibrary.Models.Sensors;

public class ScalarSensorModel : SensorModel
{
    public List<ScalarSensorReadModel> Values { get; set; }
    public ScalarSensorReadModel LastRead => Values?.MaxBy(t => t.ReadDate);
    public ScalarSensorReadModel LastSavedRead => Values?.Where(t => t.IsSavedToDb).MaxBy(t => t.ReadDate);
    public double TemperatureOffset { get; set; }
    public int HumidityOffset { get; set; }
    public int LightOffset { get; set; }
    public double AmmoniaOffset { get; set; }
    public double Co2Offset { get; set; }
}