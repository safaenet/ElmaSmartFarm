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
            var Now = mqtt.ReadDate;
            var Topic = mqtt.Topic.Replace(config.mqtt.ToServerTopic, "");
            if (Topic.StartsWith(config.mqtt.FromSensorSubTopic)) //Message from sensor.
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is from a sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                var SubTopics = Topic.Split("/");
                if (SubTopics.Length < 3 || string.IsNullOrEmpty(SubTopics[2]) || int.TryParse(SubTopics[2], out int SensorId) == false)
                {
                    AddMqttToUnknownList(mqtt);
                    Log.Warning($"Invalid MQTT message from sensor. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                    return -1;
                }
                if (SubTopics[1] == config.mqtt.KeepAliveSubTopic) //Keep Alive message from sensor.
                {
                    if (config.VerboseMode) Log.Information($"MQTT Message is KeepAlive from a sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                    await UpdateSensorKeepAliveAsync(SensorId, Now);
                }
                else if (SubTopics[1] == config.mqtt.IPAddressSubTopic) //IP Address message from sensor.
                {
                    if (config.VerboseMode) Log.Information($"MQTT Message is IP Address from a sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                    await UpdateSensorIPAddressAsync(SensorId, mqtt.Payload, Now);
                }
                else if (SubTopics[1] == config.mqtt.BatteryLevelSubTopic) //Battery Level message from sensor.
                {
                    if (config.VerboseMode) Log.Information($"MQTT Message is Battery Level from a sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                    if (int.TryParse(mqtt.Payload, out int level))
                        await UpdateSensorBatteryLevelAsync(SensorId, level, Now);
                    else
                    {
                        Log.Error($"Payload for sensor Battery Level is invlid. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                        AddMqttToUnknownList(mqtt);
                        return -1;
                    }
                }
                else if (SubTopics[1] == config.mqtt.TemperatureSubTopic) //Value from temp sensor.
                {
                    if (config.VerboseMode) Log.Information($"MQTT Message is value from a temp sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                    var sensors = FindTempSensorsById(SensorId);
                    if (sensors == null)
                    {
                        AddMqttToUnknownList(mqtt);
                        Log.Warning($"Unknown Temp Sensor sends value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        return -1;
                    }
                    if (!double.TryParse(mqtt.Payload, out double Payload)) //Invalid data.
                    {
                        AddMqttToUnknownList(mqtt);
                        Log.Warning($"A Sensor sends invalid data. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        if (sensors != null)
                            foreach (var s in sensors)
                            {
                                SensorErrorModel newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.InvalidData, Now, $"Data: {mqtt.Payload}");
                                if (s.AddError(newErr, SensorErrorType.InvalidData, config.system.MaxSensorErrorCount))
                                {
                                    var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
                                    if (newId > 0) s.LastError.Id = newId;
                                }
                                s.EraseError(SensorErrorType.NotAlive, Now);
                                await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.NotAlive, Now);
                            }
                        return -1;
                    }
                    foreach (var s in sensors) //Sensor(s) found. Check value.
                    {
                        if (config.VerboseMode) Log.Information($"Sensor ID is found in Poultries/Farms. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
                        s.KeepAliveMessageDate = Now;
                        s.EraseError(SensorErrorType.NotAlive, Now);
                        await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.NotAlive, Now);
                        if ((s.Type == SensorType.FarmTemperature && (Payload < config.system.FarmTempMinValue || Payload > config.system.FarmTempMaxValue)) || (s.Type == SensorType.OutdoorTemperature && (Payload < config.system.OutdoorTempMinValue || Payload > config.system.OutdoorTempMaxValue))) //Invalid value.
                        {
                            Log.Error($"A Temp Sensor sends invalid value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                            SensorErrorModel newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.InvalidValue, Now, $"Data: {mqtt.Payload}");
                            if (s.AddError(newErr, SensorErrorType.InvalidValue, config.system.MaxSensorErrorCount))
                            {
                                var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
                                if (newId > 0) s.LastError.Id = newId;
                            }
                        }
                        else //Sensor Valid, Value Valid.
                        {
                            if (config.VerboseMode) Log.Information($"Sensor is valid; Value is valid. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
                            s.EraseError(SensorErrorType.InvalidData, Now);
                            await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.InvalidData, Now);
                            s.EraseError(SensorErrorType.InvalidValue, Now);
                            await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.InvalidValue, Now);
                            if (s.Values == null) s.Values = new();
                            SensorReadModel<double> newRead = new() { Value = Payload, ReadDate = Now };
                            if (s.IsWatched && (s.Values.Count == 0 || s.LastSavedRead == null || (config.system.WriteOnValueChangeByDiffer && Math.Abs(s.LastRead.Value - Payload) >= config.system.TempMaxDifferValue) || (s.LastSavedRead != null && (Now - s.LastSavedRead.ReadDate).TotalSeconds >= config.system.WriteTempToDbInterval))) //Writable to db.
                            {
                                var newId = await DbProcessor.WriteSensorValueToDbAsync(s, Payload, Now);
                                if (newId > 0)
                                {
                                    newRead.IsSavedToDb = true;
                                    newRead.Id = newId;
                                }
                            }
                            if (s.Values.Count >= config.system.MaxSensorReadCount)
                            {
                                if (config.VerboseMode) Log.Information($"Count of Temp value list exceeded it's limit. Removing the oldest one. Sensor ID: {s.Id}, Count: {s.Values.Count}");
                                s.Values.RemoveOldestNotSaved();
                            }
                            s.Values.Add(newRead);
                            if (config.VerboseMode) Log.Information($"Sensor value received: {s.LastRead.ReadDate}, : {s.LastRead.Value}, Count: {s.Values.Count}");
                        }
                    }
                }
            }
            else //Unknown message.
            {
                AddMqttToUnknownList(mqtt);
                Log.Warning($"Invalid MQTT message from sensor. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                return -1;
            }








                #region Temperature sensor handler.
            //if (mqtt.Topic.StartsWith(config.mqtt.FullTemperatureTopic)) //Temp Sensor
            //{
            //    if (config.VerboseMode) Log.Information($"MQTT Message is from a Temp sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
            //    if (mqtt.Topic.StartsWith(config.mqtt.FullTemperatureTopic + config.mqtt.KeepAliveSubTopic)) //Keep alive
            //    {
            //        if (config.VerboseMode) Log.Information($"MQTT Message is KeepAlive from a Temp sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
            //        int SensorId = GetSensorIdFromTopic(mqtt, config.mqtt.FullTemperatureTopic + config.mqtt.KeepAliveSubTopic);
            //        if (SensorId == 0)
            //        {
            //            AddMqttToUnknownList(mqtt);
            //            Log.Warning($"Invalid KeepAlive MQTT message. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
            //            return -1;
            //        }
            //        if (config.VerboseMode) Log.Information($"Sensor ID extracted from MQTT Message. Sensor ID: {SensorId}");
            //        var sensors = FindTempSensorsById(SensorId);
            //        if (sensors == null)
            //        {
            //            AddMqttToUnknownList(mqtt);
            //            Log.Warning($"Unknown Sensor sends KeepAlive. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
            //            return -1;
            //        }
            //        foreach (var s in sensors) //Update KeepAlive and erase error if exists.
            //        {
            //            if (config.VerboseMode) Log.Information($"Sensor ID is found in Poultries/Farms. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
            //            s.KeepAliveMessageDate = mqtt.ReadDate;
            //            s.EraseError(SensorErrorType.NotAlive, Now);
            //            await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.NotAlive, Now);
            //        }
            //    }
            //    else if (mqtt.Topic.StartsWith(config.mqtt.FullTemperatureTopic)) //Temp value
            //    {
            //        if (config.VerboseMode) Log.Information($"MQTT Message is a Temp sensor value. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
            //        int SensorId = GetSensorIdFromTopic(mqtt, config.mqtt.FullTemperatureTopic);
            //        if (SensorId == 0)
            //        {
            //            AddMqttToUnknownList(mqtt);
            //            Log.Warning($"Invalid MQTT message. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
            //            return -1;
            //        }
            //        if (config.VerboseMode) Log.Information($"Sensor ID extracted from MQTT Message. Sensor ID: {SensorId}");
            //        var sensors = FindTempSensorsById(SensorId);
            //        if (sensors == null)
            //        {
            //            AddMqttToUnknownList(mqtt);
            //            Log.Warning($"Unknown Temp Sensor sends value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
            //            return -1;
            //        }
            //        if (!double.TryParse(mqtt.Payload, out double Payload)) //Invalid data.
            //        {
            //            AddMqttToUnknownList(mqtt);
            //            Log.Warning($"A Sensor sends invalid data. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
            //            if (sensors != null)
            //                foreach (var s in sensors)
            //                {
            //                    SensorErrorModel newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.InvalidData, Now, $"Data: {mqtt.Payload}");
            //                    if (s.AddError(newErr, SensorErrorType.InvalidData, config.system.MaxSensorErrorCount))
            //                    {
            //                        var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
            //                        if (newId > 0) s.LastError.Id = newId;
            //                    }
            //                    s.EraseError(SensorErrorType.NotAlive, Now);
            //                    await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.NotAlive, Now);
            //                }
            //            return -1;
            //        }
            //        foreach (var s in sensors)
            //        {
            //            if (config.VerboseMode) Log.Information($"Sensor ID is found in Poultries/Farms. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
            //            s.KeepAliveMessageDate = Now;
            //            s.EraseError(SensorErrorType.NotAlive, Now);
            //            await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.NotAlive, Now);
            //            if ((s.Type == SensorType.FarmTemperature && (Payload < config.system.FarmTempMinValue || Payload > config.system.FarmTempMaxValue)) || (s.Type == SensorType.OutdoorTemperature && (Payload < config.system.OutdoorTempMinValue || Payload > config.system.OutdoorTempMaxValue))) //Invalid value.
            //            {
            //                Log.Error($"A Temp Sensor sends invalid value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
            //                SensorErrorModel newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.InvalidValue, Now, $"Data: {mqtt.Payload}");
            //                if (s.AddError(newErr, SensorErrorType.InvalidValue, config.system.MaxSensorErrorCount))
            //                {
            //                    var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
            //                    if (newId > 0) s.LastError.Id = newId;
            //                }
            //            }
            //            else //Sensor Valid, Value Valid.
            //            {
            //                if (config.VerboseMode) Log.Information($"Sensor is valid; Value is valid. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
            //                s.EraseError(SensorErrorType.InvalidData, Now);
            //                await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.InvalidData, Now);
            //                s.EraseError(SensorErrorType.InvalidValue, Now);
            //                await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.InvalidValue, Now);
            //                if (s.Values == null) s.Values = new();
            //                SensorReadModel<double> newRead = new() { Value = Payload, ReadDate = Now };
            //                if (s.IsWatched && (s.Values.Count == 0 || s.LastSavedRead == null || (config.system.WriteOnValueChangeByDiffer && Math.Abs(s.LastRead.Value - Payload) >= config.system.TempMaxDifferValue) || (s.LastSavedRead != null && (DateTime.Now - s.LastSavedRead.ReadDate).TotalSeconds >= config.system.WriteTempToDbInterval)))
            //                {
            //                    if (config.VerboseMode) Log.Information($"Writing Temp sensor value to Database. Sensor ID: {s.Id}, Value: {newRead.Value}");
            //                    var newId = await DbProcessor.WriteSensorValueToDbAsync(s, Payload, Now);
            //                    if (newId > 0)
            //                    {
            //                        newRead.IsSavedToDb = true;
            //                        newRead.Id = newId;
            //                    }
            //                }
            //                if (s.Values.Count >= config.system.MaxSensorReadCount)
            //                {
            //                    if (config.VerboseMode) Log.Information($"Count of Temp value list exceeded its limit. Removing the oldest one. Sensor ID: {s.Id}, Count: {s.Values.Count}");
            //                    s.Values.RemoveOldestNotSaved();
            //                }
            //                s.Values.Add(newRead);
            //                if (config.VerboseMode) Log.Information($"Sensor value received: {s.LastRead.ReadDate}, : {s.LastRead.Value}, Count: {s.Values.Count}");
            //            }
            //        }
            //    }
            //}
                #endregion
            if (UnknownMqttMessages.Any(m => m.Topic == mqtt.Topic)) UnknownMqttMessages.Remove(UnknownMqttMessages.First(m => m.Topic == mqtt.Topic));
            return 0;
        }

        private async Task<int> UpdateSensorBatteryLevelAsync(int sensorId, int battery, DateTime now)
        {
            if (Poultries == null) return 0;
            int returnValue = 0;
            List<TemperatureSensorModel>? temps;
            temps = FindTempSensorsById(sensorId);
            if (temps != null && temps.Any())
            {
                foreach (var s in temps)
                {
                    s.BatteryLevel = battery;
                    s.EraseError(SensorErrorType.NotAlive, now);
                    if (battery != -1 && battery <= config.system.SensorLowBatteryLevel)
                    {
                        SensorErrorModel newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.LowBattery, now, $"Level: {battery}");
                        s.AddError(newErr, SensorErrorType.LowBattery, config.system.MaxSensorErrorCount);
                        await DbProcessor.WriteSensorErrorToDbAsync(newErr, now);
                    }
                    else
                    {
                        s.EraseError(SensorErrorType.LowBattery, now);
                        await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.LowBattery, now);
                    }
                }
                await DbProcessor.EraseSensorErrorFromDbAsync(sensorId, SensorErrorType.NotAlive, now);
                return returnValue;
            }
            temps = null;
            //...
            return 0;
        }

        private async Task<int> UpdateSensorIPAddressAsync(int sensorId, string ip, DateTime now)
        {
            if (Poultries == null) return 0;
            int returnValue = 0;
            List<TemperatureSensorModel>? temps;
            temps = FindTempSensorsById(sensorId);
            if (temps != null && temps.Any())
            {
                temps.ForEach(s => { s.IPAddress = ip; s.EraseError(SensorErrorType.NotAlive, now); });
                returnValue = temps.Count();
                await DbProcessor.EraseSensorErrorFromDbAsync(sensorId, SensorErrorType.NotAlive, now);
                return returnValue;
            }
            temps = null;
            //...
            return 0;
        }

        private async Task<int> UpdateSensorKeepAliveAsync(int sensorId, DateTime now)
        {
            if (Poultries == null) return 0;
            int returnValue = 0;
            List<TemperatureSensorModel>? temps;
            temps = FindTempSensorsById(sensorId);
            if (temps != null && temps.Any())
            {
                temps.ForEach(s => { s.KeepAliveMessageDate = now; s.EraseError(SensorErrorType.NotAlive, now); });
                await DbProcessor.EraseSensorErrorFromDbAsync(sensorId, SensorErrorType.NotAlive, now);
                return returnValue;
            }
            temps = null;
            //...
            return 0;
        }

        private int GetSensorIdFromTopic(string SubTopics)
        {
            if (string.IsNullOrEmpty(SubTopics)) return 0;
            var splits = SubTopics.Split("/");
            if (splits.Length < 3) return 0;
            if (string.IsNullOrEmpty(splits[2])) return 0;
            if (int.TryParse(splits[2], out int id)) return id;
            return 0;
        }

        private void AddMqttToUnknownList(MqttMessageModel mqtt)
        {
            if (UnknownMqttMessages.Any(m => m.Topic == mqtt.Topic)) UnknownMqttMessages.Remove(UnknownMqttMessages.First(m => m.Topic == mqtt.Topic));
            UnknownMqttMessages.Add(mqtt);
            if (UnknownMqttMessages != null && UnknownMqttMessages.Count > config.mqtt.MaxUnknownMqttCount) UnknownMqttMessages.Remove(UnknownMqttMessages.First());
        }

        private SensorErrorModel GenerateSensorError(SensorBaseModel sensor, SensorErrorType type, DateTime Now, string description = "")
        {
            return new SensorErrorModel()
            {
                SensorId = sensor.Id,
                LocationId = sensor.LocationId,
                Section = sensor.Section,
                ErrorType = type,
                DateHappened = Now,
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
    }
}