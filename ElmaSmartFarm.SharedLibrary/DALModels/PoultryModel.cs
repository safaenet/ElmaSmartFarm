using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class PoultryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<FarmModel> Farms { get; set; }
        public TemperatureSensorModel OutdoorTemperature { get; set; }
        public HumiditySensorModel OutdoorHumidity { get; set; }
        public BinarySensorModel MainPowerStatus { get; set; }
        public BinarySensorModel BackupPowerStatus { get; set; }
        public bool HasAlarm => Farms != null && Farms.Any(f => f.HasAlarm);
        public bool IsEnabled { get; set; }
    }
}