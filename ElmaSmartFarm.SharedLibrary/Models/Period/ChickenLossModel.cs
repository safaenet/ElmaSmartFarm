namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class ChickenLossModel
    {
        public int Id { get; set; }
        public int FarmId { get; set; }
        public int LossCount { get; set; }
        public DateTime DateHappened { get; set; }
        public DateTime DateRegistered { get; set; }
        public string Descriptions { get; set; }
    }
}