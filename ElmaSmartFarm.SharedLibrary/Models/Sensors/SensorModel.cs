namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class SensorModel
    {
        public int Id { get; set; }
        public int LocationId { get; set; } //FarmId or PoultryId
        public SensorSection Section { get; set; }
        public SensorType Type { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsInPeriod { get; set; }
        public int BatteryLevel { get; set; }
        public DateTime? KeepAliveMessageDate { get; set; }
        public IEnumerable<SensorErrorModel> Errors { get; set; }
        public bool HasError => IsEnabled && Errors != null && Errors.Any(e => e.DateErased == null);
        public string Descriptions { get; set; }
    }
}