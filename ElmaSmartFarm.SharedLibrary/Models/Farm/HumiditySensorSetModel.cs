using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class HumiditySensorSetModel : SensorSetModel<HumiditySensorModel>
    {
        public int? MinimumValue => ActiveSensors?.Min(t => t.LastRead)?.Value;
        public int? MaximumValue => ActiveSensors?.Max(t => t.LastRead)?.Value;
        public double? AverageValue => ActiveSensors?.Average(t => t.LastRead.Value);
    }
}