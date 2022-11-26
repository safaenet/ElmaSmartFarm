using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FarmTemperatureModel
    {
        public List<TemperatureSensorModel> Sensors { get; set; }
        public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
        public double? MinimumValue => Sensors?.Where(s => s.IsEnabled)?.Min(t => t.CurrentValue);
        public double? MaximumValue => Sensors?.Where(s => s.IsEnabled)?.Max(t => t.CurrentValue);
        public double? AverageValue => Sensors?.Where(s => s.IsEnabled)?.Average(t => t.CurrentValue);
        public SensorSection? MinimumValueSection => Sensors?.Where(s => s.IsEnabled)?.MinBy(t => t.CurrentValue).Section;
        public SensorSection? MaximumValueSection => Sensors?.Where(s => s.IsEnabled)?.MaxBy(t => t.CurrentValue).Section;
        public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
    }
}