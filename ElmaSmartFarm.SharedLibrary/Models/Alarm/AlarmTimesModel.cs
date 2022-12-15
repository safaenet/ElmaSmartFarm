namespace ElmaSmartFarm.SharedLibrary.Models.Alarm;
public class AlarmTimesModel
{
    public int Level { get; set; }
    public bool Enable { get; set; }
    public int RaiseTime { get; set; }
    public bool FarmAlarmEnable { get; set; }
    public int FarmAlarmRaiseTimeOffset { get; set; }
    public int FarmAlarmRaiseTime => RaiseTime + FarmAlarmRaiseTimeOffset;
    public int FarmAlarmEvery { get; set; }
    public int FarmAlarmSnooze { get; set; }
    public int FarmAlarmCountInCycle { get; set; }
    public bool SmsEnable { get; set; }
    public int SmsRaiseTimeOffset { get; set; }
    public int SmsRaiseTime => RaiseTime + SmsRaiseTimeOffset;
    public int SmsEvery { get; set; }
    public int SmsSnooze { get; set; }
    public int SmsCountInCycle { get; set; }
    public bool PoultryAlarmEnable { get; set; }
    public int PoultryAlarmRaiseTimeOffset { get; set; }
    public int PoultryAlarmRaiseTime => RaiseTime + PoultryAlarmRaiseTimeOffset;
    public int PoultryAlarmEvery { get; set; }
    public int PoultryAlarmSnooze { get; set; }
    public int PoultryAlarmCountInCycle { get; set; }
}