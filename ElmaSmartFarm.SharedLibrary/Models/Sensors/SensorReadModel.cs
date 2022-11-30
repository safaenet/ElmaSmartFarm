namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class SensorReadModel<T>
    {
        public DateTime ReadDate { get; set; }
        public T Value { get; set; }
        public bool IsSavedToDb { get; set; }
    }
}