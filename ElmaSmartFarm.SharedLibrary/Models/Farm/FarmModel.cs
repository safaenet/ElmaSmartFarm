﻿using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FarmModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FarmTemperatureModel Temperature { get; set; }
        public FarmHumidityModel Humidity { get; set; }
        public FarmAmbientLightModel AmbientLight { get; set; }
        public FarmCommuteModel Commute { get; set; }
        public PushButtonSensorModel CheckupSensor { get; set; }
        public PushButtonSensorModel FeedSensor { get; set; }
        public BinarySensorModel ElectricPower { get; set; }
        public PeriodModel Period { get; set; }
        public bool IsInPeriod => Period != null && Period.EndDate != null;
        public bool IsEnabled { get; set; }
        public bool HasSensorError => IsEnabled && ((Temperature != null && Temperature.HasError) || (Humidity != null && Humidity.HasError) || (AmbientLight != null && AmbientLight.HasError) || (Commute != null && Commute.HasError) || (CheckupSensor != null && CheckupSensor.HasError) || (FeedSensor != null && FeedSensor.HasError) || (ElectricPower != null && ElectricPower.HasError));
        public string Descriptions { get; set; }
    }
}