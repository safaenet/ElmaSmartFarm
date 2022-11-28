namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class PoultryInPeriodErrorModel
    {
        public int Id { get; set; }
        public int PoultryId { get; set; }
        public int PeriodId { get; set; }
        public PoultryInPeriodErrorType ErrorType { get; set; }
        public DateTime DateHappened { get; set; }
        public DateTime? DateErased { get; set; }
    }
}