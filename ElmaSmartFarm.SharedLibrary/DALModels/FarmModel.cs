namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class FarmModel
    {
        public int Id { get; set; }
        public FarmTemperatureModel Temperature { get; set; }
        public FarmHumidityModel Humidity { get; set; }
        public FarmAmbientLightModel AmbientLight { get; set; }
        public FarmCommuteModel Commute { get; set; }
        public ButtonSensorModel CheckupSensor { get; set; }
        public ButtonSensorModel FeedSensor { get; set; }
        public DateTime? LastCheckupDate => CheckupSensor?.LastReadDate;
        public DateTime? LastFeedDate => FeedSensor?.LastReadDate;
        public ChickenStatisticsModel ChickenStatistics { get; set; }
        public List<FarmAlarmType> Alarms { get; set; }
        public bool HasAlarm => Alarms != null && Alarms.Count > 0;
        public BinarySensorModel ElectricPowerStatus { get; set; }
        public bool IsEnabled { get; set; }
        public string Descriptions { get; set; }
    }
}