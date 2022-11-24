namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FoodStatisticsModel
    {
        public List<FeedModel> Feeds { get; set; }
        public int TotalFeedWeight => Feeds != null ? Feeds.Sum(f => f != null ? f.FoodWeight : 0) : 0;
    }
}