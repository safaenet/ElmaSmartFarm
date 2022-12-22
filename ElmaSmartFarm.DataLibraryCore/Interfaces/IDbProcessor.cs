using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Alarm;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.Interfaces;

public interface IDbProcessor
{
    Task<int> WriteScalarSensorValueToDbAsync(SensorModel sensor, ScalarSensorReadModel value);
    Task<int> WriteSensorValueToDbAsync(SensorModel sensor, double value, DateTime now, double offset = 0);
    Task<PoultryModel> LoadPoultryAsync();
    Task<int> WriteSensorErrorToDbAsync(SensorErrorModel error, DateTime now);
    Task<int> WriteFarmErrorToDbAsync(FarmInPeriodErrorModel error, DateTime now);
    Task<bool> EraseSensorErrorFromDbAsync(int sensorId, DateTime eraseDate, params SensorErrorType[] types);
    Task<bool> EraseFarmErrorFromDbAsync(int farmId, DateTime eraseDate, params FarmInPeriodErrorType[] types);
    Task<bool> ErasePoultryErrorFromDbAsync(DateTime eraseDate, params PoultryInPeriodErrorType[] types);
    Task<int> WritePoultryErrorToDbAsync(PoultryInPeriodErrorModel error, DateTime now);
    Task<int> WriteSensorWatchLogToDbAsync(int sensorId, int locationId, SensorSection section, SensorWatchAction action, DateTime now, string descriptions = "");
    Task<int> WriteAlarmTriggerLogToDbAsync(int alarmId, int locationId, AlarmTriggerType action, DateTime now);
}