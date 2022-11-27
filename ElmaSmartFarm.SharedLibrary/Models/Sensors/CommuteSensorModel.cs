namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class CommuteSensorModel : SensorModel
    {
        public List<SensorReadModel<CommuteSensorValueType>> Values { get; set; }
        public DateTime? LastStepInDate => Values?.Where(c => c.Value == CommuteSensorValueType.StepIn).MaxBy(r => r.ReadDate)?.ReadDate;
        public DateTime? LastStepOutDate => Values?.Where(c => c.Value == CommuteSensorValueType.StepOut).MaxBy(r => r.ReadDate)?.ReadDate;
        public SensorReadModel<CommuteSensorValueType> LastRead => Values?.MaxBy(r => r.ReadDate);
        public double? MaximumAllowedTriggerLatency { get; set; } //In Seconds.
        public bool HasAlarm => IsEnabled && LastRead != null && MaximumAllowedTriggerLatency != null && LastRead.ReadDate < DateTime.Now.AddSeconds((double)MaximumAllowedTriggerLatency * -1);
        public bool HasIssue => HasError || HasAlarm;
    }
}