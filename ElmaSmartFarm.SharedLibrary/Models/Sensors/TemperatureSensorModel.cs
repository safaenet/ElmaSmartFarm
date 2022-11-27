namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class TemperatureSensorModel : SensorModel
    {
        public List<SensorReadModel<double>> Values { get; set; }
        public SensorReadModel<double> LastRead => Values?.MaxBy(t => t.ReadDate);
        public DateTime? LastReadDate => Values?.MaxBy(t => t.ReadDate)?.ReadDate;
        public double Offset { get; set; }
        public bool IsWatched { get; set; }
    }
}