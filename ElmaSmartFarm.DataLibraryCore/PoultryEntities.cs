using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System;
using System.Collections.Generic;

namespace ElmaSmartFarm.DataLibraryCore;

public class PoultryEntities
{
    public PoultryModel Poultry { get; set; }
    public List<MqttMessageModel> UnknownMqttMessages { get; set; } = new();
    public List<SensorErrorModel> AlarmableSensorErrors { get; set; } = new();
    public List<FarmInPeriodErrorModel> AlarmableFarmPeriodErrors { get; set; } = new();
    public List<PoultryInPeriodErrorModel> AlarmablePoultryPeriodErrors { get; set; } = new();
    public DateTime SystemStartUpTime { get; set; }
}