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
        public DateTime? LastReadDate { get; set; }
        public bool IsEnabled { get; set; }
        public string Descriptions { get; set; }
    }
}