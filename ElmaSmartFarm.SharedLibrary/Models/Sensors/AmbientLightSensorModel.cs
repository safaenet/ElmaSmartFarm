namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class AmbientLightSensorModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Section { get; set; }
        public List<AmbientLightReadModel> AmbientLightReads { get; set; }
        public int? MinimumRecentAmbientLight => AmbientLightReads?.Min(t => t.AmbientLight);
        public int? MaximumRecentAmbientLight => AmbientLightReads?.Max(t => t.AmbientLight);
        public int? CurrentAmbientLight => AmbientLightReads?.MaxBy(t => t.ReadDate)?.AmbientLight;
        public DateTime? LastReadDate => AmbientLightReads?.MaxBy(t => t.ReadDate)?.ReadDate;
        public bool IsEnabled { get; set; }
        public int BatteryLevel { get; set; }
        public string Descriptions { get; set; }
    }
}