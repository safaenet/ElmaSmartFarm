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
            #region Scalar Value Handler.
            else if (SubTopics[1] == config.mqtt.ScalarSubTopic) //Value from scalar sensor.
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is value from a scalar sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                var sensor = FindSensorsById<ScalarSensorModel>(SensorId);
                if (sensor == null || sensor.IsEnabled == false)
                {
                    AddMqttToUnknownList(mqtt);
                    return -1;
                }
                var payloads = mqtt.Payload.Split("/");
                if (payloads == null || payloads.Length == 0) //Invalid data.
                {
                    await InvalidMqttPayloadHandler(mqtt, sensor, Now);
                    return -1;
                }
                ScalarSensorReadModel newRead = new();
                foreach (var p in payloads)
                {
                    if (p.StartsWith("T:"))
                    {
                        var v = p.Replace("T:", "");
                        if (double.TryParse(v, out var temp)) newRead.Temperature = temp;
                        continue;
                    }
                    else if (p.StartsWith("H:"))
                    {
                        var v = p.Replace("H:", "");
                        if (int.TryParse(v, out var humid)) newRead.Humidity = humid;
                        continue;
                    }
                    else if (p.StartsWith("L:"))
                    {
                        var v = p.Replace("L:", "");
                        if (int.TryParse(v, out var light)) newRead.Light = light;
                        continue;
                    }
                    else if (p.StartsWith("A:"))
                    {
                        var v = p.Replace("A:", "");
                        if (double.TryParse(v, out var ammonia)) newRead.Ammonia = ammonia;
                        continue;
                    }
                    else if (p.StartsWith("C:"))
                    {
                        var v = p.Replace("C:", "");
                        if (double.TryParse(v, out var co2)) newRead.Co2 = co2;
                        continue;
                    }
                    //else await InvalidMqttPayloadHandler(mqtt, sensor, Now);
                }
                //Sensor found. Check values.
                await EraseSensorNotAliveErrorIfExists(sensor, Now);
                if (newRead.HasValidTemp(sensor.Type) == false)
                {
                    await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidTemperature, Now); //Invalid sensor temp value.
                    newRead.Temperature = null;
                }
                if (newRead.HasValidHumid() == false)
                {
                    await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidHumidity, Now); //Invalid sensor humid value.
                    newRead.Humidity = null;
                }
                if (newRead.HasValidLight() == false)
                {
                    await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidLight, Now); //Invalid sensor light value.
                    newRead.Light = null;
                }
                if (newRead.HasValidAmmonia() == false)
                {
                    await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidAmmonia, Now); //Invalid sensor ammonia value.
                    newRead.Ammonia = null;
                }
                if (newRead.HasValidCo2() == false)
                {
                    await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidCo2, Now); //Invalid sensor co2 value.
                    newRead.Co2 = null;
                }
                //Sensor Valid, Value Valid.

                await EraseInvalidDataAndValueErrors(sensor, Now);
                if (sensor.Values == null) sensor.Values = new();
                if ((sensor.IsWatched || config.system.WriteScalarToDbAlways) && (sensor.Values.Count == 0 || sensor.LastSavedRead == null || sensor.LastSavedRead.ReadDate.IsElapsed(config.system.WriteScalarToDbInterval))) //Writable to db.
                {
                    var newId = await DbProcessor.WriteSensorValueToDbAsync(sensor, Payload, Now, sensor.Offset);
                    if (newId > 0)
                    {
                        newRead.IsSavedToDb = true;
                        newRead.Id = newId;
                    }
                }
                if (sensor.Values.Count >= config.system.MaxSensorReadCount) sensor.Values.RemoveOldestNotSaved(config.VerboseMode);
                sensor.Values.Add(newRead);
                if (config.VerboseMode) Log.Information($"Temp sensor value done processing: {sensor.LastRead.ReadDate}, : {sensor.LastRead.Value}, Count: {sensor.Values.Count}");

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
                var sensor = FindSensorsById<CommuteSensorModel>(SensorId);
                if (sensor == null || sensor.IsEnabled == false)
                {
                    AddMqttToUnknownList(mqtt);
                    return -1;
                }
                if (!Enum.TryParse(mqtt.Payload, out CommuteSensorValueType Payload)) //Invalid data.
                {
                    await InvalidMqttPayloadHandler(mqtt, sensor, Now);
                    return -1;
                }
                //Sensor found. Sensor valid, Value valid.
                await EraseSensorNotAliveErrorIfExists(sensor, Now);
                await EraseInvalidDataAndValueErrors(sensor, Now);
                if (sensor.Values == null) sensor.Values = new();
                CommuteSensorReadModel newRead = new() { Value = Payload, ReadDate = Now };
                if ((sensor.IsWatched || config.system.WriteCommuteToDbAlways) && (sensor.Values.Count == 0 || (Payload == CommuteSensorValueType.StepIn && sensor.LastStepInSavedRead == null) || (Payload == CommuteSensorValueType.StepOut && sensor.LastStepOutSavedRead == null) || sensor.LastRead.Value != Payload)) //Writable to db.
                {
                    var newId = await DbProcessor.WriteSensorValueToDbAsync(sensor, (double)Payload, Now);
                    if (newId > 0)
                    {
                        newRead.IsSavedToDb = true;
                        newRead.Id = newId;
                    }
                }
                if (sensor.Values.Count >= config.system.MaxSensorReadCount) sensor.Values.RemoveOldestNotSaved(config.VerboseMode);
                sensor.Values.Add(newRead);
                if (config.VerboseMode) Log.Information($"Commute sensor value done processing: {sensor.LastRead.ReadDate}, : {sensor.LastRead.Value}, Count: {sensor.Values.Count}");
            }
            #endregion
            #region PushButton Value Handler.
            else if (SubTopics[1] == config.mqtt.PushButtonSubTopic) //Value from push button sensor.
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is value from a push button sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                var sensor = FindSensorsById<PushButtonSensorModel>(SensorId);
                if (sensor == null || sensor.IsEnabled == false)
                {
                    AddMqttToUnknownList(mqtt);
                    return -1;
                }
                //Sensor found. Check value. Sensor Valid, Value Valid.
                await EraseSensorNotAliveErrorIfExists(sensor, Now);
                if (sensor.Values == null) sensor.Values = new();
                PushButtonSensorReadModel newRead = new() { Value = Now, ReadDate = Now };
                if (sensor.IsWatched || config.system.WritePushButtonToDbAlways) //Writable to db.
                {
                    var newId = await DbProcessor.WriteSensorValueToDbAsync(sensor, 0, Now);
                    if (newId > 0)
                    {
                        newRead.IsSavedToDb = true;
                        newRead.Id = newId;
                    }
                }
                if (sensor.Values.Count >= config.system.MaxSensorReadCount) sensor.Values.RemoveOldestNotSaved(config.VerboseMode);
                sensor.Values.Add(newRead);
                if (config.VerboseMode) Log.Information($"PushButton sensor value done processing: {sensor.LastRead.ReadDate}, : {sensor.LastRead.Value}, Count: {sensor.Values.Count}");
            }
            #endregion
            #region Binary Value Handler.
            else if (SubTopics[1] == config.mqtt.BinarySubTopic) //Value from binary sensor.
            {
                if (config.VerboseMode) Log.Information($"MQTT Message is value from a binary sensor. Topic: {mqtt.Topic}, Payload: {mqtt.Payload}");
                var sensor = FindSensorsById<BinarySensorModel>(SensorId);
                if (sensor == null || sensor.IsEnabled == false)
                {
                    AddMqttToUnknownList(mqtt);
                    return -1;
                }
                if (!Enum.TryParse(mqtt.Payload, out BinarySensorValueType Payload)) //Invalid data.
                {
                    await InvalidMqttPayloadHandler(mqtt, sensor, Now);
                    return -1;
                }
                //Sensor found. Check value. Sensor Valid, Value Valid.
                await EraseSensorNotAliveErrorIfExists(sensor, Now);
                await EraseInvalidDataAndValueErrors(sensor, Now);
                if (sensor.Values == null) sensor.Values = new();
                BinarySensorReadModel newRead = new() { Value = Payload, ReadDate = Now };
                if ((sensor.IsWatched || config.system.WriteBinaryToDbAlways) && (sensor.Values.Count == 0 || sensor.LastSavedRead == null || (config.system.WriteBinaryOnValueChange && sensor.LastSavedRead.Value != Payload) || (Now - sensor.LastSavedRead.ReadDate).TotalSeconds >= config.system.WriteBinaryToDbInterval)) //Writable to db.
                {
                    var newId = await DbProcessor.WriteSensorValueToDbAsync(sensor, (double)Payload, Now);
                    if (newId > 0)
                    {
                        newRead.IsSavedToDb = true;
                        newRead.Id = newId;
                    }
                }
                if (sensor.Values.Count >= config.system.MaxSensorReadCount) sensor.Values.RemoveOldestNotSaved(config.VerboseMode);
                sensor.Values.Add(newRead);
                if (config.VerboseMode) Log.Information($"Binary sensor value done processing: {sensor.LastRead.ReadDate}, : {sensor.LastRead.Value}, Count: {sensor.Values.Count}");
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

    private async Task InvalidSensorValueHandler<T>(T s, MqttMessageModel mqtt, SensorErrorType e, DateTime Now) where T : SensorModel
    {
        Log.Error($"A sensor sends invalid value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
        var newErr = GenerateSensorError(s.AsBaseModel(), e, Now, $"Data: {mqtt.Payload}");
        if (s.Errors.AddError(newErr, e, config.system.MaxSensorErrorCount))
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

    private async Task InvalidMqttPayloadHandler(MqttMessageModel mqtt, SensorModel sensor, DateTime Now)
    {
        AddMqttToUnknownList(mqtt);
        if (sensor != null && sensor.IsEnabled)
        {
            var newErr = GenerateSensorError(sensor.AsBaseModel(), SensorErrorType.InvalidData, Now, $"Data: {mqtt.Payload}");
            if (sensor.Errors.AddError(newErr, SensorErrorType.InvalidData, config.system.MaxSensorErrorCount))
            {
                var newId = await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
                if (newId > 0) sensor.LastError.Id = newId;
            }
            sensor.Errors.EraseError(SensorErrorType.NotAlive, Now);
            await DbProcessor.EraseSensorErrorFromDbAsync(sensor.Id, new[] { SensorErrorType.NotAlive }, Now);
        }
    }

    private async Task<int> UpdateSensorBatteryLevelAsync(int sensorId, int battery, DateTime now)
    {
        if (Poultries == null) return 0;
        var sensor = FindSensorsById(sensorId);
        if (sensor != null && sensor.IsEnabled)
        {
            sensor.BatteryLevel = battery;
            sensor.Errors.EraseError(SensorErrorType.NotAlive, now);
            if (battery != -1 && battery <= config.system.SensorLowBatteryLevel)
            {
                var newErr = GenerateSensorError(sensor.AsBaseModel(), SensorErrorType.LowBattery, now, $"Level: {battery}");
                sensor.Errors.AddError(newErr, SensorErrorType.LowBattery, config.system.MaxSensorErrorCount);
                await DbProcessor.WriteSensorErrorToDbAsync(newErr, now);
            }
            else
            {
                sensor.Errors.EraseError(SensorErrorType.LowBattery, now);
                await DbProcessor.EraseSensorErrorFromDbAsync(sensor.Id, new[] { SensorErrorType.LowBattery }, now);
            }
            await DbProcessor.EraseSensorErrorFromDbAsync(sensorId, new[] { SensorErrorType.NotAlive }, now);
            return 1;
        }
        return 0;
    }

    private async Task<int> UpdateSensorIPAddressAsync(int sensorId, string ip, DateTime now)
    {
        if (Poultries == null) return 0;
        var sensor = FindSensorsById(sensorId);
        if (sensor != null && sensor.IsEnabled)
        {
            sensor.IPAddress = ip; sensor.Errors.EraseError(SensorErrorType.NotAlive, now);
            await DbProcessor.EraseSensorErrorFromDbAsync(sensorId, new[] { SensorErrorType.NotAlive }, now);
            return 1;
        }
        return 0;
    }

    private async Task<int> UpdateSensorKeepAliveAsync(int sensorId, DateTime now)
    {
        if (Poultries == null) return 0;
        if (config.system.KeepAliveInterval == 0)
        {
            Log.Warning($"KeepAlive is disabled but sensor a is sending KeepAlive message. Sensor ID: {sensorId}");
            return -1;
        }
        var sensor = FindSensorsById(sensorId);
        if (sensor != null && sensor.IsEnabled)
        {
            sensor.KeepAliveMessageDate = now; sensor.Errors.EraseError(SensorErrorType.NotAlive, now);
            await DbProcessor.EraseSensorErrorFromDbAsync(sensor.Id, new[] { SensorErrorType.NotAlive }, now);
            return 1;
        }
        return 0;
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
    private T? FindSensorsById<T>(int id) where T : SensorModel
    {
        if (Poultries == null) return null;
        T? sensor = null;
        if (typeof(T) == typeof(ScalarSensorModel))
        {
            var x = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Scalars.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
            if (x == null) x = (from p in Poultries where p != null && p.Scalar != null && p.Scalar.Id == id select p.Scalar).FirstOrDefault();
            if (x != null) sensor = (T?)Convert.ChangeType(x, typeof(T));
        }
        else if (typeof(T) == typeof(CommuteSensorModel))
            sensor = (T?)Convert.ChangeType((from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Commutes.Sensors)) where s != null && s.Id == id select s).FirstOrDefault(), typeof(T));
        else if (typeof(T) == typeof(PushButtonSensorModel))
        {
            var x = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Checkups.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
            if (x == null) x = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Feeds.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
            if (sensor != null) sensor = (T?)Convert.ChangeType(x, typeof(T));
        }
        else if (typeof(T) == typeof(BinarySensorModel))
        {
            var x = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.ElectricPowers.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
            if (x == null) x = (from p in Poultries where p != null && p.MainElectricPower != null && p.MainElectricPower.Id == id select p.MainElectricPower).FirstOrDefault();
            if (x == null) x = (from p in Poultries where p != null && p.BackupElectricPower != null && p.BackupElectricPower.Id == id select p.BackupElectricPower).FirstOrDefault();
            if(x!=null) sensor = (T?)Convert.ChangeType(x, typeof(T));
        }
        if (sensor != null) return sensor; else return null;
    }

    /// <summary>
    /// Looks for the sensors in all sensors of the Poultries.
    /// </summary>
    private SensorModel? FindSensorsById(int id)
    {
        if (Poultries == null) return null;
        SensorModel? sensor = null;
        sensor = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Scalars.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
        if (sensor == null) sensor = (from p in Poultries where p != null && p.Scalar != null && p.Scalar.Id == id select p.Scalar).FirstOrDefault();
        if (sensor == null) sensor = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Commutes.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
        if (sensor == null) sensor = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Checkups.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
        if (sensor == null) sensor = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Feeds.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
        if (sensor == null) sensor = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.ElectricPowers.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
        if (sensor == null) sensor = (from p in Poultries where p != null && p.MainElectricPower != null && p.MainElectricPower.Id == id select p.MainElectricPower).FirstOrDefault();
        if (sensor == null) sensor = (from p in Poultries where p != null && p.BackupElectricPower != null && p.BackupElectricPower.Id == id select p.BackupElectricPower).FirstOrDefault();
        return sensor;
    }
}