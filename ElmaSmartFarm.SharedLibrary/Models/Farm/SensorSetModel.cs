using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System.Collections.Generic;
using System.Linq;

namespace ElmaSmartFarm.SharedLibrary.Models;

public class SensorSetModel<T> where T : SensorModel
{
    public List<T> Sensors { get; set; }
    public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
    public IEnumerable<T> EnabledSensors => Sensors?.Where(s => s.IsEnabled);
    public IEnumerable<T> ActiveSensors => EnabledSensors?.Where(s => s.IsWatched);
    public bool HasActiveSensors => ActiveSensors != null && ActiveSensors.Any();
    public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
}