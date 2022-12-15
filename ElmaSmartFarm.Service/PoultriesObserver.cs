using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.Models.Alarm;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using Serilog;

namespace ElmaSmartFarm.Service;

public partial class Worker
{
    private async Task<string?> ObservePoultryAsync()
    {
        var Now = DateTime.Now;

        #region Observe Farm Scalar Sensors.
        var FarmScalarSets = Poultry.Farms.Select(f => f.Scalars);
        if (FarmScalarSets != null)
            foreach (var set in FarmScalarSets)
            {
                if (set.HasSensors)
                {
                    foreach (var sensor in set.EnabledSensors)
                    {
                        if (sensor.IsWatched) await CheckKeepAliveMessageDate(sensor, config.system.KeepAliveWaitingTimeout, Now);
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                var alarmTimes = GetAlarmTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.InvalidTemperatureData)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchTemperature) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if(sensor.WatchTemperature) sensor.WatchTemperature = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchTemperature, sensor.IsInPeriod, config.system.TempInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidTemperatureValue)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchTemperature) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchTemperature) sensor.WatchTemperature = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchTemperature, sensor.IsInPeriod, config.system.TempInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidHumidityData)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchHumidity) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchHumidity) sensor.WatchHumidity = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchHumidity, sensor.IsInPeriod, config.system.HumidInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidHumidityValue)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchHumidity) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchHumidity) sensor.WatchHumidity = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchHumidity, sensor.IsInPeriod, config.system.HumidInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidLightData)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchLight) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchLight) sensor.WatchLight = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchLight, sensor.IsInPeriod, config.system.AmbientLightInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidLightValue)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchLight) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchLight) sensor.WatchLight = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchLight, sensor.IsInPeriod, config.system.AmbientLightInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidAmmoniaData)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchAmmonia) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchAmmonia) sensor.WatchAmmonia = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchAmmonia, sensor.IsInPeriod, config.system.AmmoniaInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidAmmoniaValue)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchAmmonia) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchAmmonia) sensor.WatchAmmonia = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchAmmonia, sensor.IsInPeriod, config.system.AmmoniaInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidCo2Data)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchCo2) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchCo2) sensor.WatchCo2 = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchCo2, sensor.IsInPeriod, config.system.Co2InvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidCo2Value)
                                {
                                    if (alarmTimes.Enable && (sensor.WatchCo2) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.WatchCo2) sensor.WatchCo2 = !CheckToUnWatch(e, sensor.IsWatched, sensor.WatchCo2, sensor.IsInPeriod, config.system.Co2InvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.ScalarNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                }
                            }
                            if (sensor.IsWatched && !sensor.WatchTemperature && !sensor.WatchHumidity && !sensor.WatchLight && !sensor.WatchAmmonia && !sensor.WatchCo2) //Sensor is fully damaged.
                            {
                                Log.Information($"All units of the sensor is out of watch; Putting the whole sensor out of watch, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}.");
                                sensor.IsWatched = false;
                                //inform, save to db
                            }
                        }
                        var startDate = Poultry.Farms.Where(f => f.IsInPeriod && f.Scalars.Sensors.Any(s => s.Id == sensor.Id)).FirstOrDefault()?.Period.StartDate;
                        if (!sensor.WatchTemperature && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidTemperatureData || e.ErrorType == SensorErrorType.InvalidTemperatureValue) == false) //Temp sensor is healthy.
                            sensor.WatchTemperature = CheckToReWatchSensor(sensor, startDate);
                        if (!sensor.WatchHumidity && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidHumidityData || e.ErrorType == SensorErrorType.InvalidHumidityValue) == false) //Humidity sensor is healthy.
                            sensor.WatchHumidity = CheckToReWatchSensor(sensor, startDate);
                        if (!sensor.WatchLight && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidLightData || e.ErrorType == SensorErrorType.InvalidLightValue) == false) //Light sensor is healthy.
                            sensor.WatchLight = CheckToReWatchSensor(sensor, startDate);
                        if (!sensor.WatchAmmonia && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidAmmoniaData || e.ErrorType == SensorErrorType.InvalidAmmoniaValue) == false) //Ammonia sensor is healthy.
                            sensor.WatchAmmonia = CheckToReWatchSensor(sensor, startDate);
                        if (!sensor.WatchCo2 && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidCo2Data || e.ErrorType == SensorErrorType.InvalidCo2Value) == false) //Co2 sensor is healthy.
                            sensor.WatchCo2 = CheckToReWatchSensor(sensor, startDate);
                        if (!sensor.IsWatched && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.NotAlive) == false && (sensor.WatchTemperature || sensor.WatchHumidity || sensor.WatchLight || sensor.WatchAmmonia || sensor.WatchCo2)) sensor.IsWatched = CheckToReWatchSensor(sensor, startDate); //Sensor is alive and healthy (again).

                        //Remove expired reads
                    }
                }
                if (set.MinimumTemperatureSensor.LastRead.Temperature < config.system.TempMinWorkingValue)
                {

                }
            }
        FarmScalarSets = null;
        #endregion
        #region Observe Commute Sensors.
        var CommuteSets = Poultry.Farms.Select(f => f.Commutes);
        if (CommuteSets != null)
            foreach (var set in CommuteSets)
            {
                if (set.HasSensors)
                {
                    foreach (var sensor in set.EnabledSensors)
                    {
                        if (sensor.IsWatched) await CheckKeepAliveMessageDate(sensor, config.system.KeepAliveWaitingTimeout, Now);
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                var alarmTimes = GetAlarmTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.InvalidData)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.InvalidValue)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteInvalidValueWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, config.system.CommuteNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                }
                            }
                        }

                        var startDate = Poultry.Farms.Where(f => f.IsInPeriod && f.Commutes.Sensors.Any(s => s.Id == sensor.Id)).FirstOrDefault()?.Period.StartDate;
                        if (!sensor.IsWatched && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.InvalidData || e.ErrorType == SensorErrorType.InvalidValue || e.ErrorType == SensorErrorType.NotAlive) == false) //Sensor is healthy.
                            sensor.IsWatched = CheckToReWatchSensor(sensor, startDate);

                        //Remove expired reads
                    }
                }
            }
        CommuteSets = null;
        #endregion
        #region Observe PushButton Sensors.
        var CheckupSets = Poultry.Farms.Select(f => f.Checkups);
        var FeedSets = Poultry.Farms.Select(f => f.Feeds);
        var PushButtonSets = CheckupSets.Concat(FeedSets);
        if (PushButtonSets != null)
            foreach (var set in PushButtonSets)
            {
                if (set.HasSensors)
                {
                    foreach (var sensor in set.EnabledSensors)
                    {
                        if (sensor.IsWatched) await CheckKeepAliveMessageDate(sensor, config.system.KeepAliveWaitingTimeout, Now);
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                var alarmTimes = GetAlarmTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    int PushButtonNotAliveWatchTimeout = 0;
                                    if (sensor.Type == SensorType.FarmFeed) PushButtonNotAliveWatchTimeout = config.system.FeedNotAliveWatchTimeout;
                                    else if (sensor.Type == SensorType.FarmCheckup) PushButtonNotAliveWatchTimeout = config.system.CheckupNotAliveWatchTimeout;
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, PushButtonNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                }
                            }
                        }

                        var startDate = Poultry.Farms.Where(f => f.IsInPeriod && f.Checkups.Sensors.Any(s => s.Id == sensor.Id)).FirstOrDefault()?.Period.StartDate;
                        if (startDate == null) startDate = Poultry.Farms.Where(f => f.IsInPeriod && f.Feeds.Sensors.Any(s => s.Id == sensor.Id)).FirstOrDefault()?.Period.StartDate;
                        if (!sensor.IsWatched && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.NotAlive) == false) //Sensor is healthy.
                            sensor.IsWatched = CheckToReWatchSensor(sensor, startDate);

                        //Remove expired reads
                    }
                }
            }
        CheckupSets = null;
        FeedSets = null;
        PushButtonSets = null;
        #endregion
        #region Observe Binary Sensors.
        var BinarySets = Poultry.Farms.Select(f => f.ElectricPowers);
        if (BinarySets != null)
            foreach (var set in BinarySets)
            {
                if (set.HasSensors)
                {
                    foreach (var sensor in set.EnabledSensors)
                    {
                        if (sensor.IsWatched) await CheckKeepAliveMessageDate(sensor, config.system.KeepAliveWaitingTimeout, Now);
                        if (sensor.HasError)
                        {
                            foreach (var e in sensor.ActiveErrors)
                            {
                                var alarmTimes = GetAlarmTimings(e.ErrorType);
                                if (e.ErrorType == SensorErrorType.InvalidData)
                                {
                                    int BinaryInvalidDataWatchTimeout = 0;
                                    if (sensor.Type == SensorType.FarmElectricPower)
                                    {
                                        BinaryInvalidDataWatchTimeout = config.system.FarmPowerInvalidDataWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.PoultryMainElectricPower)
                                    {
                                        BinaryInvalidDataWatchTimeout = config.system.MainPowerInvalidDataWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.PoultryBackupElectricPower)
                                    {
                                        BinaryInvalidDataWatchTimeout = config.system.BackupPowerInvalidDataWatchTimeout;
                                    }
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, BinaryInvalidDataWatchTimeout);
                                }
                                else if (e.ErrorType == SensorErrorType.NotAlive)
                                {
                                    int BinaryNotAliveWatchTimeout = 0;
                                    if (sensor.Type == SensorType.FarmElectricPower)
                                    {
                                        BinaryNotAliveWatchTimeout = config.system.FarmPowerNotAliveWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.PoultryMainElectricPower)
                                    {
                                        BinaryNotAliveWatchTimeout = config.system.MainPowerNotAliveWatchTimeout;
                                    }
                                    else if (sensor.Type == SensorType.PoultryBackupElectricPower)
                                    {
                                        BinaryNotAliveWatchTimeout = config.system.BackupPowerInvalidDataWatchTimeout;
                                    }
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                    if (sensor.IsWatched) sensor.IsWatched = !CheckToUnWatch(e, sensor.IsWatched, sensor.IsWatched, sensor.IsInPeriod, BinaryNotAliveWatchTimeout);
                                }
                                else if (sensor.IsWatched && e.ErrorType == SensorErrorType.LowBattery)
                                {
                                    if (alarmTimes.Enable && (sensor.IsWatched) && e.DateHappened.IsElapsed(alarmTimes.RaiseTime) && !AlarmableSensorErrors.Contains(e)) AlarmableSensorErrors.Add(e);
                                }
                            }

                            var startDate = Poultry.Farms.Where(f => f.IsInPeriod && f.Checkups.Sensors.Any(s => s.Id == sensor.Id)).FirstOrDefault()?.Period.StartDate;
                            if (startDate == null) startDate = Poultry.Farms.Where(f => f.IsInPeriod && f.Feeds.Sensors.Any(s => s.Id == sensor.Id)).FirstOrDefault()?.Period.StartDate;
                            if (!sensor.IsWatched && sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.NotAlive) == false) sensor.IsWatched = CheckToReWatchSensor(sensor, startDate);//Sensor is healthy.

                            //Remove expired reads
                        }
                    }
                }
            }
        BinarySets = null;
        #endregion

        #region Observe Farms in Period
        //if (Poultry.Farms != null)
        //{
        //    foreach (var f in Poultry.Farms)
        //    {

        //    }
        //}
        #endregion
        ProcessAlarmableErrors(Now);
        //var m = new MqttApplicationMessageBuilder().WithTopic("safa").WithPayload("dana").Build();
        //if (mqttClient.IsConnected) await mqttClient.PublishAsync(m);
        return null;
    }

    private AlarmTimesModel GetAlarmTimings(SensorErrorType e)
    {
        AlarmTimesModel a = new();
        switch (e)
        {
            case SensorErrorType.LowBattery:
                a.Level = config.system.AlarmLevelLowBattery;
                break;
            case SensorErrorType.NotAlive:
                a.Level = config.system.AlarmLevelNotAlive;
                break;
            case SensorErrorType.InvalidData:
            case SensorErrorType.InvalidTemperatureData:
            case SensorErrorType.InvalidHumidityData:
            case SensorErrorType.InvalidLightData:
            case SensorErrorType.InvalidAmmoniaData:
            case SensorErrorType.InvalidCo2Data:
                a.Level = config.system.AlarmLevelInvalidData;
                break;
            case SensorErrorType.InvalidValue:
            case SensorErrorType.InvalidTemperatureValue:
            case SensorErrorType.InvalidHumidityValue:
            case SensorErrorType.InvalidLightValue:
            case SensorErrorType.InvalidAmmoniaValue:
            case SensorErrorType.InvalidCo2Value:
                a.Level = config.system.AlarmLevelInvalidValue;
                break;
        }
        switch (a.Level)
        {
            case 1:
                a.Enable = config.system.AlarmLevelOneEnable;
                a.RaiseTime = config.system.AlarmLevelOneRaiseTime;
                a.FarmAlarmEnable = config.system.AlarmLevelOneFarmAlarmEnable;
                a.FarmAlarmRaiseTimeOffset = config.system.AlarmLevelOneFarmAlarmRaiseTimeOffset;
                a.FarmAlarmEvery = config.system.AlarmLevelOneFarmAlarmEvery;
                a.FarmAlarmSnooze = config.system.AlarmLevelOneFarmAlarmSnooze;
                a.FarmAlarmCountInCycle = config.system.AlarmLevelOneFarmAlarmCountInCycle;

                a.SmsEnable = config.system.AlarmLevelOneSmsEnable;
                a.SmsRaiseTimeOffset = config.system.AlarmLevelOneSmsRaiseTimeOffset;
                a.SmsEvery = config.system.AlarmLevelOneSmsEvery;
                a.SmsSnooze = config.system.AlarmLevelOneSmsSnooze;
                a.SmsCountInCycle = config.system.AlarmLevelOneSmsCountInCycle;

                a.PoultryAlarmEnable = config.system.AlarmLevelOnePoultryAlarmEnable;
                a.PoultryAlarmRaiseTimeOffset = config.system.AlarmLevelOnePoultryAlarmRaiseTimeOffset;
                a.PoultryAlarmEvery = config.system.AlarmLevelOnePoultryAlarmEvery;
                a.PoultryAlarmSnooze = config.system.AlarmLevelOnePoultryAlarmSnooze;
                a.PoultryAlarmCountInCycle = config.system.AlarmLevelOnePoultryAlarmCountInCycle;
                break;
            case 2:
                a.Enable = config.system.AlarmLevelTwoEnable;
                a.RaiseTime = config.system.AlarmLevelTwoRaiseTime;
                a.FarmAlarmEnable = config.system.AlarmLevelTwoFarmAlarmEnable;
                a.FarmAlarmRaiseTimeOffset = config.system.AlarmLevelTwoFarmAlarmRaiseTimeOffset;
                a.FarmAlarmEvery = config.system.AlarmLevelTwoFarmAlarmEvery;
                a.FarmAlarmSnooze = config.system.AlarmLevelTwoFarmAlarmSnooze;
                a.FarmAlarmCountInCycle = config.system.AlarmLevelTwoFarmAlarmCountInCycle;

                a.SmsEnable = config.system.AlarmLevelTwoSmsEnable;
                a.SmsRaiseTimeOffset = config.system.AlarmLevelTwoSmsRaiseTimeOffset;
                a.SmsEvery = config.system.AlarmLevelTwoSmsEvery;
                a.SmsSnooze = config.system.AlarmLevelTwoSmsSnooze;
                a.SmsCountInCycle = config.system.AlarmLevelTwoSmsCountInCycle;

                a.PoultryAlarmEnable = config.system.AlarmLevelTwoPoultryAlarmEnable;
                a.PoultryAlarmRaiseTimeOffset = config.system.AlarmLevelTwoPoultryAlarmRaiseTimeOffset;
                a.PoultryAlarmEvery = config.system.AlarmLevelTwoPoultryAlarmEvery;
                a.PoultryAlarmSnooze = config.system.AlarmLevelTwoPoultryAlarmSnooze;
                a.PoultryAlarmCountInCycle = config.system.AlarmLevelTwoPoultryAlarmCountInCycle;
                break;
            case 3:
                a.Enable = config.system.AlarmLevelThreeEnable;
                a.RaiseTime = config.system.AlarmLevelThreeRaiseTime;
                a.FarmAlarmEnable = config.system.AlarmLevelThreeFarmAlarmEnable;
                a.FarmAlarmRaiseTimeOffset = config.system.AlarmLevelThreeFarmAlarmRaiseTimeOffset;
                a.FarmAlarmEvery = config.system.AlarmLevelThreeFarmAlarmEvery;
                a.FarmAlarmSnooze = config.system.AlarmLevelThreeFarmAlarmSnooze;
                a.FarmAlarmCountInCycle = config.system.AlarmLevelThreeFarmAlarmCountInCycle;

                a.SmsEnable = config.system.AlarmLevelThreeSmsEnable;
                a.SmsRaiseTimeOffset = config.system.AlarmLevelThreeSmsRaiseTimeOffset;
                a.SmsEvery = config.system.AlarmLevelThreeSmsEvery;
                a.SmsSnooze = config.system.AlarmLevelThreeSmsSnooze;
                a.SmsCountInCycle = config.system.AlarmLevelThreeSmsCountInCycle;

                a.PoultryAlarmEnable = config.system.AlarmLevelThreePoultryAlarmEnable;
                a.PoultryAlarmRaiseTimeOffset = config.system.AlarmLevelThreePoultryAlarmRaiseTimeOffset;
                a.PoultryAlarmEvery = config.system.AlarmLevelThreePoultryAlarmEvery;
                a.PoultryAlarmSnooze = config.system.AlarmLevelThreePoultryAlarmSnooze;
                a.PoultryAlarmCountInCycle = config.system.AlarmLevelThreePoultryAlarmCountInCycle;
                break;
            case 4:
                a.Enable = config.system.AlarmLevelFourEnable;
                a.RaiseTime = config.system.AlarmLevelFourRaiseTime;
                a.FarmAlarmEnable = config.system.AlarmLevelFourFarmAlarmEnable;
                a.FarmAlarmRaiseTimeOffset = config.system.AlarmLevelFourFarmAlarmRaiseTimeOffset;
                a.FarmAlarmEvery = config.system.AlarmLevelFourFarmAlarmEvery;
                a.FarmAlarmSnooze = config.system.AlarmLevelFourFarmAlarmSnooze;
                a.FarmAlarmCountInCycle = config.system.AlarmLevelFourFarmAlarmCountInCycle;

                a.SmsEnable = config.system.AlarmLevelFourSmsEnable;
                a.SmsRaiseTimeOffset = config.system.AlarmLevelFourSmsRaiseTimeOffset;
                a.SmsEvery = config.system.AlarmLevelFourSmsEvery;
                a.SmsSnooze = config.system.AlarmLevelFourSmsSnooze;
                a.SmsCountInCycle = config.system.AlarmLevelFourSmsCountInCycle;

                a.PoultryAlarmEnable = config.system.AlarmLevelFourPoultryAlarmEnable;
                a.PoultryAlarmRaiseTimeOffset = config.system.AlarmLevelFourPoultryAlarmRaiseTimeOffset;
                a.PoultryAlarmEvery = config.system.AlarmLevelFourPoultryAlarmEvery;
                a.PoultryAlarmSnooze = config.system.AlarmLevelFourPoultryAlarmSnooze;
                a.PoultryAlarmCountInCycle = config.system.AlarmLevelFourPoultryAlarmCountInCycle;
                break;
        }
        return a;
    }

    private async Task CheckKeepAliveMessageDate(SensorModel sensor, int keepAliveTimeout, DateTime Now)
    {
        if (sensor.IsWatched && config.system.IsKeepAliveEnabled && !sensor.ActiveErrors.Any(e => e.ErrorType == SensorErrorType.NotAlive) && sensor.KeepAliveMessageDate.IsElapsed(keepAliveTimeout))
        {
            Log.Warning($"Sensor with ID: {sensor.Id} is not sending KeepAlive message since {sensor.KeepAliveMessageDate}. Location: {sensor.LocationId}, Section: {sensor.Section}");
            var newErr = GenerateSensorError(sensor.AsBaseModel(), SensorErrorType.NotAlive, Now, $"Not Alive since: {sensor.KeepAliveMessageDate}");
            sensor.Errors.AddError(newErr, SensorErrorType.NotAlive, config.system.MaxSensorErrorCount);
            await DbProcessor.WriteSensorErrorToDbAsync(newErr, Now);
        }
    }

    private bool CheckToReWatchSensor(SensorModel sensor, DateTime? PeriodStartDate)
    {
        if (config.system.ObserveAlways || (sensor.IsInPeriod && PeriodStartDate.IsElapsed(sensor.WatchStartDay * 86400)))
        {
            Log.Information($"Sensor unit is back online. ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}");
            //Inform the watch, save to db
            return true;
        }
        return false;
    }

    private bool CheckToUnWatch(SensorErrorModel e, bool SensorWatch, bool UnitWatch, bool SensorIsInPeriod, int Elapse)
    {
        if (SensorWatch && UnitWatch && e.DateHappened.IsElapsed(Elapse) && (SensorIsInPeriod || config.system.ObserveAlways))
        {
            //inform, save to db
            Log.Warning($"Sensor with ID {e.SensorId} has been unwatched due to persisting error. Error Type: {e.ErrorType}, Error happened date: {e.DateHappened}");
            return true;
        }
        return false;
    }

    private void ProcessAlarmableErrors(DateTime Now)
    {
        foreach (var e in AlarmableSensorErrors.ToList())
        {
            if (e.DateErased.HasValue)
            {
                AlarmableSensorErrors.Remove(e);
                //Disable light/siren alarm if active
                continue;
            }
            var sensor = FindSensorsById(e.SensorId);
            if (sensor == null)
            {
                Log.Error($"No sensor found related to the Error. Sensor ID: {e.SensorId}, Error Type: {e.ErrorType}, Date Happened: {e.DateHappened}");
                continue;
            }
            var alarmTimes = GetAlarmTimings(e.ErrorType);
            if (alarmTimes.Enable)
            {
                if (e.DateAlarmRaised == null) e.DateAlarmRaised = Now;
                if (alarmTimes.FarmAlarmEnable && sensor.IsFarmSensor())
                {
                    if (e.DateFarmAlarmRaised == null && e.DateHappened.IsElapsed(alarmTimes.FarmAlarmRaiseTime)) //first alarm.
                    {
                        e.FarmAlarmRaisedCount = 1;
                        Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.FarmAlarmRaisedCount}");

                        e.DateFarmAlarmRaised = Now;
                    }
                    else if (e.FarmAlarmRaisedCount % alarmTimes.FarmAlarmCountInCycle != 0 && e.DateFarmAlarmRaised.IsElapsed(alarmTimes.FarmAlarmEvery)) //alarm every.
                    {
                        e.FarmAlarmRaisedCount++;
                        Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.FarmAlarmRaisedCount}");
                        //inform, save to db
                        e.DateFarmAlarmRaised = Now;
                    }
                    else if (e.FarmAlarmRaisedCount % alarmTimes.FarmAlarmCountInCycle == 0 && e.DateFarmAlarmRaised.IsElapsed(alarmTimes.FarmAlarmSnooze)) //alarm sleep.
                    {
                        e.FarmAlarmRaisedCount++;
                        Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.FarmAlarmRaisedCount}");
                        //inform, save to db
                        e.DateFarmAlarmRaised = Now;
                    }
                }
                if (alarmTimes.SmsEnable)
                {
                    if (e.DateSmsRaised == null && e.DateHappened.IsElapsed(alarmTimes.SmsRaiseTime)) //first alarm.
                    {
                        e.SmsRaisedCount = 1;
                        Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.SmsRaisedCount}");
                        //inform, save to db
                        e.DateSmsRaised = Now;
                    }
                    else if (e.SmsRaisedCount % alarmTimes.SmsCountInCycle != 0 && e.DateSmsRaised.IsElapsed(alarmTimes.SmsEvery)) //alarm every.
                    {
                        e.SmsRaisedCount++;
                        Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.SmsRaisedCount}");
                        //inform, save to db
                        e.DateSmsRaised = Now;
                    }
                    else if (e.SmsRaisedCount % alarmTimes.SmsCountInCycle == 0 && e.DateSmsRaised.IsElapsed(alarmTimes.SmsSnooze)) //alarm sleep.
                    {
                        e.SmsRaisedCount++;
                        Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.SmsRaisedCount}");
                        //inform, save to db
                        e.DateSmsRaised = Now;
                    }
                }
                if (alarmTimes.PoultryAlarmEnable)
                {
                    if (e.DatePoultryAlarmRaised == null && e.DateHappened.IsElapsed(alarmTimes.PoultryAlarmRaiseTime)) //first alarm.
                    {
                        e.PoultryAlarmRaisedCount = 1;
                        Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.PoultryAlarmRaisedCount}");

                        e.DatePoultryAlarmRaised = Now;
                    }
                    else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultryAlarmCountInCycle != 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultryAlarmEvery)) //alarm every.
                    {
                        e.PoultryAlarmRaisedCount++;
                        Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.PoultryAlarmRaisedCount}");
                        //inform, save to db
                        e.DatePoultryAlarmRaised = Now;
                    }
                    else if (e.PoultryAlarmRaisedCount % alarmTimes.PoultryAlarmCountInCycle == 0 && e.DatePoultryAlarmRaised.IsElapsed(alarmTimes.PoultryAlarmSnooze)) //alarm sleep.
                    {
                        e.PoultryAlarmRaisedCount++;
                        Log.Information($"Informing Alarm of {e.ErrorType}, Sensor ID: {sensor.Id}, Location: {sensor.LocationId}, Section: {sensor.Section}. Count: {e.PoultryAlarmRaisedCount}");
                        //inform, save to db
                        e.DatePoultryAlarmRaised = Now;
                    }
                }
            }
        }
    }

    private async Task RunObserverTimerAsync()
    {
        long ticks;
        while (true)
        {
            if (CanRunObserver)
            {
                ticks = DateTime.Now.Ticks;
                if (config.VerboseMode) Log.Information($"===============  Start observation process ===============");
                try
                {
                    if (Poultry != null && (Poultry.IsInPeriod || config.system.ObserveAlways))
                    {
                        var result = await ObservePoultryAsync();
                        if (!string.IsNullOrEmpty(result))
                            Log.Error($"Observation process returned with error: {result}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception thrown in Observer:");
                }
                if (config.VerboseMode) Log.Information($"===============  End of observation process ({TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds} ms) ===============");
                await Task.Delay(TimeSpan.FromSeconds(config.system.ObserverCheckInterval));
            }
        }
    }
}