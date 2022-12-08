using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models;

public class SensorSetModel<T> where T : SensorModel
{
    public List<T> Sensors { get; set; }
    public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
    public IEnumerable<T> ActiveSensors => Sensors?.Where(s => s.IsEnabled && s.IsWatched);
    public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
}