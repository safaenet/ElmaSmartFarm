using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.SharedLibrary
{
    public enum SensorType
    {
        Temperature = 1,
        Humidity = 2,
        Light = 3,
        Feed = 4,
        PushButton = 5,
        ElectricPower = 6
    }

    public enum EnvironmentType
    {
        Farm = 1,
        Outside = 2,
        Warehouse = 3
    }
}