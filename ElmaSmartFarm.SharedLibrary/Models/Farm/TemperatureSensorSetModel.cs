using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class TemperatureSensorSetModel
    {
        public List<TemperatureSensorModel> Sensors { get; set; }
        public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
        public double? MinimumValue => Sensors?.Where(s => s.IsEnabled && !s.HasError)?.Min(t => t.LastRead)?.Value;
        public double? MaximumValue => Sensors?.Where(s => s.IsEnabled && !s.HasError)?.Max(t => t.LastRead)?.Value;
        public double? AverageValue => Sensors?.Where(s => s.IsEnabled && !s.HasError)?.Average(t => t.LastRead.Value);
        public SensorSection? MinimumValueSection => Sensors?.Where(s => s.IsEnabled && !s.HasError)?.MinBy(t => t.LastRead).Section;
        public SensorSection? MaximumValueSection => Sensors?.Where(s => s.IsEnabled && !s.HasError)?.MaxBy(t => t.LastRead).Section;
        public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
    }
}