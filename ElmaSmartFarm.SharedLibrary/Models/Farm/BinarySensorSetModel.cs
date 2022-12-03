using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class BinarySensorSetModel
    {
        public List<BinarySensorModel> Sensors { get; set; }
        public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
        public IEnumerable<BinarySensorModel> ActiveSensors => Sensors?.Where(s => s.IsEnabled && s.IsWatched && !s.HasError);
        public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
    }
}