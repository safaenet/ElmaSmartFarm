namespace ElmaSmartFarm.SharedLibrary.Models.Sensors;

public class SensorErrorModel
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public int LocationId { get; set; }
    public SensorSection Section { get; set; }
    public SensorErrorType ErrorType { get; set; }
    public DateTime DateHappened { get; set; }
    public DateTime? DateErased { get; set; }
    public DateTime? DateAlarmRaised { get; set; }
    public DateTime? DateFarmAlarmRaised { get; set; }
    public int FarmAlarmRaisedCount { get; set; }
    public DateTime? DateSmsRaised { get; set; }
    public int SmsRaisedCount { get; set; }
    public DateTime? DatePoultryAlarmRaised { get; set; }
    public int PoultryAlarmRaisedCount { get; set; }
    public string Descriptions { get; set; }
}