namespace ElmaSmartFarm.SharedLibrary.Models.Alarm;
public class AlarmTimesModel
{
    public int Level { get; set; }
    /// <summary>
    /// Indicates whether alarm is enabled for this Level.
    /// </summary>
    public bool Enable { get; set; }
    public int RaiseTime { get; set; }
    /// <summary>
    /// Indicates whether farm alarm is enabled for this Level.
    /// </summary>
    public bool FarmAlarmEnable { get; set; }
    public int FarmAlarmRaiseTimeOffset { get; set; }
    public int FarmAlarmRaiseTime => RaiseTime + FarmAlarmRaiseTimeOffset;
    public int FarmAlarmEvery { get; set; }
    public int FarmAlarmSnooze { get; set; }
    public int FarmAlarmCountInCycle { get; set; }
    /// <summary>
    /// Indicates whether sms alarm is enabled for this Level.
    /// </summary>
    public bool SmsEnable { get; set; }
    public int SmsRaiseTimeOffset { get; set; }
    public int SmsRaiseTime => RaiseTime + SmsRaiseTimeOffset;
    public int SmsEvery { get; set; }
    public int SmsSnooze { get; set; }
    public int SmsCountInCycle { get; set; }
    /// <summary>
    /// Indicates whether poultry light alarm is enabled for this Level.
    /// </summary>
    public bool PoultryLightAlarmEnable { get; set; }
    public int PoultryLightAlarmRaiseTimeOffset { get; set; }
    public int PoultryLightAlarmRaiseTime => RaiseTime + PoultryLightAlarmRaiseTimeOffset;
    public int PoultryLightAlarmEvery { get; set; }
    public int PoultryLightAlarmSnooze { get; set; }
    public int PoultryLightAlarmCountInCycle { get; set; }
    /// <summary>
    /// Indicates whether poultry siren alarm is enabled for this Level.
    /// </summary>
    public bool PoultrySirenAlarmEnable { get; set; }
    public int PoultrySirenAlarmRaiseTimeOffset { get; set; }
    public int PoultrySirenAlarmRaiseTime => RaiseTime + PoultrySirenAlarmRaiseTimeOffset;
    public int PoultrySirenAlarmEvery { get; set; }
    public int PoultrySirenAlarmSnooze { get; set; }
    public int PoultrySirenAlarmCountInCycle { get; set; }
}