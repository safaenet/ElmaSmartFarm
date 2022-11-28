namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FarmModel
    {
        public int Id { get; set; }
        public int PoultryId { get; set; }
        public string Name { get; set; }
        public int FarmNumber { get; set; }
        public int MaxCapacity { get; set; }
        public TemperatureSensorSetModel Temperatures { get; set; }
        public HumiditySensorSetModel Humidities { get; set; }
        public AmbientLightSensorSetModel AmbientLights { get; set; }
        public CommuteSensorSetModel Commutes { get; set; }
        public PushButtonSensorSetModel Checkups { get; set; }
        public PushButtonSensorSetModel Feeds { get; set; }
        public BinarySensorSetModel ElectricPowers { get; set; }
        public PeriodModel Period { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsInPeriod => IsEnabled && Period != null && Period.EndDate != null;
        public List<FarmInPeriodErrorModel> InPeriodErrors { get; set; }
        public bool HasSensorError => IsEnabled && ((Temperatures != null && Temperatures.HasError) || (Humidities != null && Humidities.HasError) || (AmbientLights != null && AmbientLights.HasError) || (Commutes != null && Commutes.HasError) || (Checkups != null && Checkups.HasError) || (Feeds != null && Feeds.HasError) || (ElectricPowers != null && ElectricPowers.HasError));
        public string Descriptions { get; set; }
    }
}