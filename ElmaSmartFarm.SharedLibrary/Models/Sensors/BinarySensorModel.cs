namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class BinarySensorModel : SensorModel
    {
        public IEnumerable<SensorReadModel<BinarySensorValueType>> Values { get; set; }
        public BinarySensorValueType? LastValue => Values?.MaxBy(t => t.ReadDate)?.Value;
        public DateTime? LastReadDate => Values?.MaxBy(t => t.ReadDate)?.ReadDate;
    }
}