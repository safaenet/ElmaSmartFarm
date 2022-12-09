namespace ElmaSmartFarm.SharedLibrary.Models.Sensors;

public class SensorModel : SensorBaseModel
{
    public string IPAddress { get; set; } = "N/A";
    public int BatteryLevel { get; set; } = -1;
    public DateTime? KeepAliveMessageDate { get; set; }
    public List<SensorErrorModel> Errors { get; set; } = new();
    public SensorErrorModel LastError => Errors?.MaxBy(x => x.DateHappened);
    public IEnumerable<SensorErrorModel> ActiveErrors => Errors.Where(e => e.DateErased == null);
    public bool HasError => IsEnabled && ActiveErrors.Any();
    public bool IsInPeriod { get; set; }
    public int WatchStartDay { get; set; }
    public bool IsWatched { get; set; } = true;
}