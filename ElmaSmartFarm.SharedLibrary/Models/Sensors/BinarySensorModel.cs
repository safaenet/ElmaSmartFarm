namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class BinarySensorModel : SensorModel
    {
        public List<SensorReadModel<BinarySensorValueType>> Values { get; set; }
        public SensorReadModel<BinarySensorValueType> LastRead => Values?.MaxBy(t => t.ReadDate);
        public SensorReadModel<BinarySensorValueType> LastSavedRead => Values?.Where(t => t.IsSavedToDb).MaxBy(t => t.ReadDate);
    }
}