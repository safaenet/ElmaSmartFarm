namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class PushButtonSensorModel : SensorModel
    {
        public List<DateTime> PushedDates { get; set; }
        public DateTime? LastPushedDate => PushedDates?.Max();
    }
}