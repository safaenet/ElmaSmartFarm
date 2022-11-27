using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class PushButtonSensorSetModel
    {
        public List<PushButtonSensorModel> Sensors { get; set; }
        public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
        public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
        public bool HasAlarm => HasSensors && Sensors.Any(s => s.HasAlarm);
        public bool HasIssue => HasSensors && Sensors.Any(s => s.HasIssue);
    }
}