namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class SensorModel
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public SensorSection Section { get; set; }
        public SensorType Type { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsInPeriod { get; set; }
        public int BatteryLevel { get; set; }
        public List<SesnorErrorModel> Errors { get; set; }
        public bool HasError => IsEnabled && Errors != null && Errors.Count > 0 && Errors.Any(e => e.DateErased == null);
        public string Descriptions { get; set; }
    }
}