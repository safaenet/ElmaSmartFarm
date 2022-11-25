namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class BinarySensorModel : SensorModel
    {
        public List<SensorReadModel<BinarySensorValueType>> Values { get; set; }
        public BinarySensorValueType? CurrentValue => Values?.MaxBy(t => t.ReadDate)?.Value;
        public DateTime? LastReadDate => Values?.MaxBy(t => t.ReadDate)?.ReadDate;
    }
}