﻿namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class TemperatureSensorModel : SensorModel
    {
        public List<SensorReadModel<double>> Values { get; set; }
        public SensorReadModel<double> LastRead => Values?.MaxBy(t => t.ReadDate);
        public SensorReadModel<double> LastSavedRead => Values?.Where(t => t.IsSavedToDb).MaxBy(t => t.ReadDate);
        public double Offset { get; set; }
    }
}