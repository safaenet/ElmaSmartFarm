namespace ElmaSmartFarm.SharedLibrary.Models;

public class PoultryInPeriodErrorModel : ErrorModel
{
    public int PoultryId { get; set; }
    public int PeriodId { get; set; }
    public PoultryInPeriodErrorType ErrorType { get; set; }
}