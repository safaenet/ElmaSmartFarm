namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class PushButtonSensorModel : SensorModel
    {
        public List<PushButtonSensorReadModel> Values { get; set; }
        public PushButtonSensorReadModel LastRead => Values?.MaxBy(t => t.ReadDate);
        public PushButtonSensorReadModel LastSavedRead => Values?.Where(t => t.IsSavedToDb).MaxBy(t => t.ReadDate);
    }
}