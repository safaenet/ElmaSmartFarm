namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class AmbientLightSensorModel : SensorModel
    {
        public List<SensorReadModel<int>> Values { get; set; }
        public SensorReadModel<int> LastValue => Values?.MaxBy(t => t.ReadDate);
        public DateTime? LastReadDate => Values?.MaxBy(t => t.ReadDate)?.ReadDate;
        public int Offset { get; set; }
        public bool IsWatched { get; set; }
    }
}