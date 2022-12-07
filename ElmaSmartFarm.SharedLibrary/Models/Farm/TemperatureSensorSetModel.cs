using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class TemperatureSensorSetModel : SensorSetModel<TemperatureSensorModel>
    {
        public double? MinimumValue => ActiveSensors?.Min(t => t.LastRead)?.Value;
        public double? MaximumValue => ActiveSensors?.Max(t => t.LastRead)?.Value;
        public double? AverageValue => ActiveSensors?.Average(t => t.LastRead.Value);
    }
}