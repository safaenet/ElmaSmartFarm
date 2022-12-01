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
                    int SensorId = GetSensorIdFromTopic(mqtt, config.mqtt.FullTemperatureTopic + config.mqtt.KeepAliveSubTopic);
                    if(SensorId == 0)
                    {
                        //AddToUnknownList(); //Add mqtt to invalid list.
                        Log.Warning($"Invalid KeepAlive MQTT message. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        return -1;
                    }
                    var sensors = FindTempSensorsById(SensorId);
                    if (sensors == null)
                    {
                        //AddToUnknownList(); //Add sensor to unknown list.
                        Log.Warning($"Unknown Sensor sends KeepAlive. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        return -1;
                    }
                    foreach (var s in sensors) //Update KeepAlive and erase error if exists.
                    {
                        s.KeepAliveMessageDate = mqtt.ReadDate;
                        if (s.Errors != null) EraseSensorError(s.Errors, SensorErrorType.NotAlive);
                    }
                }
                else if (mqtt.Topic.StartsWith(config.mqtt.FullTemperatureTopic)) //Temp value
                {
                    int SensorId = GetSensorIdFromTopic(mqtt, config.mqtt.FullTemperatureTopic);
                    if (SensorId == 0)
                    {
                        //AddToUnknownList(); //Add mqtt to invalid list.
                        Log.Warning($"Invalid MQTT message. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        return -1;
                    }
                    var sensors = FindTempSensorsById(SensorId);
                    if (sensors == null)
                    {
                        //AddToUnknownList(); //Add sensor to unknown list.
                        Log.Warning($"Unknown Sensor sends temp value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        return -1;
                    }
                    if (!double.TryParse(mqtt.Payload, out double Payload))
                    {
                        Log.Warning($"A Sensor sends invalid value. Topic: {mqtt.Topic} , Payload: {mqtt.Payload}");
                        if (sensors != null) foreach (var s in sensors) AddSensorError(s.AsBaseModel(), s.Errors, SensorErrorType.InvalidValue);
                        return -1;
                    }
                    foreach (var s in sensors)
                    {
                        s.KeepAliveMessageDate = DateTime.Now;
                        EraseSensorError(s.Errors, SensorErrorType.NotAlive);
                        if ((s.Type == SensorType.FarmTemperature && (Payload < config.system.FarmTempMinValue || Payload > config.system.FarmTempMaxValue)) || (s.Type == SensorType.OutdoorTemperature && (Payload < config.system.OutdoorTempMinValue || Payload > config.system.OutdoorTempMaxValue))) //Invalid value.
                        {
                            AddSensorError(s.AsBaseModel(), s.Errors, SensorErrorType.InvalidValue);
                        }
                        else //Sensor OK, Value OK.
                        {
                            EraseSensorError(s.Errors, SensorErrorType.InvalidValue);
                            if (s.Values == null) s.Values = new();
                            SensorReadModel<double> newRead = new() { Value = Payload, ReadDate = DateTime.Now };
                            if (s.IsWatched && (s.Values.Count == 0 || s.LastSavedRead == null || Math.Abs(s.LastRead.Value - Payload) >= config.system.TempMaxDifferValue || (s.LastSavedRead != null && (DateTime.Now - s.LastSavedRead.ReadDate).TotalSeconds >= config.system.WriteTempToDbInterval)))
                            {
                                var newId = await DbProcessor.SaveSensorValueToDbAsync(s, Payload);
                                if (newId > 0) newRead.IsSavedToDb = true;
                            }
                            if (s.Values.Count >= config.MaxSensorReadCount)
                            {
                                s.Values.RemoveOldestNotSaved();
                            }
                            s.Values.Add(newRead);
                            Log.Information($"Sensor value received: {s.LastRead.ReadDate} : {s.LastRead.Value}");
                        }
                    }
                }
            }
            return 0;
        }

        private int GetSensorIdFromTopic(MqttMessageModel mqtt, string TopicTemplate)
        {
            var sensorId = mqtt.Topic.Replace(TopicTemplate, "");
            if (string.IsNullOrEmpty(sensorId)) return 0;
            if (!int.TryParse(sensorId, out int SensorId)) return 0;
            return SensorId;
        }

        private void EraseSensorError(List<SensorErrorModel> Errors, SensorErrorType errorType)
        {
            if (Errors == null) return;
            var e = Errors.FirstOrDefault(e => e.ErrorType == errorType);
            if (e == null) return;
            e.DateErased = DateTime.Now;
            //Update db.
        }

        private void AddSensorError(SensorBaseModel model, List<SensorErrorModel> Errors, SensorErrorType errorType)
        {
            if (Errors == null) Errors = new();
            var e = Errors.Where(e => e.ErrorType == errorType && e.DateErased == null);
            if (e != null && e.Count() > 0) return;
            SensorErrorModel newError = new()
            {
                SensorId = model.Id,
                ErrorType = errorType,
                LocationId = model.LocationId,
                Section = model.Section,
                DateHappened = DateTime.Now
            };
            Errors.Add(newError);
            //Update db.

            if (Errors.Count > config.MaxSensorErrorCount) //Remove oldest record.
            {
                var error = Errors.Where(x => x.DateErased != null)?.MinBy(x => x.DateErased);
                if (error != null) Errors.Remove(error);
                else Log.Warning($"Error count in Sensor Id: {Errors[0].SensorId} hass reached limit but not erased! (System Error).");
            }
        }

        private List<TemperatureSensorModel>? FindTempSensorsById(int id)
        {
            if (poultries == null) return null;
            List<TemperatureSensorModel>? sensors = new();
            sensors.AddRange(from p in poultries where p != null && p.OutdoorTemperature != null && p.OutdoorTemperature.Id == id select p.OutdoorTemperature);
            sensors.AddRange(from s in poultries.SelectMany(p => p.Farms.SelectMany(f => f.Temperatures.Sensors)) where s != null && s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private List<HumiditySensorModel>? FindHumidSensorsById(int id)
        {
            if (poultries == null) return null;
            List<HumiditySensorModel>? sensors = new();
            sensors.AddRange(from p in poultries where p != null && p.OutdoorHumidity.Id == id select p.OutdoorHumidity);
            sensors.AddRange(from s in poultries.SelectMany(p => p.Farms.SelectMany(f => f.Humidities.Sensors)) where s != null && s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private List<AmbientLightSensorModel>? FindAmbientLightSensorsById(int id)
        {
            if (poultries == null) return null;
            List<AmbientLightSensorModel>? sensors = new();
            sensors.AddRange(from s in poultries.SelectMany(p => p.Farms.SelectMany(f => f.AmbientLights.Sensors)) where s != null && s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private List<PushButtonSensorModel>? FindPushButtonSensorsById(int id)
        {
            if (poultries == null) return null;
            List<PushButtonSensorModel>? sensors = new();
            sensors.AddRange(from s in poultries.SelectMany(p => p.Farms.SelectMany(f => f.Checkups.Sensors)) where s != null && s.Id == id select s);
            sensors.AddRange(from s in poultries.SelectMany(p => p.Farms.SelectMany(f => f.Feeds.Sensors)) where s != null && s.Id == id select s);
            if (sensors.Count > 0) return sensors; else return null;
        }

        private List<BinarySensorModel>? FindBinarySensorsById(int id)
        {
            if (poultries == null) return null;
            List<BinarySensorModel>? sensors = new();
            sensors.AddRange(from p in poultries where p != null && p.MainElectricPower.Id == id select p.MainElectricPower);
            sensors.AddRange(from p in poultries where p != null && p.BackupElectricPower.Id == id select p.BackupElectricPower);
            sensors.AddRange(from s in poultries.SelectMany(p => p.Farms.SelectMany(f => f.ElectricPowers.Sensors)) where s != null && s.Id == id select s);
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