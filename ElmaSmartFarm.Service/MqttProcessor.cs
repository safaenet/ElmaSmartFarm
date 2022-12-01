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
            var Now = DateTime.Now;
            if (mqtt.Topic.StartsWith(config.mqtt.FullTemperatureTopic)) //Temp Sensor
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is from a Temp sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                if (mqtt.Topic.StartsWith(config.mqtt.FullTemperatureTopic + config.mqtt.KeepAliveSubTopic)) //Keep alive
                {
                    if (config.VerboseMode) Log.Information($"MQTT Message is KeepAlive from a Temp sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                    int SensorId = GetSensorIdFromTopic(mqtt, config.mqtt.FullTemperatureTopic + config.mqtt.KeepAliveSubTopic);
                    if(SensorId == 0)
                    {
                        if (!UnknownMqttMessages.Any(m => m.Topic == mqtt.Topic)) UnknownMqttMessages.Add(mqtt);
                        Log.Warning($"Invalid KeepAlive MQTT message. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        return -1;
                    }
                    if (config.VerboseMode) Log.Information($"Sensor ID extracted from MQTT Message. Sensor ID: {SensorId}");
                    var sensors = FindTempSensorsById(SensorId);
                    if (sensors == null)
                    {
                        if (!UnknownMqttMessages.Any(m => m.Topic == mqtt.Topic)) UnknownMqttMessages.Add(mqtt);
                        Log.Warning($"Unknown Sensor sends KeepAlive. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        return -1;
                    }
                    foreach (var s in sensors) //Update KeepAlive and erase error if exists.
                    {
                        if (config.VerboseMode) Log.Information($"Sensor ID is found in Poultries/Farms. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
                        s.KeepAliveMessageDate = mqtt.ReadDate;
                        if (s.Errors != null && s.EraseError(SensorErrorType.NotAlive, Now))
                        {
                            await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.NotAlive, Now);
                        }
                    }
                }
                else if (mqtt.Topic.StartsWith(config.mqtt.FullTemperatureTopic)) //Temp value
                {
                    if (config.VerboseMode) Log.Information($"MQTT Message is a Temp sensor value. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                    int SensorId = GetSensorIdFromTopic(mqtt, config.mqtt.FullTemperatureTopic);
                    if (SensorId == 0)
                    {
                        if (!UnknownMqttMessages.Any(m => m.Topic == mqtt.Topic)) UnknownMqttMessages.Add(mqtt);
                        Log.Warning($"Invalid MQTT message. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        return -1;
                    }
                    if (config.VerboseMode) Log.Information($"Sensor ID extracted from MQTT Message. Sensor ID: {SensorId}");
                    var sensors = FindTempSensorsById(SensorId);
                    if (sensors == null)
                    {
                        if (!UnknownMqttMessages.Any(m => m.Topic == mqtt.Topic)) UnknownMqttMessages.Add(mqtt);
                        Log.Warning($"Unknown Temp Sensor sends value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        return -1;
                    }
                    if (!double.TryParse(mqtt.Payload, out double Payload))
                    {
                        if (!UnknownMqttMessages.Any(m => m.Topic == mqtt.Topic)) UnknownMqttMessages.Add(mqtt);
                        Log.Warning($"A Sensor sends invalid data. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        if (sensors != null) 
                            foreach (var s in sensors)
                            {
                                SensorErrorModel newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.InvalidValue, $"Data: {mqtt.Payload}");
                                if (s.AddError(newErr, SensorErrorType.InvalidValue, config.system.MaxSensorErrorCount))
                                {
                                    var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
                                    if (newId > 0) s.LastError.Id = newId;
                                }
                            }
                        return -1;
                    }
                    foreach (var s in sensors)
                    {
                        if (config.VerboseMode) Log.Information($"Sensor ID is found in Poultries/Farms. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
                        s.KeepAliveMessageDate = Now;
                        if (s.EraseError(SensorErrorType.NotAlive, Now))
                        {
                            await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.NotAlive, Now);
                        }
                        if ((s.Type == SensorType.FarmTemperature && (Payload < config.system.FarmTempMinValue || Payload > config.system.FarmTempMaxValue)) || (s.Type == SensorType.OutdoorTemperature && (Payload < config.system.OutdoorTempMinValue || Payload > config.system.OutdoorTempMaxValue))) //Invalid value.
                        {
                            Log.Error($"A Temp Sensor sends invalid value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                            SensorErrorModel newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.InvalidValue, $"Data: {mqtt.Payload}");
                            if (s.AddError(newErr, SensorErrorType.InvalidValue, config.system.MaxSensorErrorCount))
                            {
                                var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
                                if (newId > 0) s.LastError.Id = newId;
                            }
                        }
                        else //Sensor Valid, Value Valid.
                        {
                            if (config.VerboseMode) Log.Information($"Sensor is valid; Value is valid. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
                            if (s.EraseError(SensorErrorType.InvalidValue, Now))
                            {
                                await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.InvalidValue, Now);
                            }
                            if (s.Values == null) s.Values = new();
                            SensorReadModel<double> newRead = new() { Value = Payload, ReadDate = Now };
                            if (s.IsWatched && (s.Values.Count == 0 || s.LastSavedRead == null || (config.system.WriteOnValueChangeByDiffer && Math.Abs(s.LastRead.Value - Payload) >= config.system.TempMaxDifferValue) || (s.LastSavedRead != null && (DateTime.Now - s.LastSavedRead.ReadDate).TotalSeconds >= config.system.WriteTempToDbInterval)))
                            {
                                if (config.VerboseMode) Log.Information($"Writing Temp sensor value to Database. Sensor ID: {s.Id}, Value: {newRead.Value}");
                                var newId = await DbProcessor.SaveSensorValueToDbAsync(s, Payload, Now);
                                if (newId > 0) 
                                {
                                    newRead.IsSavedToDb = true;
                                    newRead.Id = newId;
                                }
                            }
                            if (s.Values.Count >= config.system.MaxSensorReadCount)
                            {
                                if (config.VerboseMode) Log.Information($"Count of Temp value list exceeded its limit. Removing the oldest one. Sensor ID: {s.Id}, Count: {s.Values.Count}");
                                s.Values.RemoveOldestNotSaved();
                            }
                            s.Values.Add(newRead);
                            if (config.VerboseMode) Log.Information($"Sensor value received: {s.LastRead.ReadDate}, : {s.LastRead.Value}, Count: {s.Values.Count}");
                        }
                    }
                }
            }
            UnknownMqttMessages.Remove(mqtt);
            return 0;
        }

        private SensorErrorModel GenerateSensorError(SensorBaseModel sensor, SensorErrorType type, string description = "")
        {
            return new SensorErrorModel()
            {
                SensorId = sensor.Id,
                LocationId = sensor.LocationId,
                Section = sensor.Section,
                ErrorType = type,
                DateHappened = DateTime.Now,
                Descriptions = description
            };
        }

        private int GetSensorIdFromTopic(MqttMessageModel mqtt, string TopicTemplate)
        {
            var sensorId = mqtt.Topic.Replace(TopicTemplate, "");
            if (string.IsNullOrEmpty(sensorId)) return 0;
            if (!int.TryParse(sensorId, out int SensorId)) return 0;
            return SensorId;
        }

        private List<TemperatureSensorModel>? FindTempSensorsById(int id)
        {
            if (Poultries == null) return null;
            List<TemperatureSensorModel>? sensors = new();
            sensors.AddRange(from p in Poultries where p != null && p.OutdoorTemperature != null && p.OutdoorTemperature.Id == id select p.OutdoorTemperature);
            sensors.AddRange(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Temperatures.Sensors)) where s != null && s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private List<HumiditySensorModel>? FindHumidSensorsById(int id)
        {
            if (Poultries == null) return null;
            List<HumiditySensorModel>? sensors = new();
            sensors.AddRange(from p in Poultries where p != null && p.OutdoorHumidity.Id == id select p.OutdoorHumidity);
            sensors.AddRange(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Humidities.Sensors)) where s != null && s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private List<AmbientLightSensorModel>? FindAmbientLightSensorsById(int id)
        {
            if (Poultries == null) return null;
            List<AmbientLightSensorModel>? sensors = new();
            sensors.AddRange(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.AmbientLights.Sensors)) where s != null && s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private List<PushButtonSensorModel>? FindPushButtonSensorsById(int id)
        {
            if (Poultries == null) return null;
            List<PushButtonSensorModel>? sensors = new();
            sensors.AddRange(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Checkups.Sensors)) where s != null && s.Id == id select s);
            sensors.AddRange(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Feeds.Sensors)) where s != null && s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private List<BinarySensorModel>? FindBinarySensorsById(int id)
        {
            if (Poultries == null) return null;
            List<BinarySensorModel>? sensors = new();
            sensors.AddRange(from p in Poultries where p != null && p.MainElectricPower.Id == id select p.MainElectricPower);
            sensors.AddRange(from p in Poultries where p != null && p.BackupElectricPower.Id == id select p.BackupElectricPower);
            sensors.AddRange(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.ElectricPowers.Sensors)) where s != null && s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private TemperatureSensorModel? FindFarmTempSensorById(int id)
        {
            if (Poultries == null) return null;
            foreach (var p in Poultries)
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
            if (Poultries == null) return null;
            foreach (var p in Poultries)
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
            if (Poultries == null) return null;
            foreach (var p in Poultries)
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
            if (Poultries == null) return null;
            foreach (var p in Poultries)
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
            if (Poultries == null) return null;
            foreach (var p in Poultries)
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
            if (Poultries == null) return null;
            foreach (var p in Poultries)
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
            if (Poultries == null) return null;
            List<TemperatureSensorModel>? sensors = new();
            foreach (var p in Poultries) if (p.OutdoorTemperature != null && p.OutdoorTemperature.Id == id) sensors.Add(p.OutdoorTemperature);
            if (sensors.Count == 0) return null; else return sensors;
        }

        private List<HumiditySensorModel>? FindOutdoorHumidSensorById(int id)
        {
            if (Poultries == null) return null;
            List<HumiditySensorModel>? sensors = new();
            foreach (var p in Poultries) if (p.OutdoorHumidity != null && p.OutdoorHumidity.Id == id) sensors.Add(p.OutdoorHumidity);
            if (sensors.Count == 0) return null; else return sensors;
        }
    }
}