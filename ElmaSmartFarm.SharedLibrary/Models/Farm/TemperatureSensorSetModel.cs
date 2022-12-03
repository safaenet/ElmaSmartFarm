using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class TemperatureSensorSetModel
    {
        public List<TemperatureSensorModel> Sensors { get; set; }
        public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
        public IEnumerable<TemperatureSensorModel> ActiveSensors => Sensors?.Where(s => s.IsEnabled && s.IsWatched && !s.HasError);
        public double? MinimumValue => ActiveSensors?.Min(t => t.LastRead)?.Value;
        public double? MaximumValue => ActiveSensors?.Max(t => t.LastRead)?.Value;
        public double? AverageValue => ActiveSensors?.Average(t => t.LastRead.Value);
        public SensorSection? MinimumValueSection => ActiveSensors?.MinBy(t => t.LastRead).Section;
        public SensorSection? MaximumValueSection => ActiveSensors?.MaxBy(t => t.LastRead).Section;
        public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
    }
}