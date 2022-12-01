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
        Task<int> SaveSensorValueToDbAsync(TemperatureSensorModel sensor, double value, DateTime now);
        Task<List<PoultryModel>> LoadPoultriesAsync();
        Task<int> WriteSensorErrorToDbAsync(SensorErrorModel error, DateTime now);
        Task<bool> EraseSensorErrorFromDbAsync(SensorBaseModel sensor, SensorErrorType type, DateTime eraseDate);
    }
}