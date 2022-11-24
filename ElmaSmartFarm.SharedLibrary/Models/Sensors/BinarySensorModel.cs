using ElmaSmartFarm.SharedLibrary.Models.Sensors.ReadModels;

namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class BinarySensorModel : SensorBaseModel
    {
        public BinarySensorType Type { get; set; }
        public List<BinarySensorReadModel> BinaryReads { get; set; }
        public bool? CurrentStatus => BinaryReads?.MaxBy(t => t.ReadDate)?.Status;
        public DateTime? LastReadDate => BinaryReads?.MaxBy(t => t.ReadDate)?.ReadDate;
    }
}