using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using Serilog;

namespace ElmaSmartFarm.Service
{
    public partial class Worker
    {
        private async Task<int> ProcessMqttMessageAsync(MqttMessageModel mqtt)
        {
            if (mqtt == null) return -1;
            if (mqtt.Topic.StartsWith(config.mqtt.FullTemperatureTopic)) //Temp Sensor
            {
                if (mqtt.Topic.StartsWith(config.mqtt.FullTemperatureTopic + config.mqtt.KeepAliveSubTopic)) //Keep alive
                {
                    var sensorId = mqtt.Topic.Replace(config.mqtt.FullTemperatureTopic + config.mqtt.KeepAliveSubTopic, "");
                    if (string.IsNullOrEmpty(sensorId)) return -1;
                    if (!int.TryParse(sensorId, out int SensorId)) return -1;
                    var sensors = FindTempSensorsById(SensorId);
                    if (sensors == null)
                    {
                        //AddToUnknownList(); //Add sensor to unknown list.
                        return -1;
                    }
                    foreach (var s in sensors)
                    {
                        s.KeepAliveMessageDate = mqtt.ReadDate;
                        if (s.HasError && s.Errors != null)
                        {
                            var e = s.Errors.FirstOrDefault(e => e.ErrorType == SensorErrorType.NotAlive);
                            if (e != null) e.DateErased = DateTime.Now;
                            //Save to db.
                            if (s.Errors.Count > config.MaxSensorErrorCount) //Remove oldest record.
                            {
                                var error = s.Errors.Where(x => x.DateErased != null)?.MinBy(x => x.DateErased);
                                if (error != null) s.Errors.Remove(error);
                                else Log.Warning($"Error count in Sensor Id: {s.Id} hass reached limit but not erased! (System Error).");
                            }
                        }
                    }
                }
                else if (mqtt.Topic.StartsWith(config.mqtt.FullTemperatureTopic)) //Temp value
                {
                    var sensorId = mqtt.Topic.Replace(config.mqtt.FullTemperatureTopic, "");
                    if (string.IsNullOrEmpty(sensorId)) return -1;
                    if (!int.TryParse(sensorId, out int SensorId)) return -1;
                    if (!double.TryParse(mqtt.Payload, out double Payload)) return -1;
                    var sensors = FindTempSensorsById(SensorId);
                    if (sensors == null)
                    {
                        //AddToUnknownList(); //Add sensor to unknown list.
                        return -1;
                    }
                    foreach (var s in sensors)
                    {
                        if (s.Type == SensorType.FarmTemperature)
                        {
                            if (Payload < config.system.FarmTempMinValue || Payload > config.system.FarmTempMaxValue) //Invalid value
                            {
                                if (s.Errors == null) s.Errors = new();
                                if (!s.Errors.Where(e => e.ErrorType == SensorErrorType.InvalidValue).Any())
                                {
                                    //Save to db.
                                    s.Errors.Add(new()
                                    {
                                        SensorId = s.Id,
                                        ErrorType = SensorErrorType.InvalidValue,
                                        DateHappened = DateTime.Now,
                                        Descriptions = $"Invalid value: {Payload}"
                                    });
                                }
                            }
                        }
                        else if (s.Type == SensorType.OutdoorTemperature)
                        {
                            if (Payload < config.system.OutdoorTempMinValue || Payload > config.system.OutdoorTempMaxValue) //Invalid value
                            {
                                if (s.Errors == null) s.Errors = new();
                                if (!s.Errors.Where(e => e.ErrorType == SensorErrorType.InvalidValue).Any())
                                {
                                    //Save to db.
                                    s.Errors.Add(new()
                                    {
                                        SensorId = s.Id,
                                        ErrorType = SensorErrorType.InvalidValue,
                                        DateHappened = DateTime.Now,
                                        Descriptions = $"Invalid value: {Payload}"
                                    });
                                }
                            }
                        }
                        s.KeepAliveMessageDate = mqtt.ReadDate;
                        if (s.HasError && s.Errors != null) //Erase error
                        {
                            var e = s.Errors.FirstOrDefault(e => e.ErrorType == SensorErrorType.NotAlive);
                            if (e != null) e.DateErased = DateTime.Now;
                            if (s.Errors.Count > config.MaxSensorErrorCount) //Remove oldest record.
                            {
                                var error = s.Errors.Where(x => x.DateErased != null)?.MinBy(x => x.DateErased);
                                if (error != null) s.Errors.Remove(error);
                                else Log.Warning($"Error count in Sensor Id: {s.Id} hass reached limit but not erased! (System Error).");
                            }
                        }
                        //
                    }
                }

                TemperatureModel temp = new()
                {
                    SensorId = mqtt.SensorId,
                    Celsius = Payload,
                    ReadDate = mqtt.ReadDate
                };
                return await DbProcessor.SaveTemperatureToDb(temp);
            }
            return 0;
        }

        private List<TemperatureSensorModel>? FindTempSensorsById(int id)
        {
            if (poultries == null) return null;
            List<TemperatureSensorModel>? sensors = new();
            sensors.AddRange(from p in poultries where p.OutdoorTemperature.Id == id select p.OutdoorTemperature);
            sensors.AddRange(from s in poultries.SelectMany(p => p.Farms.SelectMany(f => f.Temperatures.Sensors)) where s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private List<HumiditySensorModel>? FindHumidSensorsById(int id)
        {
            if (poultries == null) return null;
            List<HumiditySensorModel>? sensors = new();
            sensors.AddRange(from p in poultries where p.OutdoorHumidity.Id == id select p.OutdoorHumidity);
            sensors.AddRange(from s in poultries.SelectMany(p => p.Farms.SelectMany(f => f.Humidities.Sensors)) where s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private List<AmbientLightSensorModel>? FindAmbientLightSensorsById(int id)
        {
            if (poultries == null) return null;
            List<AmbientLightSensorModel>? sensors = new();
            sensors.AddRange(from s in poultries.SelectMany(p => p.Farms.SelectMany(f => f.AmbientLights.Sensors)) where s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private List<PushButtonSensorModel>? FindPushButtonSensorsById(int id)
        {
            if (poultries == null) return null;
            List<PushButtonSensorModel>? sensors = new();
            sensors.AddRange(from s in poultries.SelectMany(p => p.Farms.SelectMany(f => f.Checkups.Sensors)) where s.Id == id select s);
            sensors.AddRange(from s in poultries.SelectMany(p => p.Farms.SelectMany(f => f.Feeds.Sensors)) where s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private List<BinarySensorModel>? FindBinarySensorsById(int id)
        {
            if (poultries == null) return null;
            List<BinarySensorModel>? sensors = new();
            sensors.AddRange(from p in poultries where p.MainElectricPower.Id == id select p.MainElectricPower);
            sensors.AddRange(from p in poultries where p.BackupElectricPower.Id == id select p.BackupElectricPower);
            sensors.AddRange(from s in poultries.SelectMany(p => p.Farms.SelectMany(f => f.ElectricPowers.Sensors)) where s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
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