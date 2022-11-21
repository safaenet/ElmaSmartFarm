namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class FarmModel
    {
        public int Id { get; set; }
        public List<TemperatureSensorModel> TemperatureSensors { get; set; }
        public List<HumiditySensorModel> HumiditySensors { get; set; }
        public List<AmbientLightSensorModel> AmbientLightSensors { get; set; }
        public ButtonSensorModel CheckupSensor { get; set; }
        public ButtonSensorModel FeedSensor { get; set; }
        public DateTime? LastCheckupDate => CheckupSensor?.LastReadDate;
        public DateTime? LastFeedDate => FeedSensor?.LastReadDate;
        public List<FarmAlarmType> Alarms { get; set; }
        public bool HasAlarm => Alarms != null && Alarms.Count > 0;
        public bool IsEnabled { get; set; }
        public string Descriptions { get; set; }
    }
}