namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class SesnorErrorModel
    {
        public SensorErrorType Type { get; set; }
        public DateTime DateHappened { get; set; }
        public DateTime? DateErased { get; set; }
        public string Descriptions { get; set; }
    }
}