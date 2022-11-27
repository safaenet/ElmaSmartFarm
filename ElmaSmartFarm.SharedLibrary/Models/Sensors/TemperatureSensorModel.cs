namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class TemperatureSensorModel : SensorModel
    {
        public List<SensorReadModel<double>> Values { get; set; }
        public double? CurrentValue => Values?.MaxBy(t => t.ReadDate)?.Value;
        public DateTime? LastReadDate => Values?.MaxBy(t => t.ReadDate)?.ReadDate;
        public double? MinimumAllowedValue { get; set; }
        public double? MaximumAllowedValue { get; set; }
        public bool HasAlarm => IsEnabled && CurrentValue != null && ((MaximumAllowedValue != null && CurrentValue > MaximumAllowedValue) || (MinimumAllowedValue != null && CurrentValue < MinimumAllowedValue));
        public bool HasIssue => HasError || HasAlarm;
        public double Offset { get; set; }
        public bool IsWatched { get; set; }
    }
}