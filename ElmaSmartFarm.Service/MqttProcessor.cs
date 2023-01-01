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
            await ProcessMqttFromSensor(mqtt, Topic, Now);
        }
        else //Unknown message.
        {
            AddMqttToUnknownList(mqtt);
            return -1;
        }
        if (UnknownMqttMessages.Any(m => m.Topic == mqtt.Topic)) UnknownMqttMessages.Remove(UnknownMqttMessages.First(m => m.Topic == mqtt.Topic));
        return 0;
    }

    private async Task<int> ProcessMqttFromSensor(MqttMessageModel mqtt, string Topic, DateTime Now)
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
            await EraseSensorErrors(sensor, Now);
            var payloads = mqtt.Payload.Split("/");
            if (payloads == null || payloads.Length == 0) //Invalid data.
            {
                await InvalidMqttPayloadHandler(mqtt, sensor, Now);
                return -1;
            }
            ScalarSensorReadModel newRead = new();
            newRead.ReadDate = Now;
            foreach (var p in payloads)
            {
                if (p.StartsWith("T:"))
                {
                    var v = p.Replace("T:", "");
                    if (double.TryParse(v, out var value))
                    {
                        newRead.Temperature = value + sensor.TemperatureOffset;
                        await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidTemperatureData);
                        if (newRead.HasValidTemp(sensor.Type, config.system.FarmTempMinValue, config.system.FarmTempMaxValue, config.system.OutdoorTempMinValue, config.system.OutdoorTempMaxValue))
                        {
                            await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidTemperatureValue);
                            if (sensor.IsFarmSensor() && sensor.IsWatched && sensor.IsInPeriod)
                            {
                                var farm = FindFarmBySensorId(sensor.Id);
                                if (farm == null) Log.Error($"Farm for the indoor sensor not detected. Sensor ID: {sensor.Id} (System Error)");
                                else
                                {
                                    if (newRead.Temperature < config.system.TempMinWorkingValue)
                                        await AddkValueRangeFarmError(farm, sensor, newRead, FarmInPeriodErrorType.LowTemperature, FarmInPeriodErrorType.HighTemperature, Now);
                                    else if (newRead.Temperature > config.system.TempMaxWorkingValue)
                                        await AddkValueRangeFarmError(farm, sensor, newRead, FarmInPeriodErrorType.HighTemperature, FarmInPeriodErrorType.LowTemperature, Now);
                                    else if (farm != null) await EraseFarmErrors(farm, Now, FarmInPeriodErrorType.LowTemperature, FarmInPeriodErrorType.HighTemperature);
                                }
                            }
                        }
                        else
                        {
                            newRead.Temperature = null;
                            await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidTemperatureValue, Now);
                        }
                    }
                    else await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidTemperatureData, Now); //Invalid sensor temp data.
                    continue;
                }
                else if (p.StartsWith("H:"))
                {
                    var v = p.Replace("H:", "");
                    if (int.TryParse(v, out var value))
                    {
                        newRead.Humidity = value + sensor.HumidityOffset;
                        await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidHumidityData);
                        if (newRead.HasValidHumid(config.system.HumidMinValue, config.system.HumidMaxValue))
                        {
                            await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidHumidityValue);
                            if (sensor.IsFarmSensor() && sensor.IsWatched && sensor.IsInPeriod)
                            {
                                var farm = FindFarmBySensorId(sensor.Id);
                                if (farm == null) Log.Error($"Farm for the indoor sensor not detected. Sensor ID: {sensor.Id} (System Error)");
                                else
                                {
                                    if (newRead.Humidity < config.system.HumidMinWorkingValue)
                                        await AddkValueRangeFarmError(farm, sensor, newRead, FarmInPeriodErrorType.LowHumidity, FarmInPeriodErrorType.HighHumidity, Now);
                                    else if (newRead.Humidity > config.system.HumidMaxWorkingValue)
                                        await AddkValueRangeFarmError(farm, sensor, newRead, FarmInPeriodErrorType.HighHumidity, FarmInPeriodErrorType.LowHumidity, Now);
                                    else if (farm != null) await EraseFarmErrors(farm, Now, FarmInPeriodErrorType.LowHumidity, FarmInPeriodErrorType.HighHumidity);
                                }
                            }
                        }
                        else
                        {
                            newRead.Humidity = null;
                            await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidHumidityValue, Now);
                        }
                    }
                    else await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidHumidityData, Now); //Invalid sensor humid value.
                    continue;
                }
                else if (p.StartsWith("L:"))
                {
                    var v = p.Replace("L:", "");
                    if (int.TryParse(v, out var value))
                    {
                        newRead.Light = value + sensor.LightOffset;
                        await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidLightData);
                        if (newRead.HasValidLight(config.system.AmbientLightMinValue, config.system.AmbientLightMaxValue)) await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidLightValue);
                        else
                        {
                            newRead.Light = null;
                            await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidLightValue, Now);
                        }
                    }
                    else await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidLightData, Now); //Invalid sensor light value.
                    continue;
                }
                else if (p.StartsWith("A:"))
                {
                    var v = p.Replace("A:", "");
                    if (double.TryParse(v, out var value))
                    {
                        newRead.Ammonia = value + sensor.AmmoniaOffset;
                        await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidAmmoniaData);
                        if (newRead.HasValidAmmonia(config.system.AmmoniaMinValue, config.system.AmmoniaMaxValue))
                        {
                            await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidAmmoniaValue);
                            if (sensor.IsFarmSensor() && sensor.IsWatched && sensor.IsInPeriod)
                            {
                                var farm = FindFarmBySensorId(sensor.Id);
                                if (farm == null) Log.Error($"Farm for the indoor sensor not detected. Sensor ID: {sensor.Id} (System Error)");
                                else
                                {
                                    if (newRead.Ammonia > config.system.AmmoniaMaxWorkingValue)
                                        await AddkValueRangeFarmError(farm, sensor, newRead, FarmInPeriodErrorType.HighAmmonia, null, Now);
                                    else if (farm != null) await EraseFarmErrors(farm, Now, FarmInPeriodErrorType.HighAmmonia);
                                }
                            }
                        }
                        else
                        {
                            newRead.Ammonia = null;
                            await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidAmmoniaValue, Now);
                        }
                    }
                    else await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidAmmoniaData, Now); //Invalid sensor ammonia value.
                    continue;
                }
                else if (p.StartsWith("C:"))
                {
                    var v = p.Replace("C:", "");
                    if (double.TryParse(v, out var value))
                    {
                        newRead.Co2 = value + sensor.Co2Offset;
                        await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidCo2Data);
                        if (newRead.HasValidCo2(config.system.Co2MinValue, config.system.Co2MaxValue))
                        {
                            await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidCo2Value);
                            if (sensor.IsFarmSensor() && sensor.IsWatched && sensor.IsInPeriod)
                            {
                                var farm = FindFarmBySensorId(sensor.Id);
                                if (farm == null) Log.Error($"Farm for the indoor sensor not detected. Sensor ID: {sensor.Id} (System Error)");
                                else
                                {
                                    if (newRead.Co2 > config.system.AmmoniaMaxWorkingValue)
                                        await AddkValueRangeFarmError(farm, sensor, newRead, FarmInPeriodErrorType.HighCo2, null, Now);
                                    else if (farm != null) await EraseFarmErrors(farm, Now, FarmInPeriodErrorType.HighCo2);
                                }
                            }
                        }
                        else
                        {
                            newRead.Co2 = null;
                            await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidCo2Value, Now);
                        }
                    }
                    else await InvalidSensorValueHandler(sensor, mqtt, SensorErrorType.InvalidCo2Data, Now); //Invalid sensor co2 value.
                    continue;
                }
                //else await InvalidMqttPayloadHandler(mqtt, sensor, Now);
            }
            await EraseSensorErrors(sensor, Now, SensorErrorType.NotAlive);
            if (newRead.HasValue == false)
            {
                await InvalidMqttPayloadHandler(mqtt, sensor, Now);
                return -1;
            }
            else await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidData);
            //Sensor Valid, Values Valid.
            if (sensor.Values == null) sensor.Values = new();
            if ((sensor.IsWatched || config.system.WriteScalarToDbAlways) && (sensor.Values.Count == 0 || sensor.LastSavedRead == null || sensor.LastSavedRead.ReadDate.IsElapsed(config.system.WriteScalarToDbInterval))) //Writable to db.
            {
                var newId = await DbProcessor.WriteScalarSensorValueToDbAsync(sensor, newRead);
                if (newId > 0)
                {
                    newRead.IsSavedToDb = true;
                    newRead.Id = newId;
                }
            }
            if (sensor.Values.Count >= config.system.MaxSensorReadCount) sensor.Values.RemoveOldestNotSaved(config.VerboseMode);
            sensor.Values.Add(newRead);
            if (config.VerboseMode) Log.Information($"Scalar sensor value done processing. Count: {sensor.Values.Count}");
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
            await EraseSensorErrors(sensor, Now, SensorErrorType.NotAlive);
            if (!Enum.TryParse(mqtt.Payload, out CommuteSensorValueType Payload)) //Invalid data.
            {
                await InvalidMqttPayloadHandler(mqtt, sensor, Now);
                return -1;
            }
            //Sensor found. Sensor valid, Value valid.
            await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidData);
            if (sensor.Values == null) sensor.Values = new();
            CommuteSensorReadModel newRead = new() { Value = Payload, ReadDate = Now };
            if ((sensor.IsWatched || config.system.WriteCommuteToDbAlways) && (sensor.Values.Count == 0 || sensor.LastRead.Value != Payload || (Payload == CommuteSensorValueType.StepIn && sensor.LastStepInSavedRead == null) || (Payload == CommuteSensorValueType.StepOut && sensor.LastStepOutSavedRead == null))) //Writable to db.
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
            var farm = FindFarmBySensorId(sensor.Id);
            if (farm != null) await EraseFarmErrors(farm, Now, FarmInPeriodErrorType.LongLeave);
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
            await EraseSensorErrors(sensor, Now, SensorErrorType.NotAlive);
            if (sensor.Values == null) sensor.Values = new();
            PushButtonSensorReadModel newRead = new() { Value = Now, ReadDate = Now };
            if (sensor.IsWatched || (sensor.Type == SensorType.FarmFeed && config.system.WriteFeedToDbAlways) || (sensor.Type == SensorType.FarmCheckup && config.system.WriteCheckupToDbAlways)) //Writable to db.
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
            var farm = FindFarmBySensorId(sensor.Id);
            if (farm != null)
            {
                if (sensor.Type == SensorType.FarmFeed) await EraseFarmErrors(farm, Now, FarmInPeriodErrorType.LongNoFeed);
                else if (sensor.Type == SensorType.FarmCheckup) await EraseFarmErrors(farm, Now, FarmInPeriodErrorType.LongLeave);
            }
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
            await EraseSensorErrors(sensor, Now, SensorErrorType.NotAlive);
            if (!Enum.TryParse(mqtt.Payload, out BinarySensorValueType Payload)) //Invalid data.
            {
                await InvalidMqttPayloadHandler(mqtt, sensor, Now);
                return -1;
            }
            //Sensor found. Check value. Sensor Valid, Value Valid.
            await EraseSensorErrors(sensor, Now, SensorErrorType.InvalidData);
            if (sensor.Values == null) sensor.Values = new();
            BinarySensorReadModel newRead = new() { Value = Payload, ReadDate = Now };
            bool WriteToDbAways = false;
            bool WritePowerOnValueChange = false;
            int WriteToDbInterval = 0;
            if (sensor.Type == SensorType.FarmElectricPower)
            {
                WriteToDbAways = config.system.WriteFarmPowerToDbAlways;
                WriteToDbInterval = config.system.WriteFarmPowerToDbInterval;
                WritePowerOnValueChange = config.system.WriteFarmPowerOnValueChange;
            }
            if (sensor.Type == SensorType.PoultryMainElectricPower)
            {
                WriteToDbAways = config.system.WriteMainPowerToDbAlways;
                WriteToDbInterval = config.system.WriteMainPowerToDbInterval;
                WritePowerOnValueChange = config.system.WriteMainPowerOnValueChange;

            }
            if (sensor.Type == SensorType.PoultryBackupElectricPower)
            {
                WriteToDbAways = config.system.WriteBackupPowerToDbAlways;
                WriteToDbInterval = config.system.WriteBackupPowerToDbInterval;
                WritePowerOnValueChange = config.system.WriteBackupPowerOnValueChange;
            }
            if ((sensor.IsWatched || WriteToDbAways) && (sensor.Values.Count == 0 || sensor.LastSavedRead == null || (WritePowerOnValueChange && sensor.LastSavedRead.Value != Payload) || sensor.LastSavedRead.ReadDate.IsElapsed(WriteToDbInterval))) //Writable to db.
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

            if (sensor.IsInPeriod)
            {
                if (sensor.Type == SensorType.FarmElectricPower)
                {
                    if (newRead.Value == BinarySensorValueType.Off)
                    {
                        if (config.VerboseMode) Log.Warning($"{FarmInPeriodErrorType.NoPower} detected in one of farms. sensor ID: {sensor.Id}");
                        var farm = FindFarmBySensorId(sensor.Id);
                        if (farm == null) Log.Error($"Farm for the indoor sensor not detected. Sensor ID: {sensor.Id} (System Error)");
                        else
                        {
                            if (config.VerboseMode) Log.Warning($"{FarmInPeriodErrorType.NoPower} detected in farm ID: {farm.Id}, Name: {farm.Name}. sensor ID: {sensor.Id}");
                            var newErr = GenerateFarmError(sensor, FarmInPeriodErrorType.NoPower, Now, farm.Period?.Id ?? 0, $"{FarmInPeriodErrorType.NoPower} on: {newRead.ReadDate}, Detected by: {sensor.Type}");
                            if (farm.InPeriodErrors == null) farm.InPeriodErrors = new();
                            if (farm.InPeriodErrors.AddError(newErr, FarmInPeriodErrorType.NoPower, config.system.MaxFarmErrorCount))
                            {
                                var newId = await DbProcessor.WriteFarmErrorToDbAsync(newErr, Now);
                                if (newId > 0) newErr.Id = newId;
                            }
                        }
                    }
                    else if (newRead.Value == BinarySensorValueType.On)
                    {
                        var farm = FindFarmBySensorId(sensor.Id);
                        if (farm != null) await EraseFarmErrors(farm, Now, FarmInPeriodErrorType.NoPower);
                    }
                }
                else if (sensor.Type == SensorType.PoultryMainElectricPower)
                {
                    if (newRead.Value == BinarySensorValueType.Off)
                    {
                        if (config.VerboseMode) Log.Warning($"{PoultryInPeriodErrorType.NoMainPower} detected in poultry. sensor ID: {sensor.Id}");
                        var newErr = GeneratePoultryError(sensor, PoultryInPeriodErrorType.NoMainPower, Now, $"{PoultryInPeriodErrorType.NoMainPower} on: {newRead.ReadDate}, Detected by: {sensor.Type}");
                        if (Poultry.InPeriodErrors == null) Poultry.InPeriodErrors = new();
                        if (Poultry.InPeriodErrors.AddError(newErr, config.system.MaxPoultryErrorCount))
                        {
                            var newId = await DbProcessor.WritePoultryErrorToDbAsync(newErr, Now);
                            if (newId > 0) newErr.Id = newId;
                        }
                    }
                    else if (newRead.Value == BinarySensorValueType.On)
                    {
                        await ErasePoultryErrors(Now, PoultryInPeriodErrorType.NoMainPower);
                    }
                }
                else if (sensor.Type == SensorType.PoultryBackupElectricPower)
                {
                    if (newRead.Value == BinarySensorValueType.Off)
                    {
                        if (config.VerboseMode) Log.Warning($"{PoultryInPeriodErrorType.NoBackupPower} detected in poultry. sensor ID: {sensor.Id}");
                        var newErr = GeneratePoultryError(sensor, PoultryInPeriodErrorType.NoBackupPower, Now, $"{PoultryInPeriodErrorType.NoBackupPower} on: {newRead.ReadDate}, Detected by: {sensor.Type}");
                        if (Poultry.InPeriodErrors == null) Poultry.InPeriodErrors = new();
                        if (Poultry.InPeriodErrors.AddError(newErr, config.system.MaxPoultryErrorCount))
                        {
                            var newId = await DbProcessor.WritePoultryErrorToDbAsync(newErr, Now);
                            if (newId > 0) newErr.Id = newId;
                        }
                    }
                    else if (newRead.Value == BinarySensorValueType.On)
                    {
                        await ErasePoultryErrors(Now, PoultryInPeriodErrorType.NoBackupPower);
                    }
                }
            }

            if (config.VerboseMode) Log.Information($"Binary sensor value done processing: {sensor.LastRead.ReadDate}, : {sensor.LastRead.Value}, Count: {sensor.Values.Count}");
        }
        #endregion
        return 0;
    }

    private async Task AddkValueRangeFarmError(FarmModel farm, SensorModel sensor, ScalarSensorReadModel newRead, FarmInPeriodErrorType ErrorToAdd, FarmInPeriodErrorType? ErrorToErase, DateTime Now)
    {
        await AddFarmError(farm, sensor, newRead.ReadDate, ErrorToAdd, Now);
        if (ErrorToErase.HasValue) await EraseFarmErrors(farm, Now, ErrorToErase.Value);
    }

    private async Task EraseSensorErrors<T>(T s, DateTime Now, params SensorErrorType[] types) where T : SensorModel
    {
        if (types == null || types.Length == 0) return;
        if (config.VerboseMode) Log.Information($"Sensor is valid; Value is valid. Type: {s.Type}, LocationID: {s.LocationId}, Section: {s.Section}");
        foreach (var e in types)
        {
            s.Errors.EraseError(e, Now);
            if (e == SensorErrorType.NotAlive) s.KeepAliveMessageDate = Now;
        }
        await DbProcessor.EraseSensorErrorFromDbAsync(s.Id, Now, types);
    }

    private async Task EraseFarmErrors(FarmModel f, DateTime Now, params FarmInPeriodErrorType[] types)
    {
        if (types == null || types.Length == 0) return;
        foreach (var e in types)
        {
            f.InPeriodErrors.EraseError(e, Now);
        }
        await DbProcessor.EraseFarmErrorFromDbAsync(f.Id, Now, types);
    }

    private async Task ErasePoultryErrors(DateTime Now, params PoultryInPeriodErrorType[] types)
    {
        if (types == null || types.Length == 0) return;
        foreach (var e in types)
        {
            Poultry.InPeriodErrors.EraseError(e, Now);
        }
        await DbProcessor.ErasePoultryErrorFromDbAsync(Now, types);
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
            //sensor.Errors.EraseError(SensorErrorType.NotAlive, Now);
            //await DbProcessor.EraseSensorErrorFromDbAsync(sensor.Id, new[] { SensorErrorType.NotAlive }, Now);
        }
    }

    private async Task<int> UpdateSensorBatteryLevelAsync(int sensorId, int battery, DateTime now)
    {
        if (Poultry == null) return 0;
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
                await DbProcessor.EraseSensorErrorFromDbAsync(sensor.Id, now, SensorErrorType.LowBattery);
            }
            await DbProcessor.EraseSensorErrorFromDbAsync(sensorId, now, SensorErrorType.NotAlive);
            return 1;
        }
        return 0;
    }

    private async Task<int> UpdateSensorIPAddressAsync(int sensorId, string ip, DateTime now)
    {
        if (Poultry == null) return 0;
        var sensor = FindSensorsById(sensorId);
        if (sensor != null && sensor.IsEnabled)
        {
            sensor.IPAddress = ip; sensor.Errors.EraseError(SensorErrorType.NotAlive, now);
            await DbProcessor.EraseSensorErrorFromDbAsync(sensorId, now, SensorErrorType.NotAlive);
            return 1;
        }
        return 0;
    }

    private async Task<int> UpdateSensorKeepAliveAsync(int sensorId, DateTime now)
    {
        if (Poultry == null) return 0;
        if (config.system.IsKeepAliveEnabled == false)
        {
            Log.Warning($"KeepAlive is disabled but sensor a is sending KeepAlive message. Sensor ID: {sensorId}");
            return -1;
        }
        var sensor = FindSensorsById(sensorId);
        if (sensor != null && sensor.IsEnabled)
        {
            sensor.KeepAliveMessageDate = now;
            sensor.Errors.EraseError(SensorErrorType.NotAlive, now);
            await DbProcessor.EraseSensorErrorFromDbAsync(sensor.Id, now, SensorErrorType.NotAlive);
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

    private static FarmInPeriodErrorModel GenerateFarmError(SensorModel sensor, FarmInPeriodErrorType type, DateTime Now, int periodId, string description = "")
    {
        return new FarmInPeriodErrorModel()
        {
            FarmId = sensor.LocationId,
            PeriodId = periodId,
            ErrorType = type,
            DateHappened = Now,
            CausedSensorId = sensor.Id,
            Descriptions = description
        };
    }

    private static PoultryInPeriodErrorModel GeneratePoultryError(SensorModel sensor, PoultryInPeriodErrorType type, DateTime Now, string description = "")
    {
        return new PoultryInPeriodErrorModel()
        {
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
        if (Poultry == null) return null;
        T? sensor = null;
        if (typeof(T) == typeof(ScalarSensorModel))
        {
            var x = (from s in Poultry.Farms?.SelectMany(f => f.Scalars.Sensors) where s != null && s.Id == id select s).FirstOrDefault();
            if (x == null) x = Poultry.Scalar?.Id == id ? Poultry.Scalar : null;
            if (x != null) sensor = (T?)Convert.ChangeType(x, typeof(T));
        }
        else if (typeof(T) == typeof(CommuteSensorModel))
            sensor = (T?)Convert.ChangeType((from s in Poultry.Farms.SelectMany(f => f.Commutes.Sensors) where s != null && s.Id == id select s).FirstOrDefault(), typeof(T));
        else if (typeof(T) == typeof(PushButtonSensorModel))
        {
            var x = (from s in Poultry.Farms?.SelectMany(f => f.Checkups.Sensors) where s != null && s.Id == id select s).FirstOrDefault();
            if (x == null) x = (from s in Poultry.Farms?.SelectMany(f => f.Feeds.Sensors) where s != null && s.Id == id select s).FirstOrDefault();
            if (x != null) sensor = (T?)Convert.ChangeType(x, typeof(T));
        }
        else if (typeof(T) == typeof(BinarySensorModel))
        {
            var x = (from s in Poultry.Farms?.SelectMany(f => f.ElectricPowers.Sensors) where s != null && s.Id == id select s).FirstOrDefault();
            if (x == null) x = Poultry.MainElectricPower?.Id == id ? Poultry.MainElectricPower : null;
            if (x == null) x = Poultry.BackupElectricPower?.Id == id ? Poultry.BackupElectricPower : null;
            if (x != null) sensor = (T?)Convert.ChangeType(x, typeof(T));
        }
        //else if (typeof(T) == typeof(SensorModel))
        //{
        //    var x = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Scalars.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
        //    if (x == null) x = (from p in Poultries where p != null && p.Scalar != null && p.Scalar.Id == id select p.Scalar).FirstOrDefault();
        //    if (x != null) sensor = (T?)Convert.ChangeType(x, typeof(T));

        //    if(sensor == null) sensor = (T?)Convert.ChangeType((from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Commutes.Sensors)) where s != null && s.Id == id select s).FirstOrDefault(), typeof(T));

        //    if (sensor == null)
        //    {
        //        var y = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Checkups.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
        //        if (y == null) y = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.Feeds.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
        //        if (y != null) sensor = (T?)Convert.ChangeType(y, typeof(T));
        //    }
        //    if (sensor == null)
        //    {
        //        var y = (from s in Poultries.SelectMany(p => p.Farms.SelectMany(f => f.ElectricPowers.Sensors)) where s != null && s.Id == id select s).FirstOrDefault();
        //        if (y == null) y = (from p in Poultries where p != null && p.MainElectricPower != null && p.MainElectricPower.Id == id select p.MainElectricPower).FirstOrDefault();
        //        if (y == null) y = (from p in Poultries where p != null && p.BackupElectricPower != null && p.BackupElectricPower.Id == id select p.BackupElectricPower).FirstOrDefault();
        //        if (y != null) sensor = (T?)Convert.ChangeType(x, typeof(T));
        //    }
        //}
        if (sensor != null) return sensor; else return null;
    }

    /// <summary>
    /// Looks for the sensors in all sensors of the Poultries.
    /// </summary>
    private SensorModel? FindSensorsById(int id)
    {
        if (Poultry == null) return null;
        SensorModel? sensor = null;
        sensor = (from s in Poultry.Farms.SelectMany(f => f.Scalars.Sensors) where s != null && s.Id == id select s).FirstOrDefault();
        if (sensor == null) sensor = Poultry.Scalar.Id == id ? Poultry.Scalar : null;
        if (sensor == null) sensor = (from s in Poultry.Farms.SelectMany(f => f.Commutes.Sensors) where s != null && s.Id == id select s).FirstOrDefault();
        if (sensor == null) sensor = (from s in Poultry.Farms.SelectMany(f => f.Checkups.Sensors) where s != null && s.Id == id select s).FirstOrDefault();
        if (sensor == null) sensor = (from s in Poultry.Farms.SelectMany(f => f.Feeds.Sensors) where s != null && s.Id == id select s).FirstOrDefault();
        if (sensor == null) sensor = (from s in Poultry.Farms.SelectMany(f => f.ElectricPowers.Sensors) where s != null && s.Id == id select s).FirstOrDefault();
        if (sensor == null) sensor = Poultry.MainElectricPower.Id == id ? Poultry.MainElectricPower : null;
        if (sensor == null) sensor = Poultry.BackupElectricPower.Id == id ? Poultry.BackupElectricPower : null;
        return sensor;
    }
}