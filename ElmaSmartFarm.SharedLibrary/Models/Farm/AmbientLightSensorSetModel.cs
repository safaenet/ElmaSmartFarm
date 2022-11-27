using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class AmbientLightSensorSetModel
    {
        public List<AmbientLightSensorModel> Sensors { get; set; }
        public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
        public int? MinimumValue => Sensors?.Where(s => s.IsEnabled)?.Min(t => t.CurrentValue);
        public int? MaximumValue => Sensors?.Where(s => s.IsEnabled)?.Max(t => t.CurrentValue);
        public double? AverageValue => Sensors?.Where(s => s.IsEnabled)?.Average(t => t.CurrentValue);
        public SensorSection? MinimumValueSection => Sensors?.Where(s => s.IsEnabled)?.MinBy(t => t.CurrentValue).Section;
        public SensorSection? MaximumValueSection => Sensors?.Where(s => s.IsEnabled)?.MaxBy(t => t.CurrentValue).Section;
        public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
        public bool HasAlarm => HasSensors && Sensors.Any(s => s.HasAlarm);
        public bool HasIssue => HasSensors && Sensors.Any(s => s.HasIssue);
    }
}