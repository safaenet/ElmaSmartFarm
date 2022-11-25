using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FarmAmbientLightModel
    {
        public List<AmbientLightSensorModel> AmbientLightSensors { get; set; }
        public bool HasAmbientLightSensors => AmbientLightSensors != null && AmbientLightSensors.Any(t => t.IsEnabled);
        public int? MinimumAmbientLight => AmbientLightSensors?.Where(s => s.IsEnabled)?.Min(t => t.CurrentValue);
        public int? MaximumAmbientLight => AmbientLightSensors?.Where(s => s.IsEnabled)?.Max(t => t.CurrentValue);
        public double? AverageTemperature => AmbientLightSensors?.Where(s => s.IsEnabled)?.Average(t => t.CurrentValue);
        public int? MinimumAmbientLightSection => AmbientLightSensors?.Where(s => s.IsEnabled)?.MinBy(t => t.CurrentValue).Section;
        public int? MaximumAmbientLightSection => AmbientLightSensors?.Where(s => s.IsEnabled)?.MaxBy(t => t.CurrentValue).Section;
    }
}