namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class ChickenStatisticsModel
    {
        public int ChickenPrimaryCount { get; set; }
        public IEnumerable<ChickenLossModel> ChickenLosses { get; set; }
        public int TotalLossCount => ChickenLosses != null ? ChickenLosses.Sum(c => c != null ? c.LossCount : 0) : 0;
        public int TotalAliveCount => ChickenPrimaryCount - TotalLossCount;
    }
}