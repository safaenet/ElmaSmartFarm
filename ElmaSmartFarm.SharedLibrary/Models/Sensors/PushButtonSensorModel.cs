namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class PushButtonSensorModel : SensorModel
    {
        public List<DateTime> Values { get; set; }
        public DateTime? LastReadDate => Values?.Max();
    }
}