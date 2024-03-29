﻿namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class CommuteSensorModel : SensorModel
    {
        public List<SensorReadModel<CommuteSensorValueType>> Values { get; set; }
        public SensorReadModel<CommuteSensorValueType> LastRead => Values?.MaxBy(r => r.ReadDate);
        public SensorReadModel<CommuteSensorValueType> LastStepInSavedRead => Values?.Where(t => t.IsSavedToDb && t.Value == CommuteSensorValueType.StepIn).MaxBy(t => t.ReadDate);
        public SensorReadModel<CommuteSensorValueType> LastStepOutSavedRead => Values?.Where(t => t.IsSavedToDb && t.Value == CommuteSensorValueType.StepOut).MaxBy(t => t.ReadDate);
        public DateTime? LastStepInDate => Values?.Where(c => c.Value == CommuteSensorValueType.StepIn).MaxBy(r => r.ReadDate)?.ReadDate;
        public DateTime? LastStepOutDate => Values?.Where(c => c.Value == CommuteSensorValueType.StepOut).MaxBy(r => r.ReadDate)?.ReadDate;
    }
}