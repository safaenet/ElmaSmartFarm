namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class PeriodModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int DayOfPeriod => (DateTime.Now - StartDate).Days + 1;
        public int? Duration => EndDate == null ? null : (EndDate - StartDate).Value.Days;
        public ChickenStatisticsModel ChickenStatistics { get; set; }
        public FoodStatisticsModel FoodStatistics { get; set; }
        public PeriodErrorModel Errors { get; set; }
        public string Descriptions { get; set; }
    }
}