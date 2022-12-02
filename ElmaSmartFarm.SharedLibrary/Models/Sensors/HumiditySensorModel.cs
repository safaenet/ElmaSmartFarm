namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class HumiditySensorModel : SensorModel
    {
        public List<SensorReadModel<int>> Values { get; set; }
        public SensorReadModel<int> LastRead => Values?.MaxBy(t => t.ReadDate);
        public SensorReadModel<int> LastSavedRead => Values?.Where(t => t.IsSavedToDb).MaxBy(t => t.ReadDate);
        public int Offset { get; set; }
        public bool IsWatched { get; set; }
    }
}