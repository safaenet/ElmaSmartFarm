namespace ElmaSmartFarm.SharedLibrary.Models.Sensors.ReadModels
{
    public class BinarySensorReadModel
    {
        public DateTime ReadDate { get; set; }
        public bool Status { get; set; } //If true: pressed
    }
}