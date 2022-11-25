namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class SensorModel
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public SensorType Type { get; set; }
        public string Name { get; set; }
        public int Section { get; set; }
        public string IPAddress { get; set; }
        public bool IsEnabled { get; set; }
        public int BatteryLevel { get; set; }
        public List<SensorErrorType> Errors { get; set; }
        public string Descriptions { get; set; }
    }
}