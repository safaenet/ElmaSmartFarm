﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ElmaSmartFarm.SharedLibrary.Models.Sensors;

public class CommuteSensorModel : SensorModel
{
    public List<CommuteSensorReadModel> Values { get; set; }
    public CommuteSensorReadModel LastRead => Values?.MaxBy(r => r.ReadDate);
    public CommuteSensorReadModel LastStepInSavedRead => Values?.Where(t => t.IsSavedToDb && t.Value == CommuteSensorValueType.StepIn).MaxBy(t => t.ReadDate);
    public CommuteSensorReadModel LastStepOutSavedRead => Values?.Where(t => t.IsSavedToDb && t.Value == CommuteSensorValueType.StepOut).MaxBy(t => t.ReadDate);
    public DateTime? LastCommuteDate => LastRead.ReadDate;
    public DateTime? LastStepInDate => Values?.Where(c => c.Value == CommuteSensorValueType.StepIn).Max(r => r.ReadDate);
    public DateTime? LastStepOutDate => Values?.Where(c => c.Value == CommuteSensorValueType.StepOut).Max(r => r.ReadDate);
}