using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System;
using System.Linq;

namespace ElmaSmartFarm.SharedLibrary.Models;

public class CommuteSensorSetModel : SensorSetModel<CommuteSensorModel>
{
    public CommuteSensorModel LastReadSensor => ActiveSensors.MaxBy(s => s.LastRead.ReadDate);
    public DateTime? LastStepInDate => Sensors.Max(s => s.LastStepInDate);
    public DateTime? LastStepOutDate => Sensors.Max(s => s.LastStepOutDate);
}