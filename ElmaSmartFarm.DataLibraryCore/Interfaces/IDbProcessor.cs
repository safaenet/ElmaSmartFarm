using ElmaSmartFarm.SharedLibrary.Models;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.Interfaces
{
    public interface IDbProcessor
    {
        Task<int> SaveTemperatureToDb(TemperatureModel temp);
    }
}