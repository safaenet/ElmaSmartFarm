namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class PushButtonSensorModel : SensorModel
    {
        public List<DateTime> Values { get; set; }
        public DateTime? LastTriggerDate => Values?.Max();
        public int? MaximumAllowedTriggerLatency { get; set; } //In Seconds.
        public bool HasAlarm => IsEnabled && LastTriggerDate != null && MaximumAllowedTriggerLatency != null && LastTriggerDate < DateTime.Now.AddSeconds((double)MaximumAllowedTriggerLatency * -1);
        public bool HasIssue => HasError || HasAlarm;
    }
}