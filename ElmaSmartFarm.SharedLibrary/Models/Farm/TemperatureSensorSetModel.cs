using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class TemperatureSensorSetModel
    {
        public IEnumerable<TemperatureSensorModel> Sensors { get; set; }
        public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
        public double? MinimumValue => Sensors?.Where(s => s.IsEnabled)?.Min(t => t.LastRead)?.Value;
        public double? MaximumValue => Sensors?.Where(s => s.IsEnabled)?.Max(t => t.LastRead)?.Value;
        public double? AverageValue => Sensors?.Where(s => s.IsEnabled)?.Average(t => t.LastRead.Value);
        public SensorSection? MinimumValueSection => Sensors?.Where(s => s.IsEnabled)?.MinBy(t => t.LastRead).Section;
        public SensorSection? MaximumValueSection => Sensors?.Where(s => s.IsEnabled)?.MaxBy(t => t.LastRead).Section;
        public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
    }
}