namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class TemperatureSensorModel : SensorModel
    {
        public List<SensorReadModel<double>> Values { get; set; }
        public double? MinimumRecentValue => Values?.Min(t => t.Value);
        public double? MaximumRecentValue => Values?.Max(t => t.Value);
        public double? CurrentValue => Values?.MaxBy(t => t.ReadDate)?.Value;
        public DateTime? LastReadDate => Values?.MaxBy(t => t.ReadDate)?.ReadDate;
        public double Offset { get; set; }
        public bool IsWatched { get; set; }
    }
}