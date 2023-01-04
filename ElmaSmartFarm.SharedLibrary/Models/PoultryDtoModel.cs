using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System;
using System.Collections.Generic;

namespace ElmaSmartFarm.SharedLibrary.Models;

public class PoultryDtoModel
{
    public PoultryModel Poultry { get; set; }
    public List<MqttMessageModel> UnknownMqttMessages { get; set; }
    public List<SensorErrorModel> AlarmableSensorErrors { get; set; }
    public List<FarmInPeriodErrorModel> AlarmableFarmPeriodErrors { get; set; }
    public List<PoultryInPeriodErrorModel> AlarmablePoultryPeriodErrors { get; set; }
    public DateTime SystemUpTime { get; set; }
}