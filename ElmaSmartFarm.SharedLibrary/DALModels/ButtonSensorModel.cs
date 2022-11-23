using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class ButtonSensorModel
    {
        public int Id { get; set; }
        public List<DateTime> ButtonReads { get; set; }
        public DateTime? LastReadDate => ButtonReads?.Max();
        public bool IsEnabled { get; set; }
        public int BatteryLevel { get; set; }
        public string Descriptions { get; set; }
    }
}