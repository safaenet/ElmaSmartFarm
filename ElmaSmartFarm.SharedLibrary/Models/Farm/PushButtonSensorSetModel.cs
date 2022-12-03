using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class PushButtonSensorSetModel
    {
        public List<PushButtonSensorModel> Sensors { get; set; }
        public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
        public IEnumerable<PushButtonSensorModel> ActiveSensors => Sensors?.Where(s => s.IsEnabled && s.IsWatched && !s.HasError);
        public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
    }
}