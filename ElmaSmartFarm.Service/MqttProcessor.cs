using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.Service
{
    public partial class Worker
    {
        private async Task<int> ProcessMqttMessageAsync(MqttMessageModel mqtt)
        {
            if (mqtt == null) return -1;
            if (mqtt.Topic.StartsWith(config.mqtt.ToServerTopic + config.mqtt.FarmTemperatureSubTopic))
            {
                var sensorId = mqtt.Topic.Replace(config.mqtt.ToServerTopic + config.mqtt.FarmTemperatureSubTopic + "/", "");
                if (string.IsNullOrEmpty(sensorId)) return -1;
                if (!int.TryParse(sensorId, out int SensorId)) return -1;
                if (!double.TryParse(mqtt.Payload, out double Payload)) return -1;

                TemperatureModel temp = new()
                {
                    SensorId = SensorId,
                    Celsius = Payload,
                    ReadDate = mqtt.ReadDate
                };
                return await DbProcessor.SaveTemperatureToDb(temp);
            }
            return 0;
        }

        private TemperatureSensorModel? FindFarmTempSensorById(int id)
        {
            if (poultries == null) return null;
            foreach (var p in poultries)
            {
                if (p.Farms == null || p.Farms.Count == 0) continue;
                foreach (var f in p.Farms)
                {
                    var s = f.Temperatures.Sensors.FirstOrDefault(s => s.Id == id);
                    if (s != null) return s;
                }
            }
            return null;
        }

        private HumiditySensorModel? FindFarmHumidSensorById(int id)
        {
            if (poultries == null) return null;
            foreach (var p in poultries)
            {
                if (p.Farms == null || p.Farms.Count == 0) continue;
                foreach (var f in p.Farms)
                {
                    var s = f.Humidities.Sensors.FirstOrDefault(s => s.Id == id);
                    if (s != null) return s;
                }
            }
            return null;
        }

        private AmbientLightSensorModel? FindFarmAmbientLightSensorById(int id)
        {
            if (poultries == null) return null;
            foreach (var p in poultries)
            {
                if (p.Farms == null || p.Farms.Count == 0) continue;
                foreach (var f in p.Farms)
                {
                    var s = f.AmbientLights.Sensors.FirstOrDefault(s => s.Id == id);
                    if (s != null) return s;
                }
            }
            return null;
        }

        private PushButtonSensorModel? FindFarmPushButtonSensorById(int id) //Feed and Checkup
        {
            if (poultries == null) return null;
            foreach (var p in poultries)
            {
                if (p.Farms == null || p.Farms.Count == 0) continue;
                foreach (var f in p.Farms)
                {
                    var s = f.Feeds.Sensors.FirstOrDefault(s => s.Id == id);
                    if (s != null) return s;
                    s = f.Checkups.Sensors.FirstOrDefault(s => s.Id == id);
                    if (s != null) return s;
                }
            }
            return null;
        }

        private CommuteSensorModel? FindFarmCommuteSensorById(int id)
        {
            if (poultries == null) return null;
            foreach (var p in poultries)
            {
                if (p.Farms == null || p.Farms.Count == 0) continue;
                foreach (var f in p.Farms)
                {
                    var s = f.Commutes.Sensors.FirstOrDefault(s => s.Id == id);
                    if (s != null) return s;
                }
            }
            return null;
        }

        private BinarySensorModel? FindFarmElectricPowerSensorById(int id)
        {
            if (poultries == null) return null;
            foreach (var p in poultries)
            {
                if (p.Farms == null || p.Farms.Count == 0) continue;
                foreach (var f in p.Farms)
                {
                    var s = f.ElectricPowers.Sensors.FirstOrDefault(s => s.Id == id);
                    if (s != null) return s;
                }
            }
            return null;
        }

        private List<TemperatureSensorModel>? FindOutdoorTempSensorById(int id)
        {
            if (poultries == null) return null;
            List<TemperatureSensorModel>? sensors = new();
            foreach (var p in poultries) if (p.OutdoorTemperature != null && p.OutdoorTemperature.Id == id) sensors.Add(p.OutdoorTemperature);
            if (sensors.Count == 0) return null; else return sensors;
        }

        private List<HumiditySensorModel>? FindOutdoorHumidSensorById(int id)
        {
            if (poultries == null) return null;
            List<HumiditySensorModel>? sensors = new();
            foreach (var p in poultries) if (p.OutdoorHumidity != null && p.OutdoorHumidity.Id == id) sensors.Add(p.OutdoorHumidity);
            if (sensors.Count == 0) return null; else return sensors;
        }
    }
}