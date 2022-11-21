using System.Linq;
namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class TemperatureSensorModel
    {
        public int Id { get; set; }
        public List<TemperatureReadModel> TemperatureReads { get; set; }
        public double? CurrentTemperature => TemperatureReads?.MaxBy(t => t.ReadDate)?.Celsius;
        public DateTime? LastReadDate => TemperatureReads?.MaxBy(t => t.ReadDate)?.ReadDate;
        public double Offset { get; set; }
        public bool IsEnabled { get; set; }
        public string Descriptions { get; set; }
    }
}