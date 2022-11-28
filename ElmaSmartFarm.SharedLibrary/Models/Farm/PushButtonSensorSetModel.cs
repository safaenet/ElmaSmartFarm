﻿using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class PushButtonSensorSetModel
    {
        public IEnumerable<PushButtonSensorModel> Sensors { get; set; }
        public bool HasSensors => Sensors != null && Sensors.Any(t => t.IsEnabled);
        public bool HasError => HasSensors && Sensors.Any(s => s.HasError);
    }
}