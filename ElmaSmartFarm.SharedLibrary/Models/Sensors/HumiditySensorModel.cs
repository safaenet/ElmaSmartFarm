namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class HumiditySensorModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Section { get; set; }
        public List<HumidityReadModel> HumidityReads { get; set; }
        public int? MinimumRecentHumidity => HumidityReads?.Min(t => t.Humidity);
        public int? MaximumRecentHumidity => HumidityReads?.Max(t => t.Humidity);
        public int? CurrentHumidity => HumidityReads?.MaxBy(t => t.ReadDate)?.Humidity;
        public DateTime? LastReadDate => HumidityReads?.MaxBy(t => t.ReadDate)?.ReadDate;
        public int Offset { get; set; }
        public bool IsEnabled { get; set; }
        public int BatteryLevel { get; set; }
        public string Descriptions { get; set; }
    }
}