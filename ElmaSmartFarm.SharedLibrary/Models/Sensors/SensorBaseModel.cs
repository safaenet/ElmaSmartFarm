namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class SensorBaseModel
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public bool IsEnabled { get; set; }
        public int BatteryLevel { get; set; }
        public string Descriptions { get; set; }
    }
}