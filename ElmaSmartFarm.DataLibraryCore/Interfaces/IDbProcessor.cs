using ElmaSmartFarm.SharedLibrary.DALModels;
using SharedLibrary.DALModels;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.Interfaces
{
    public interface IDbProcessor
    {
        Task<int> SaveTemperatureToDb(TemperatureModel temp);
    }
}