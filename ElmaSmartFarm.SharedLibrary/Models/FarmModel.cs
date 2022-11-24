namespace ElmaSmartFarm.SharedLibrary.DALModels
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
        public ChickenStatisticsModel ChickenStatistics { get; set; }
        public BinarySensorModel ElectricPower { get; set; }
        public List<FarmAlarmType> Alarms { get; set; }
        public bool HasAlarm => Alarms != null && Alarms.Count > 0;
        public bool IsEnabled { get; set; }
        public string Descriptions { get; set; }
    }
}