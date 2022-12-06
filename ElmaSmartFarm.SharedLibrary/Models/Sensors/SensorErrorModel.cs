namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class SensorErrorModel
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public int LocationId { get; set; }
        public SensorSection Section { get; set; }
        public SensorErrorType ErrorType { get; set; }
        public DateTime DateHappened { get; set; }
        public DateTime? DateErased { get; set; }
        public DateTime? DateInformed { get; set; }
        public int InformCount { get; set; }
        public string Descriptions { get; set; }
    }
}