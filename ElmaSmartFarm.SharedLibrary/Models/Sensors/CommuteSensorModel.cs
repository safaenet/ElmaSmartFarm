﻿namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class CommuteSensorModel : SensorModel
    {
        public IEnumerable<SensorReadModel<CommuteSensorValueType>> Values { get; set; }
        public SensorReadModel<CommuteSensorValueType> LastRead => Values?.MaxBy(r => r.ReadDate);
        public DateTime? LastStepInDate => Values?.Where(c => c.Value == CommuteSensorValueType.StepIn).MaxBy(r => r.ReadDate)?.ReadDate;
        public DateTime? LastStepOutDate => Values?.Where(c => c.Value == CommuteSensorValueType.StepOut).MaxBy(r => r.ReadDate)?.ReadDate;
    }
}