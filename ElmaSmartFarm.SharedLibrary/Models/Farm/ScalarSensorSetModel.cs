using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class ScalarSensorSetModel : SensorSetModel<ScalarSensorModel>
    {
        public double? MinimumTemperatureValue => ActiveSensors?.Min(s => s.LastRead)?.Temperature;
        public double? MaximumTemperatureValue => ActiveSensors?.Max(s => s.LastRead)?.Temperature;
        public double? AverageTemperatureValue => ActiveSensors?.Average(s => s.LastRead.Temperature);

        public int? MinimumHumidityValue => ActiveSensors?.Min(s => s.LastRead)?.Humidity;
        public int? MaximumHumidityValue => ActiveSensors?.Max(s => s.LastRead)?.Humidity;
        public double? AverageHumidityValue => ActiveSensors?.Average(s => s.LastRead.Humidity);
    }
}