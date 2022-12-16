using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.Interfaces;

public interface IDbProcessor
{
    Task<int> WriteScalarSensorValueToDbAsync(SensorModel sensor, ScalarSensorReadModel value);
    Task<int> WriteSensorValueToDbAsync(SensorModel sensor, double value, DateTime now, double offset = 0);
    Task<PoultryModel> LoadPoultriesAsync();
    Task<int> WriteSensorErrorToDbAsync(SensorErrorModel error, DateTime now);
    Task<int> WriteFarmErrorToDbAsync(FarmInPeriodErrorModel error, DateTime now);
    //Task<bool> EraseSensorErrorFromDbAsync(SensorBaseModel sensor, SensorErrorType type, DateTime eraseDate);
    //Task<bool> EraseSensorErrorFromDbAsync(int sensorId, SensorErrorType type, DateTime eraseDate);
    Task<bool> EraseSensorErrorFromDbAsync(int sensorId, DateTime eraseDate, params SensorErrorType[] types);
    Task<bool> EraseFarmErrorFromDbAsync(int farmId, DateTime eraseDate, params FarmInPeriodErrorType[] types);
    Task<bool> ErasePoultryErrorFromDbAsync(DateTime eraseDate, params PoultryInPeriodErrorType[] types);
    Task<int> WritePoultryErrorToDbAsync(PoultryInPeriodErrorModel error, DateTime now);
}