namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class TemperatureSensorModel
    {
        public int Id { get; set; }
        public int Section { get; set; }
        public List<TemperatureReadModel> TemperatureReads { get; set; }
        public double? MinimumRecentTemperature => TemperatureReads?.Min(t => t.Celsius);
        public double? MaximumRecentTemperature => TemperatureReads?.Max(t => t.Celsius);
        public double? CurrentTemperature => TemperatureReads?.MaxBy(t => t.ReadDate)?.Celsius;
        public DateTime? LastReadDate => TemperatureReads?.MaxBy(t => t.ReadDate)?.ReadDate;
        public double Offset { get; set; }
        public bool IsEnabled { get; set; }
        public int BatteryLevel { get; set; }
        public string Descriptions { get; set; }
    }
}