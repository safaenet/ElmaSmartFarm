namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class FarmAmbientLightModel
    {
        public List<AmbientLightSensorModel> AmbientLightSensors { get; set; }
        public bool HasAmbientLightSensors => AmbientLightSensors != null && AmbientLightSensors.Any(t => t.IsEnabled);
        public int? MinimumAmbientLight => AmbientLightSensors?.Where(s => s.IsEnabled)?.Min(t => t.CurrentAmbientLight);
        public int? MaximumAmbientLight => AmbientLightSensors?.Where(s => s.IsEnabled)?.Max(t => t.CurrentAmbientLight);
        public double? AverageTemperature => AmbientLightSensors?.Where(s => s.IsEnabled)?.Average(t => t.CurrentAmbientLight);
        public int? MinimumAmbientLightSection => AmbientLightSensors?.Where(s => s.IsEnabled)?.MinBy(t => t.CurrentAmbientLight).Section;
        public int? MaximumAmbientLightSection => AmbientLightSensors?.Where(s => s.IsEnabled)?.MaxBy(t => t.CurrentAmbientLight).Section;
    }
}