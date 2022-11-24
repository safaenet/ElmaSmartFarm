using ElmaSmartFarm.SharedLibrary.Models.Sensors.ReadModels;

namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class AmbientLightSensorModel : SensorBaseModel
    {
        public int Section { get; set; }
        public List<AmbientLightReadModel> AmbientLightReads { get; set; }
        public int? MinimumRecentAmbientLight => AmbientLightReads?.Min(t => t.AmbientLight);
        public int? MaximumRecentAmbientLight => AmbientLightReads?.Max(t => t.AmbientLight);
        public int? CurrentAmbientLight => AmbientLightReads?.MaxBy(t => t.ReadDate)?.AmbientLight;
        public DateTime? LastReadDate => AmbientLightReads?.MaxBy(t => t.ReadDate)?.ReadDate;
    }
}