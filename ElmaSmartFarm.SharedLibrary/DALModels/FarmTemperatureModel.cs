namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class FarmTemperatureModel
    {
        public List<TemperatureSensorModel> TemperatureSensors { get; set; }
        public bool HasTemperatureSensors => TemperatureSensors != null && TemperatureSensors.Any(t => t.IsEnabled);
        public double? MinimumTemperature => TemperatureSensors?.Where(s => s.IsEnabled)?.Min(t => t.CurrentTemperature);
        public double? MaximumTemperature => TemperatureSensors?.Where(s => s.IsEnabled)?.Max(t => t.CurrentTemperature);
        public double? AverageTemperature => TemperatureSensors?.Where(s => s.IsEnabled)?.Average(t => t.CurrentTemperature);
        public int? MinimumTemperatureSection => TemperatureSensors?.Where(s => s.IsEnabled)?.MinBy(t => t.CurrentTemperature).Section;
        public int? MaximumTemperatureSection => TemperatureSensors?.Where(s => s.IsEnabled)?.MaxBy(t => t.CurrentTemperature).Section;
    }
}