using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DALModels
{
    public class TemperatureModel
    {
        public int SensorId { get; set; }
        public DateTime ReadDate { get; set; }
        public double Celsius { get; set; }
        public double Kelvin => Celsius + 273.15;
        public double Fahrenheit => Celsius * 9 / 5 + 32;
    }
}