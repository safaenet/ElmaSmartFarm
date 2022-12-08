using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using Serilog;

namespace ElmaSmartFarm.Service;

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
            if (SubTopics.Length < 3 || string.IsNullOrEmpty(SubTopics[2]) || int.TryParse(SubTopics[2], out var SensorId) == false)
            {
                AddMqttToUnknownList(mqtt);
                return -1;
            }
            #region KeepAlive Message Handler
            if (SubTopics[1] == config.mqtt.KeepAliveSubTopic) //Keep Alive message from sensor.
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is KeepAlive from a sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                if (await UpdateSensorKeepAliveAsync(SensorId, Now) == 0)
                {
                    AddMqttToUnknownList(mqtt);
                    return -1;
                }
            }
            #endregion
            #region IP Address Message Handler
            else if (SubTopics[1] == config.mqtt.IPAddressSubTopic) //IP Address message from sensor.
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is IP Address from a sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                await UpdateSensorIPAddressAsync(SensorId, mqtt.Payload, Now);
            }
            #endregion
            #region Battery Level Message Handler
            else if (SubTopics[1] == config.mqtt.BatteryLevelSubTopic) //Battery Level message from sensor.
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is Battery Level from a sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                if (int.TryParse(mqtt.Payload, out var level))
                    await UpdateSensorBatteryLevelAsync(SensorId, level, Now);
                else
                {
                    AddMqttToUnknownList(mqtt);
                    return -1;
                }
            }
            #endregion
            #region Temp Value Handler.
            else if (SubTopics[1] == config.mqtt.TemperatureSubTopic) //Value from temp sensor.
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is value from a temp sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                var sensors = FindSensorsById<TemperatureSensorModel>(SensorId);
                if (sensors == null || sensors.All(s => s.IsEnabled == false))
                {
                    AddMqttToUnknownList(mqtt);
                    return -1;
                }
                if (!double.TryParse(mqtt.Payload, out var Payload)) //Invalid data.
                {
                    await InvalidMqttPayloadHandler(mqtt, sensors, Now);
                    return -1;
                }
                foreach (var s in sensors) //Sensor(s) found. Check value.
                {
                    await EraseSensorNotAliveErrorIfExists(s, Now);
                    if ((s.Type == SensorType.FarmTemperature && (Payload < config.system.FarmTempMinValue || Payload > config.system.FarmTempMaxValue)) || (s.Type == SensorType.OutdoorTemperature && (Payload < config.system.OutdoorTempMinValue || Payload > config.system.OutdoorTempMaxValue))) //Invalid value.
                        await InvalidSensorValueHandler(s, mqtt, Now); //Invalid sensor value.
                    else //Sensor Valid, Value Valid.
                    {
                        await EraseInvalidDataAndValueErrors(s, Now);
                        if (s.Values == null) s.Values = new();
                        SensorReadModel<double> newRead = new() { Value = Payload, ReadDate = Now };
                        if ((s.IsWatched || config.system.WriteTempToDbAlways) && (s.Values.Count == 0 || s.LastSavedRead == null || (config.system.WriteTempOnValueChangeByDiffer && Math.Abs(s.LastSavedRead.Value - Payload) >= config.system.WriteTempDifferValue) || (Now - s.LastSavedRead.ReadDate).TotalSeconds >= config.system.WriteTempToDbInterval)) //Writable to db.
                        {
                            var newId = await DbProcessor.WriteSensorValueToDbAsync(s, Payload, Now, s.Offset);
                            if (newId > 0)
                            {
                                newRead.IsSavedToDb = true;
                                newRead.Id = newId;
                            }
                        }
                        if (s.Values.Count >= config.system.MaxSensorReadCount) s.Values.RemoveOldestNotSaved(config.VerboseMode);
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
                if (sensors == null || sensors.All(s => s.IsEnabled == false))
                {
                    AddMqttToUnknownList(mqtt);
                    return -1;
                }
                if (!int.TryParse(mqtt.Payload, out var Payload)) //Invalid data.
                {
                    await InvalidMqttPayloadHandler(mqtt, sensors, Now);
                    return -1;
                }
                foreach (var s in sensors) //Sensor(s) found. Check value.
                {
                    await EraseSensorNotAliveErrorIfExists(s, Now);
                    if ((Payload < config.system.HumidMinValue || Payload > config.system.HumidMaxValue)) await InvalidSensorValueHandler(s, mqtt, Now); //Invalid sensor value.
                    else //Sensor Valid, Value Valid.
                    {
                        await EraseInvalidDataAndValueErrors(s, Now);
                        if (s.Values == null) s.Values = new();
                        SensorReadModel<int> newRead = new() { Value = Payload, ReadDate = Now };
                        if ((s.IsWatched || config.system.WriteHumidToDbAlways) && (s.Values.Count == 0 || s.LastSavedRead == null || (config.system.WriteHumidOnValueChangeByDiffer && Math.Abs(s.LastSavedRead.Value - Payload) >= config.system.WriteHumidDifferValue) || (Now - s.LastSavedRead.ReadDate).TotalSeconds >= config.system.WriteHumidToDbInterval)) //Writable to db.
                        {
                            var newId = await DbProcessor.WriteSensorValueToDbAsync(s, Payload, Now, s.Offset);
                            if (newId > 0)
                            {
                                newRead.IsSavedToDb = true;
                                newRead.Id = newId;
                            }
                        }
                        if (s.Values.Count >= config.system.MaxSensorReadCount) s.Values.RemoveOldestNotSaved(config.VerboseMode);
                        s.Values.Add(newRead);
                        if (config.VerboseMode) Log.Information($"Temp sensor value done processing: {s.LastRead.ReadDate}, : {s.LastRead.Value}, Count: {s.Values.Count}");
                    }
                }
            }
            #endregion
            #region Ambient Light Value Handler.
            else if (SubTopics[1] == config.mqtt.AmbientLightSubTopic) //Value from ambient light sensor.
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is value from a ambient light sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                var sensors = FindSensorsById<AmbientLightSensorModel>(SensorId);
                if (sensors == null || sensors.All(s => s.IsEnabled == false))
                {
                    AddMqttToUnknownList(mqtt);
                    return -1;
                }
                if (!int.TryParse(mqtt.Payload, out var Payload)) //Invalid data.
                {
                    await InvalidMqttPayloadHandler(mqtt, sensors, Now);
                    return -1;
                }
                foreach (var s in sensors) //Sensor(s) found. Check value.
                {
                    await EraseSensorNotAliveErrorIfExists(s, Now);
                    if ((Payload < config.system.AmbientLightMinValue || Payload > config.system.AmbientLightMaxValue)) await InvalidSensorValueHandler(s, mqtt, Now); //Invalid sensor value.
                    else //Sensor Valid, Value Valid.
                    {
                        await EraseInvalidDataAndValueErrors(s, Now);
                        if (s.Values == null) s.Values = new();
                        SensorReadModel<int> newRead = new() { Value = Payload, ReadDate = Now };
                        if ((s.IsWatched || config.system.WriteAmbientLightToDbAlways) && (s.Values.Count == 0 || s.LastSavedRead == null || (config.system.WriteAmbientLightOnValueChangeByDiffer && Math.Abs(s.LastSavedRead.Value - Payload) >= config.system.WriteAmbientLightDifferValue) || (Now - s.LastSavedRead.ReadDate).TotalSeconds >= config.system.WriteAmbientLightToDbInterval)) //Writable to db.
                        {
                            var newId = await DbProcessor.WriteSensorValueToDbAsync(s, Payload, Now, s.Offset);
                            if (newId > 0)
                            {
                                newRead.IsSavedToDb = true;
                                newRead.Id = newId;
                            }
                        }
                        if (s.Values.Count >= config.system.MaxSensorReadCount) s.Values.RemoveOldestNotSaved(config.VerboseMode);
                        s.Values.Add(newRead);
                        if (config.VerboseMode) Log.Information($"Ambient light sensor value done processing: {s.LastRead.ReadDate}, : {s.LastRead.Value}, Count: {s.Values.Count}");
                    }
                }
            }
            #endregion
            #region Commute Value Handler.
            else if (SubTopics[1] == config.mqtt.CommuteSubTopic) //Value from commute sensor.
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is value from a commute sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                var sensors = FindSensorsById<CommuteSensorModel>(SensorId);
                if (sensors == null || sensors.All(s => s.IsEnabled == false))
                {
                    AddMqttToUnknownList(mqtt);
                    return -1;
                }
                if (!Enum.TryParse(mqtt.Payload, out CommuteSensorValueType Payload)) //Invalid data.
                {
                    await InvalidMqttPayloadHandler(mqtt, sensors, Now);
                    return -1;
                }
                foreach (var s in sensors) //Sensor(s) found. Sensor valid, Value valid.
                {
                    await EraseSensorNotAliveErrorIfExists(s, Now);
                    await EraseInvalidDataAndValueErrors(s, Now);
                    if (s.Values == null) s.Values = new();
                    SensorReadModel<CommuteSensorValueType> newRead = new() { Value = Payload, ReadDate = Now };
                    if ((s.IsWatched || config.system.WriteCommuteToDbAlways) && (s.Values.Count == 0 || (Payload == CommuteSensorValueType.StepIn && s.LastStepInSavedRead == null) || (Payload == CommuteSensorValueType.StepOut && s.LastStepOutSavedRead == null) || s.LastRead.Value != Payload)) //Writable to db.
                    {
                        var newId = await DbProcessor.WriteSensorValueToDbAsync(s, (double)Payload, Now);
                        if (newId > 0)
                        {
                            newRead.IsSavedToDb = true;
                            newRead.Id = newId;
                        }
                    }
                    if (s.Values.Count >= config.system.MaxSensorReadCount) s.Values.RemoveOldestNotSaved(config.VerboseMode);
                    s.Values.Add(newRead);
                    if (config.VerboseMode) Log.Information($"Commute sensor value done processing: {s.LastRead.ReadDate}, : {s.LastRead.Value}, Count: {s.Values.Count}");
                }
            }
            #endregion
            #region PushButton Value Handler.
            else if (SubTopics[1] == config.mqtt.PushButtonSubTopic) //Value from push button sensor.
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is value from a push button sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                var sensors = FindSensorsById<PushButtonSensorModel>(SensorId);
                if (sensors == null || sensors.All(s => s.IsEnabled == false))
                {
                    AddMqttToUnknownList(mqtt);
                    return -1;
                }
                foreach (var s in sensors) //Sensor(s) found. Check value. Sensor Valid, Value Valid.
                {
                    await EraseSensorNotAliveErrorIfExists(s, Now);
                    if (s.Values == null) s.Values = new();
                    SensorReadModel<DateTime> newRead = new() { Value = Now, ReadDate = Now };
                    if ((s.IsWatched || config.system.WritePushButtonToDbAlways)) //Writable to db.
                    {
                        var newId = await DbProcessor.WriteSensorValueToDbAsync(s, 0, Now);
                        if (newId > 0)
                        {
                            newRead.IsSavedToDb = true;
                            newRead.Id = newId;
                        }
                    }
                    if (s.Values.Count >= config.system.MaxSensorReadCount) s.Values.RemoveOldestNotSaved(config.VerboseMode);
                    s.Values.Add(newRead);
                }
            }
            #endregion
            #region Binary Value Handler.
            else if (SubTopics[1] == config.mqtt.BinarySubTopic) //Value from binary sensor.
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is value from a ambient light sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                var sensors = FindSensorsById<BinarySensorModel>(SensorId);
                if (sensors == null || sensors.All(s => s.IsEnabled == false))
                {
                    AddMqttToUnknownList(mqtt);
                    return -1;
                }
                if (!Enum.TryParse(mqtt.Payload, out BinarySensorValueType Payload)) //Invalid data.
                {
                    await InvalidMqttPayloadHandler(mqtt, sensors, Now);
                    return -1;
                }
                foreach (var s in sensors) //Sensor(s) found. Check value. Sensor Valid, Value Valid.
                {
                    await EraseSensorNotAliveErrorIfExists(s, Now);
                    await EraseInvalidDataAndValueErrors(s, Now);
                    if (s.Values == null) s.Values = new();
                    SensorReadModel<BinarySensorValueType> newRead = new() { Value = Payload, ReadDate = Now };
                    if ((s.IsWatched || config.system.WriteBinaryToDbAlways) && (s.Values.Count == 0 || s.LastSavedRead == null || (config.system.WriteBinaryOnValueChange && s.LastSavedRead.Value != Payload) || (Now - s.LastSavedRead.ReadDate).TotalSeconds >= config.system.WriteBinaryToDbInterval)) //Writable to db.
                    {
                        var newId = await DbProcessor.WriteSensorValueToDbAsync(s, (double)Payload, Now);
                        if (newId > 0)
                        {
                            newRead.IsSavedToDb = true;
                            newRead.Id = newId;
                        }
                    }
                    if (s.Values.Count >= config.system.MaxSensorReadCount) s.Values.RemoveOldestNotSaved(config.VerboseMode);
                    s.Values.Add(newRead);
                    if (config.VerboseMode) Log.Information($"Binary sensor value done processing: {s.LastRead.ReadDate}, : {s.LastRead.Value}, Count: {s.Values.Count}");
                }
            }
            #endregion
        }
        else //Unknown message.
        {
            AddMqttToUnknownList(mqtt);
            return -1;
        }
        if (UnknownMqttMessages.Any(m => m.Topic == mqtt.Topic)) UnknownMqttMessages.Remove(UnknownMqttMessages.First(m => m.Topic == mqtt.Topic));
        return 0;
    }

    private async Task EraseInvalidDataAndValueErrors<T>(T s, DateTime Now) where T : SensorModel
    {
        if (config.VerboseMode) Log.Information($"Sensor is valid; Value is valid. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
        s.Errors.EraseError(SensorErrorType.InvalidData, Now);
        s.Errors.EraseError(SensorErrorType.InvalidValue, Now);
        await DbProcessor.EraseSensorErrorFromDbAsync(s.Id, new[] { SensorErrorType.InvalidData, SensorErrorType.InvalidValue }, Now);
    }

    private async Task InvalidSensorValueHandler<T>(T s, MqttMessageModel mqtt, DateTime Now) where T : SensorModel
    {
        Log.Error($"A sensor sends invalid value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
        var newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.InvalidValue, Now, $"Data: {mqtt.Payload}");
        if (s.Errors.AddError(newErr, SensorErrorType.InvalidValue, config.system.MaxSensorErrorCount))
        {
            var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
            if (newId > 0) s.LastError.Id = newId;
        }
    }

    private async Task EraseSensorNotAliveErrorIfExists<T>(T s, DateTime Now) where T : SensorModel
    {
        if (config.VerboseMode) Log.Information($"Sensor ID is found in Poultries/Farms. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
        s.KeepAliveMessageDate = Now;
        s.Errors.EraseError(SensorErrorType.NotAlive, Now);
        await DbProcessor.EraseSensorErrorFromDbAsync(s.Id, new[] { SensorErrorType.NotAlive }, Now);
    }

    private async Task InvalidMqttPayloadHandler<T>(MqttMessageModel mqtt, List<T>? sensors, DateTime Now) where T : SensorModel
    {
        AddMqttToUnknownList(mqtt);
        if (sensors != null)
            foreach (var s in sensors)
            {
                if (s.IsEnabled)
                {
                    var newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.InvalidData, Now, $"Data: {mqtt.Payload}");
                    if (s.Errors.AddError(newErr, SensorErrorType.InvalidData, config.system.MaxSensorErrorCount))
                    {
                        var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
                        if (newId > 0) s.LastError.Id = newId;
                    }
                    s.Errors.EraseError(SensorErrorType.NotAlive, Now);
                    await DbProcessor.EraseSensorErrorFromDbAsync(s.Id, new[] { SensorErrorType.NotAlive }, Now);
                }                
            }
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
                if (s.IsEnabled)
                {
                    s.BatteryLevel = battery;
                    s.Errors.EraseError(SensorErrorType.NotAlive, now);
                    if (battery != -1 && battery <= config.system.SensorLowBatteryLevel)
                    {
                        var newErr = GenerateSensorError(s.AsBaseModel(), SensorErrorType.LowBattery, now, $"Level: {battery}");
                        s.Errors.AddError(newErr, SensorErrorType.LowBattery, config.system.MaxSensorErrorCount);
                        await DbProcessor.WriteSensorErrorToDbAsync(newErr, now);
                    }
                    else
                    {
                        s.Errors.EraseError(SensorErrorType.LowBattery, now);
                        await DbProcessor.EraseSensorErrorFromDbAsync(s.Id, new[] { SensorErrorType.LowBattery }, now);
                    }
                }
            }
            await DbProcessor.EraseSensorErrorFromDbAsync(sensorId, new[] { SensorErrorType.NotAlive }, now);
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
            foreach (var s in sensors)
            {
                if (s.IsEnabled)
                {
                    s.IPAddress = ip; s.Errors.EraseError(SensorErrorType.NotAlive, now);
                    await DbProcessor.EraseSensorErrorFromDbAsync(sensorId, new[] { SensorErrorType.NotAlive }, now);
                }
            }
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
            foreach (var s in sensors)
            {
                if (s.IsEnabled)
                {
                    s.KeepAliveMessageDate = now; s.Errors.EraseError(SensorErrorType.NotAlive, now);
                    await DbProcessor.EraseSensorErrorFromDbAsync(s.Id, new[] { SensorErrorType.NotAlive }, now);
                }
            }
        }
        return sensors.Count;
    }

    private void AddMqttToUnknownList(MqttMessageModel mqtt)
    {
        Log.Warning($"A sensor sends invalid data/value or sensor is unknown. adding to unknown mqtt list. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
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
            sensors.AddRange((IEnumerable<T>)(from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Scalars.Sensors)) where s != null && s.Id == id select s));
            sensors.AddRange((IEnumerable<T>)(from p in Poultries where p != null && p.Scalar != null && p.Scalar.Id == id select p.Scalar));
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
