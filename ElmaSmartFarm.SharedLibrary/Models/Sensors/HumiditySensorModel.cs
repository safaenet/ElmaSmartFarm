namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class HumiditySensorModel : SensorModel
    {
        public List<SensorReadModel<int>> Values { get; set; }
        public int? MinimumRecentValue => Values?.Min(t => t.Value);
        public int? MaximumRecentValue => Values?.Max(t => t.Value);
        public int? CurrentValue => Values?.MaxBy(t => t.ReadDate)?.Value;
        public DateTime? LastReadDate => Values?.MaxBy(t => t.ReadDate)?.ReadDate;
        public int Offset { get; set; }
    }
}