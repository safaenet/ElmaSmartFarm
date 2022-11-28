namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class PushButtonSensorModel : SensorModel
    {
        public IEnumerable<DateTime> PushedDates { get; set; }
        public DateTime? LastPushedDate => PushedDates?.Max();
    }
}