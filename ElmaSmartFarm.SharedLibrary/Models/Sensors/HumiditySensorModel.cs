﻿namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class HumiditySensorModel : SensorModel
    {
        public List<SensorReadModel<int>> Values { get; set; }
        public int? MinimumRecentValue => Values?.Min(t => t.Value);
        public int? MaximumRecentValue => Values?.Max(t => t.Value);
        public int? CurrentValue => Values?.MaxBy(t => t.ReadDate)?.Value;
        public DateTime? LastReadDate => Values?.MaxBy(t => t.ReadDate)?.ReadDate;
        public int? MinimumAllowedValue { get; set; }
        public int? MaximumAllowedValue { get; set; }
        public bool HasAlarm => IsEnabled && CurrentValue != null && ((MaximumAllowedValue != null && CurrentValue > MaximumAllowedValue) || (MinimumAllowedValue != null && CurrentValue < MinimumAllowedValue));
        public bool HasIssue => HasError || HasAlarm;
        public int Offset { get; set; }
        public bool IsWatched { get; set; }
    }
}