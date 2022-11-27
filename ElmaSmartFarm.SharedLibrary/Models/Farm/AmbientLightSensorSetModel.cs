using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class AmbientLightSensorSetModel
    {
        public List<AmbientLightSensorModel> Sensors { get; set; }
        public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
        public int? MinimumValue => Sensors?.Where(s => s.IsEnabled)?.Min(t => t.LastValue)?.Value;
        public int? MaximumValue => Sensors?.Where(s => s.IsEnabled)?.Max(t => t.LastValue)?.Value;
        public double? AverageValue => Sensors?.Where(s => s.IsEnabled)?.Average(t => t.LastValue.Value);
        public SensorSection? MinimumValueSection => Sensors?.Where(s => s.IsEnabled)?.MinBy(t => t.LastValue).Section;
        public SensorSection? MaximumValueSection => Sensors?.Where(s => s.IsEnabled)?.MaxBy(t => t.LastValue).Section;
        public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
    }
}