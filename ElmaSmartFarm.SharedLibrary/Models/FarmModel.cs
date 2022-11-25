using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FarmModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FarmTemperatureModel Temperature { get; set; }
        public FarmHumidityModel Humidity { get; set; }
        public FarmAmbientLightModel AmbientLight { get; set; }
        public FarmCommuteModel Commute { get; set; }
        public PushButtonSensorModel CheckupSensor { get; set; }
        public PushButtonSensorModel FeedSensor { get; set; }
        public BinarySensorModel ElectricPower { get; set; }
        public PeriodModel Period { get; set; }
        public bool IsInPeriod => Period != null && Period.EndDate != null;
        public bool IsEnabled { get; set; }
        public string Descriptions { get; set; }
    }
}