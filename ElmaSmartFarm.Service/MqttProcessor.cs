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
                    if (await UpdateSensorKeepAliveAsync(SensorId, Now) == 0)
                    {
                        Log.Warning($"An unregistered sensor is sending KeepAlive Mqtt message: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        AddMqttToUnknownList(mqtt);
                        return -1;
                    }
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
                #region Temp Value Handler.
                else if (SubTopics[1] == config.mqtt.TemperatureSubTopic) //Value from temp sensor.
                {
                    if (config.VerboseMode) Log.Information($"MQTT Message is value from a temp sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                    var sensors = FindSensorsById<TemperatureSensorModel>(SensorId);
                    if (sensors == null)
                    {
                        AddMqttToUnknownList(mqtt);
                        Log.Warning($"Unknown temp sensor sends value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        return -1;
                    }
                    if (!double.TryParse(mqtt.Payload, out double Payload)) //Invalid data.
                    {
                        AddMqttToUnknownList(mqtt);
                        Log.Warning($"A temp sensor sends invalid data. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        if (sensors != null)
                            foreach (var s in sensors)
                            {
                                SensorErrorModel newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.InvalidData, Now, $"Data: {mqtt.Payload}");
                                if (s.Errors.AddError(newErr, SensorErrorType.InvalidData, config.system.MaxSensorErrorCount))
                                {
                                    var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
                                    if (newId > 0) s.LastError.Id = newId;
                                }
                                s.Errors.EraseError(SensorErrorType.NotAlive, Now);
                                await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.NotAlive, Now);
                            }
                        return -1;
                    }
                    foreach (var s in sensors) //Sensor(s) found. Check value.
                    {
                        if (config.VerboseMode) Log.Information($"Temp sensor ID is found in Poultries/Farms. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
                        s.KeepAliveMessageDate = Now;
                        s.Errors.EraseError(SensorErrorType.NotAlive, Now);
                        await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.NotAlive, Now);
                        if ((s.Type == SensorType.FarmTemperature && (Payload < config.system.FarmTempMinValue || Payload > config.system.FarmTempMaxValue)) || (s.Type == SensorType.OutdoorTemperature && (Payload < config.system.OutdoorTempMinValue || Payload > config.system.OutdoorTempMaxValue))) //Invalid value.
                        {
                            Log.Error($"A temp sensor sends invalid value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                            SensorErrorModel newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.InvalidValue, Now, $"Data: {mqtt.Payload}");
                            if (s.Errors.AddError(newErr, SensorErrorType.InvalidValue, config.system.MaxSensorErrorCount))
                            {
                                var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
                                if (newId > 0) s.LastError.Id = newId;
                            }
                        }
                        else //Sensor Valid, Value Valid.
                        {
                            if (config.VerboseMode) Log.Information($"Temp sensor is valid; Value is valid. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
                            s.Errors.EraseError(SensorErrorType.InvalidData, Now);
                            await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.InvalidData, Now);
                            s.Errors.EraseError(SensorErrorType.InvalidValue, Now);
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
                                if (config.VerboseMode) Log.Information($"Count of temp value list exceeded it's limit. Removing the oldest one. Sensor ID: {s.Id}, Count: {s.Values.Count}");
                                s.Values.RemoveOldestNotSaved();
                            }
                            s.Values.Add(newRead);
                            if (config.VerboseMode) Log.Information($"Temp sensor value done processing: {s.LastRead.ReadDate}, : {s.LastRead.Value}, Count: {s.Values.Count}");
                        }
                    }
                }
                #endregion
                #region Humidity Value Handler.
                else if (SubTopics[1] == config.mqtt.HumiditySubTopic) //Value from temp sensor.
                {
                    if (config.VerboseMode) Log.Information($"MQTT Message is value from a humid sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                    var sensors = FindSensorsById<HumiditySensorModel>(SensorId);
                    if (sensors == null)
                    {
                        AddMqttToUnknownList(mqtt);
                        Log.Warning($"Unknown humid sensor sends value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        return -1;
                    }
                    if (!double.TryParse(mqtt.Payload, out double Payload)) //Invalid data.
                    {
                        AddMqttToUnknownList(mqtt);
                        Log.Warning($"A humid sensor sends invalid data. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        if (sensors != null)
                            foreach (var s in sensors)
                            {
                                SensorErrorModel newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.InvalidData, Now, $"Data: {mqtt.Payload}");
                                if (s.Errors.AddError(newErr, SensorErrorType.InvalidData, config.system.MaxSensorErrorCount))
                                {
                                    var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
                                    if (newId > 0) s.LastError.Id = newId;
                                }
                                s.Errors.EraseError(SensorErrorType.NotAlive, Now);
                                await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.NotAlive, Now);
                            }
                        return -1;
                    }
                    foreach (var s in sensors) //Sensor(s) found. Check value.
                    {
                        if (config.VerboseMode) Log.Information($"Humid sensor ID is found in Poultries/Farms. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
                        s.KeepAliveMessageDate = Now;
                        s.Errors.EraseError(SensorErrorType.NotAlive, Now);
                        await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.NotAlive, Now);
                        if ((Payload < config.system.HumidMinValue || Payload > config.system.HumidMaxValue)) //Invalid value.
                        {
                            Log.Error($"A humid sensor sends invalid value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                            SensorErrorModel newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.InvalidValue, Now, $"Data: {mqtt.Payload}");
                            if (s.Errors.AddError(newErr, SensorErrorType.InvalidValue, config.system.MaxSensorErrorCount))
                            {
                                var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
                                if (newId > 0) s.LastError.Id = newId;
                            }
                        }
                        else //Sensor Valid, Value Valid.
                        {
                            if (config.VerboseMode) Log.Information($"Humid sensor is valid; Value is valid. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
                            s.Errors.EraseError(SensorErrorType.InvalidData, Now);
                            await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.InvalidData, Now);
                            s.Errors.EraseError(SensorErrorType.InvalidValue, Now);
                            await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.InvalidValue, Now);
                            if (s.Values == null) s.Values = new();
                            SensorReadModel<int> newRead = new() { Value = Payload, ReadDate = Now };
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
                                if (config.VerboseMode) Log.Information($"Count of temp value list exceeded it's limit. Removing the oldest one. Sensor ID: {s.Id}, Count: {s.Values.Count}");
                                s.Values.RemoveOldestNotSaved();
                            }
                            s.Values.Add(newRead);
                            if (config.VerboseMode) Log.Information($"Temp sensor value done processing: {s.LastRead.ReadDate}, : {s.LastRead.Value}, Count: {s.Values.Count}");
                        }
                    }
                }
                #endregion
            }
            else //Unknown message.
            {
                AddMqttToUnknownList(mqtt);
                Log.Warning($"Invalid MQTT message from sensor. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                return -1;
            }
            if (UnknownMqttMessages.Any(m => m.Topic == mqtt.Topic)) UnknownMqttMessages.Remove(UnknownMqttMessages.First(m => m.Topic == mqtt.Topic));
            return 0;
        }

        private async Task<int> UpdateSensorBatteryLevelAsync(int sensorId, int battery, DateTime now)
        {
            if (Poultries == null) return 0;
            List<SensorModel>? sensors;
            sensors = new(FindSensorsById<TemperatureSensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<HumiditySensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<AmbientLightSensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<CommuteSensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<PushButtonSensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<BinarySensorModel>(sensorId) ?? new());
            if (sensors.Any())
            {
                foreach (var s in sensors)
                {
                    s.BatteryLevel = battery;
                    s.Errors.EraseError(SensorErrorType.NotAlive, now);
                    if (battery != -1 && battery <= config.system.SensorLowBatteryLevel)
                    {
                        SensorErrorModel newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.LowBattery, now, $"Level: {battery}");
                        s.Errors.AddError(newErr, SensorErrorType.LowBattery, config.system.MaxSensorErrorCount);
                        await DbProcessor.WriteSensorErrorToDbAsync(newErr, now);
                    }
                    else
                    {
                        s.Errors.EraseError(SensorErrorType.LowBattery, now);
                        await DbProcessor.EraseSensorErrorFromDbAsync(s.AsBaseModel(), SensorErrorType.LowBattery, now);
                    }
                }
                await DbProcessor.EraseSensorErrorFromDbAsync(sensorId, SensorErrorType.NotAlive, now);
            }
            return sensors.Count;
        }

        private async Task<int> UpdateSensorIPAddressAsync(int sensorId, string ip, DateTime now)
        {
            if (Poultries == null) return 0;
            List<SensorModel>? sensors;
            sensors = new(FindSensorsById<TemperatureSensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<HumiditySensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<AmbientLightSensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<CommuteSensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<PushButtonSensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<BinarySensorModel>(sensorId) ?? new());
            if (sensors.Any())
            {
                sensors.ForEach(s => { s.IPAddress = ip; s.Errors.EraseError(SensorErrorType.NotAlive, now); });
                await DbProcessor.EraseSensorErrorFromDbAsync(sensorId, SensorErrorType.NotAlive, now);
            }
            return sensors.Count;
        }

        private async Task<int> UpdateSensorKeepAliveAsync(int sensorId, DateTime now)
        {
            if (Poultries == null) return 0;
            if (config.system.KeepAliveInterval == 0)
            {
                Log.Warning($"KeepAlive is disabled but sensor a is sending KeepAlive message. Sensor ID: {sensorId}");
                return -1;
            }
            List<SensorModel>? sensors;            
            sensors = new(FindSensorsById<TemperatureSensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<HumiditySensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<AmbientLightSensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<CommuteSensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<PushButtonSensorModel>(sensorId) ?? new());
            if (!sensors.Any()) sensors = new(FindSensorsById<BinarySensorModel>(sensorId) ?? new());
            if (sensors.Any())
            {
                sensors.ForEach(s => { s.KeepAliveMessageDate = now; s.Errors.EraseError(SensorErrorType.NotAlive, now); });
                await DbProcessor.EraseSensorErrorFromDbAsync(sensorId, SensorErrorType.NotAlive, now);                
            }
            return sensors.Count;
        }

        private void AddMqttToUnknownList(MqttMessageModel mqtt)
        {
            if (UnknownMqttMessages.Any(m => m.Topic == mqtt.Topic)) UnknownMqttMessages.Remove(UnknownMqttMessages.First(m => m.Topic == mqtt.Topic));
            UnknownMqttMessages.Add(mqtt);
            if (UnknownMqttMessages != null && UnknownMqttMessages.Count > config.mqtt.MaxUnknownMqttCount) UnknownMqttMessages.Remove(UnknownMqttMessages.First());
        }

        private static SensorErrorModel GenerateSensorError(SensorBaseModel sensor, SensorErrorType type, DateTime Now, string description = "")
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

        /// <summary>
        /// Looks for the sensors in all sensors of the Poultries.
        /// </summary>
        private List<T>? FindSensorsById<T>(int id) where T : new()
        {
            if (Poultries == null) return null;
            List<T>? sensors = new();
            if (typeof(T) == typeof(TemperatureSensorModel))
            {
                sensors.AddRange((IEnumerable<T>)(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Temperatures.Sensors)) where s != null && s.Id == id select s));
                sensors.AddRange((IEnumerable<T>)(from p in Poultries where p != null && p.OutdoorTemperature != null && p.OutdoorTemperature.Id == id select p.OutdoorTemperature));
            }
            else if (typeof(T) == typeof(HumiditySensorModel))
            {
                sensors.AddRange((IEnumerable<T>)(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Humidities.Sensors)) where s != null && s.Id == id select s));
                sensors.AddRange((IEnumerable<T>)(from p in Poultries where p != null && p.OutdoorHumidity != null && p.OutdoorHumidity.Id == id select p.OutdoorHumidity));
            }
            else if (typeof(T) == typeof(AmbientLightSensorModel))
                sensors.AddRange((IEnumerable<T>)(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.AmbientLights.Sensors)) where s != null && s.Id == id select s));
            else if (typeof(T) == typeof(CommuteSensorModel))
                sensors.AddRange((IEnumerable<T>)(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Commutes.Sensors)) where s != null && s.Id == id select s));
            else if (typeof(T) == typeof(PushButtonSensorModel))
            {
                sensors.AddRange((IEnumerable<T>)(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Checkups.Sensors)) where s != null && s.Id == id select s));
                sensors.AddRange((IEnumerable<T>)(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Feeds.Sensors)) where s != null && s.Id == id select s));
            }
            else if (typeof(T) == typeof(BinarySensorModel))
            {
                sensors.AddRange((IEnumerable<T>)(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.ElectricPowers.Sensors)) where s != null && s.Id == id select s));
                sensors.AddRange((IEnumerable<T>)(from p in Poultries where p != null && p.MainElectricPower != null && p.MainElectricPower.Id == id select p.MainElectricPower));
                sensors.AddRange((IEnumerable<T>)(from p in Poultries where p != null && p.BackupElectricPower != null && p.BackupElectricPower.Id == id select p.BackupElectricPower));
            }
            if (sensors.Count > 0) return (List<T>)Convert.ChangeType(sensors, typeof(List<T>)); else return null;
        }
    }
}