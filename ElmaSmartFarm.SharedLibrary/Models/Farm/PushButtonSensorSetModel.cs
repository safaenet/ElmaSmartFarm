using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System.Linq;

namespace ElmaSmartFarm.SharedLibrary.Models;

public class PushButtonSensorSetModel : SensorSetModel<PushButtonSensorModel>
{
    public PushButtonSensorModel LastReadSensor => ActiveSensors?.MaxBy(s => s.LastRead.ReadDate);
}