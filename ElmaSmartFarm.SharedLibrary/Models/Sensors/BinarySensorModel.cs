using System.Collections.Generic;
using System.Linq;

namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class BinarySensorModel : SensorModel
    {
        public List<BinarySensorReadModel> Values { get; set; }
        public BinarySensorReadModel LastRead => Values?.MaxBy(t => t.ReadDate);
        public BinarySensorReadModel LastSavedRead => Values?.Where(t => t.IsSavedToDb).MaxBy(t => t.ReadDate);
    }
}