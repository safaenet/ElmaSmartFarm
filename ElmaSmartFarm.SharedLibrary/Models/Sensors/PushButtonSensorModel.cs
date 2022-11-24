namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class PushButtonSensorModel : SensorBaseModel
    {
        public List<DateTime> ButtonReads { get; set; }
        public DateTime? LastReadDate => ButtonReads?.Max();
    }
}