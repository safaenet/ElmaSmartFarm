using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.Interfaces
{
    public interface IDbProcessor
    {
        Task<int> WriteSensorValueToDbAsync(SensorModel sensor, double value, DateTime now, double offset = 0);
        Task<List<PoultryModel>> LoadPoultriesAsync();
        Task<int> WriteSensorErrorToDbAsync(SensorErrorModel error, DateTime now);
        //Task<bool> EraseSensorErrorFromDbAsync(SensorBaseModel sensor, SensorErrorType type, DateTime eraseDate);
        //Task<bool> EraseSensorErrorFromDbAsync(int sensorId, SensorErrorType type, DateTime eraseDate);
        Task<bool> EraseSensorErrorFromDbAsync(int sensorId, SensorErrorType[] types, DateTime eraseDate);
    }
}