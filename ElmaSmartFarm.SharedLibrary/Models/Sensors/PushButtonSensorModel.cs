namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class PushButtonSensorModel : SensorModel
    {
        public List<SensorReadModel<DateTime>> Values { get; set; }
        public SensorReadModel<DateTime> LastRead => Values?.MaxBy(t => t.ReadDate);
        public SensorReadModel<DateTime> LastSavedRead => Values?.Where(t => t.IsSavedToDb).MaxBy(t => t.ReadDate);
    }
}