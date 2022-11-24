using ElmaSmartFarm.SharedLibrary.Models;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.Interfaces
{
    public interface IMqttProcessor
    {
        Task<int> ProcessMqttMessageAsync(MqttMessageModel mqtt);
    }
}