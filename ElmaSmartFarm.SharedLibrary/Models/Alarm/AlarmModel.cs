namespace ElmaSmartFarm.SharedLibrary.Models.Alarm;

public class AlarmModel : AlarmBaseModel
{
    public bool IsActive { get; set; }
    public bool IsSnoozed { get; set; }
    public DateTime? SnoozedTime { get; set; }
    public int TriggeredCount { get; set; }
}