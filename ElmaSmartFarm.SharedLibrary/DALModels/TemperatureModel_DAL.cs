using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DALModels
{
    public class TemperatureModel_DAL
    {
        public int DeviceId { get; set; }
        public double Celsius { get; set; }
        public double Kelvin
        {
            get => Celsius + 273.15;
            set => Celsius = value - 273.15;
        }
        public double Fahrenheit
        {
            get => Celsius * 9 / 5 + 32;
            set => Celsius = (value - 32) * 5 / 9;
        }
    }
}