namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class ChickenLossModel
    {
        public int Id { get; set; }
        public int LossCount { get; set; }
        public DateTime DateHappened { get; set; }
        public DateTime DateRegistered { get; set; }
    }
}