namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class PeriodModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ElapsedDays => (DateTime.Now - StartDate).Days;
        public ChickenStatisticsModel ChickenStatistics { get; set; }
        public string Descriptions { get; set; }
    }
}