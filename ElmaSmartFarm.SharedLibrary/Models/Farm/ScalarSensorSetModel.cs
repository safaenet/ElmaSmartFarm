using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models;

public class ScalarSensorSetModel : SensorSetModel<ScalarSensorModel>
{
    public ScalarSensorModel MinimumTemperatureSensor => ActiveSensors?.Where(s => s.WatchTemperature).MinBy(s => s.LastRead?.Temperature);
    public ScalarSensorModel MaximumTemperatureSensor => ActiveSensors?.Where(s => s.WatchTemperature).MaxBy(s => s.LastRead?.Temperature);
    public double? AverageTemperatureValue => ActiveSensors?.Where(s => s.WatchTemperature).Average(s => s.LastRead?.Temperature);

    public ScalarSensorModel MinimumHumiditySensor => ActiveSensors?.Where(s => s.WatchHumidity).MinBy(s => s.LastRead?.Humidity);
    public ScalarSensorModel MaximumHumiditySensor => ActiveSensors?.Where(s => s.WatchHumidity).MaxBy(s => s.LastRead?.Humidity);
    public double? AverageHumidityValue => ActiveSensors?.Where(s => s.WatchHumidity).Average(s => s.LastRead?.Humidity);

    public ScalarSensorModel MinimumLightSensor => ActiveSensors?.Where(s => s.WatchLight).MinBy(s => s.LastRead?.Light);
    public ScalarSensorModel MaximumLightSensor => ActiveSensors?.Where(s => s.WatchLight).MaxBy(s => s.LastRead?.Light);
    public double? AverageLightValue => ActiveSensors?.Where(s => s.WatchLight).Average(s => s.LastRead?.Light);

    public ScalarSensorModel MinimumAmmoniaSensor => ActiveSensors?.Where(s => s.WatchAmmonia).MinBy(s => s.LastRead?.Ammonia);
    public ScalarSensorModel MaximumAmmoniaSensor => ActiveSensors?.Where(s => s.WatchAmmonia).MaxBy(s => s.LastRead?.Ammonia);
    public double? AverageAmmoniaValue => ActiveSensors?.Where(s => s.WatchAmmonia).Average(s => s.LastRead?.Ammonia);

    public ScalarSensorModel MinimumCo2Sensor => ActiveSensors?.Where(s => s.WatchCo2).MinBy(s => s.LastRead?.Co2);
    public ScalarSensorModel MaximumCo2Sensor => ActiveSensors?.Where(s => s.WatchCo2).MaxBy(s => s.LastRead?.Co2);
    public double? AverageCo2Value => ActiveSensors?.Where(s => s.WatchCo2).Average(s => s.LastRead?.Co2);
}