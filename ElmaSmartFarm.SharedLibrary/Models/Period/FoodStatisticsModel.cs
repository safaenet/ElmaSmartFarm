using System.Collections.Generic;
using System.Linq;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FoodStatisticsModel
    {
        public int PeriodId { get; set; }
        public List<FeedModel> Feeds { get; set; }
        public int TotalFeedWeight => Feeds != null ? Feeds.Sum(f => f != null ? f.FoodWeight : 0) : 0;
    }
}