using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class ScalarSensorSetModel : SensorSetModel<ScalarSensorModel>
    {
        public ScalarSensorModel MinimumTemperatureSensor => ActiveSensors?.MinBy(s => s.LastRead?.Temperature);
        public ScalarSensorModel MaximumTemperatureSensor => ActiveSensors?.MaxBy(s => s.LastRead?.Temperature);
        public double? AverageTemperatureValue => ActiveSensors?.Average(s => s.LastRead?.Temperature);

        public ScalarSensorModel MinimumHumiditySensor => ActiveSensors?.MinBy(s => s.LastRead?.Humidity);
        public ScalarSensorModel MaximumHumiditySensor => ActiveSensors?.MaxBy(s => s.LastRead?.Humidity);
        public double? AverageHumidityValue => ActiveSensors?.Average(s => s.LastRead?.Humidity);

        public ScalarSensorModel MinimumLightSensor => ActiveSensors?.MinBy(s => s.LastRead?.Light);
        public ScalarSensorModel MaximumLightSensor => ActiveSensors?.MaxBy(s => s.LastRead?.Light);
        public double? AverageLightValue => ActiveSensors?.Average(s => s.LastRead?.Light);

        public ScalarSensorModel MinimumAmmoniaSensor => ActiveSensors?.MinBy(s => s.LastRead?.Ammonia);
        public ScalarSensorModel MaximumAmmoniaSensor => ActiveSensors?.MaxBy(s => s.LastRead?.Ammonia);
        public double? AverageAmmoniaValue => ActiveSensors?.Average(s => s.LastRead?.Ammonia);

        public ScalarSensorModel MinimumCo2Sensor => ActiveSensors?.MinBy(s => s.LastRead?.Co2);
        public ScalarSensorModel MaximumCo2Sensor => ActiveSensors?.MaxBy(s => s.LastRead?.Co2);
        public double? AverageCo2Value => ActiveSensors?.Average(s => s.LastRead?.Co2);
    }
}