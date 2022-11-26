namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FarmInPeriodErrorModel
    {
        public int Id { get; set; }
        public FarmInPeriodErrorType ErrorType { get; set; }
        public DateTime DateHappened { get; set; }
        public DateTime? DateErased { get; set; }
    }
}