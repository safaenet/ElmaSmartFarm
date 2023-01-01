using System;

namespace ElmaSmartFarm.SharedLibrary.Models.Sensors;

public class SensorReadModel
{
    public int Id { get; set; }
    public DateTime ReadDate { get; set; }
    public bool IsSavedToDb { get; set; }
}