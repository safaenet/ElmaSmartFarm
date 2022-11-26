using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FarmHumidityModel
    {
        public List<HumiditySensorModel> HumiditySensors { get; set; }
        public bool HasHumiditySensors => HumiditySensors != null && HumiditySensors.Any(t => t.IsEnabled);
        public int? MinimumHumidity => HumiditySensors?.Where(s => s.IsEnabled)?.Min(t => t.CurrentValue);
        public int? MaximumHumidity => HumiditySensors?.Where(s => s.IsEnabled)?.Max(t => t.CurrentValue);
        public double? AverageHumidity => HumiditySensors?.Where(s => s.IsEnabled)?.Average(t => t.CurrentValue);
        public SensorSection? MinimumHumiditySection => HumiditySensors?.Where(s => s.IsEnabled)?.MinBy(t => t.CurrentValue).Section;
        public SensorSection? MaximumHumiditySection => HumiditySensors?.Where(s => s.IsEnabled)?.MaxBy(t => t.CurrentValue).Section;
    }
}