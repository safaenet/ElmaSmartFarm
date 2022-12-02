namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class SensorModel : SensorBaseModel
    {
        public string IPAddress { get; set; } = "N/A";
        public bool IsInPeriod { get; set; }
        public int BatteryLevel { get; set; } = -1;
        public DateTime? KeepAliveMessageDate { get; set; }
        public List<SensorErrorModel> Errors { get; set; } = new();
        public SensorErrorModel LastError => Errors?.MaxBy(x => x.DateHappened);
        public bool HasError => IsEnabled && Errors != null && Errors.Any(e => e.DateErased == null);
        public bool IsWatched { get; set; } = true;
    }
}