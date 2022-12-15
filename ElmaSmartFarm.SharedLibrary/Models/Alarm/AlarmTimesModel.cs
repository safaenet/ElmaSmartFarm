namespace ElmaSmartFarm.SharedLibrary.Models.Alarm;
public class AlarmTimesModel
{
    public int Level { get; set; }
    public bool Enable { get; set; }
    public int FirstTime { get; set; }
    public int Every { get; set; }
    public int Snooze { get; set; }
    public int CountInCycle { get; set; }
}