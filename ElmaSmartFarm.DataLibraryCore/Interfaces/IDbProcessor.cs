using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.Interfaces
{
    public interface IDbProcessor
    {
        Task<int> SaveSensorValueToDbAsync(TemperatureSensorModel sensor, double value);
        Task<List<PoultryModel>> LoadPoultries();
    }
}