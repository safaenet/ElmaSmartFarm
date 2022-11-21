using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class TemperatureSensorModel
    {
        public int Id { get; set; }
        public double Temperature { get; set; } //Celsius
        public double Offset { get; set; }
        public DateTime? LastReadDate { get; set; }
        public bool IsEnabled { get; set; }
        public string Descriptions { get; set; }
    }
}