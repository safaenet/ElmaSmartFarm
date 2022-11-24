using ElmaSmartFarm.SharedLibrary.Models.Sensors.ReadModels;

namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class HumiditySensorModel : SensorBaseModel
    {
        public int Section { get; set; }
        public List<HumidityReadModel> HumidityReads { get; set; }
        public int? MinimumRecentHumidity => HumidityReads?.Min(t => t.Humidity);
        public int? MaximumRecentHumidity => HumidityReads?.Max(t => t.Humidity);
        public int? CurrentHumidity => HumidityReads?.MaxBy(t => t.ReadDate)?.Humidity;
        public DateTime? LastReadDate => HumidityReads?.MaxBy(t => t.ReadDate)?.ReadDate;
        public int Offset { get; set; }
    }
}