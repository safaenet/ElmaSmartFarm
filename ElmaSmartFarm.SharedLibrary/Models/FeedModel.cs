namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FeedModel
    {
        public int Id { get; set; }
        public int FoodWeight { get; set; }
        public DateTime FeedDate { get; set; }
        public DateTime DateRegistered { get; set; }
        public string Descriptions { get; set; }
    }
}