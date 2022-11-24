using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class BinarySensorModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BinarySensorType Type { get; set; }
        public List<BinarySensorReadModel> BinaryReads { get; set; }
        public bool? CurrentStatus => BinaryReads?.MaxBy(t => t.ReadDate)?.Status;
        public DateTime? LastReadDate => BinaryReads?.MaxBy(t => t.ReadDate)?.ReadDate;
        public bool IsEnabled { get; set; }
        public int BatteryLevel { get; set; }
        public string Descriptions { get; set; }
    }
}