namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class BinarySensorReadModel
    {
        public DateTime ReadDate { get; set; }
        public bool Status { get; set; } //If true: pressed
    }
}