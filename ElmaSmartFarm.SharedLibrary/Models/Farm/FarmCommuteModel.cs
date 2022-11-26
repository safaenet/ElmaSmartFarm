﻿using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FarmCommuteModel
    {
        public List<CommuteSensorModel> Sensors { get; set; }
        public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
        public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
    }
}