namespace ElmaSmartFarm.SharedLibrary.Models;

public class FarmInPeriodErrorModel : ErrorModel
{
    public int FarmId { get; set; }
    public int PeriodId { get; set; }
    public int CausedSensorId { get; set; }
    public FarmInPeriodErrorType ErrorType { get; set; }
}