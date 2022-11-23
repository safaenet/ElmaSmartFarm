namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class ChickenStatisticsModel
    {
        public int ChickenPrimaryCount { get; set; }
        public List<ChickenLossModel> ChickenLosses { get; set; }
        public int TotalLossCount => ChickenLosses != null ? ChickenLosses.Sum(c => c.LossCount) : 0;
        public int TotalAliveCount => ChickenPrimaryCount - TotalLossCount;
    }
}