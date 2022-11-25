using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FarmTemperatureModel
    {
        public List<TemperatureSensorModel> TemperatureSensors { get; set; }
        public bool HasTemperatureSensors => TemperatureSensors != null && TemperatureSensors.Any(t => t.IsEnabled);
        public double? MinimumTemperature => TemperatureSensors?.Where(s => s.IsEnabled)?.Min(t => t.CurrentValue);
        public double? MaximumTemperature => TemperatureSensors?.Where(s => s.IsEnabled)?.Max(t => t.CurrentValue);
        public double? AverageTemperature => TemperatureSensors?.Where(s => s.IsEnabled)?.Average(t => t.CurrentValue);
        public int? MinimumTemperatureSection => TemperatureSensors?.Where(s => s.IsEnabled)?.MinBy(t => t.CurrentValue).Section;
        public int? MaximumTemperatureSection => TemperatureSensors?.Where(s => s.IsEnabled)?.MaxBy(t => t.CurrentValue).Section;
    }
}