using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class AmbientLightSensorModel
    {
        public int Id { get; set; }
        public int Light { get; set; } //Percent
        public DateTime? LastReadDate { get; set; }
        public bool IsEnabled { get; set; }
        public string Descriptions { get; set; }
    }
}