using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class FarmModel
    {
        public int Id { get; set; }
        public List<TemperatureSensorModel> TemperatureSensors { get; set; }
        public List<HumiditySensorModel> HumiditySensors { get; set; }
        public List<AmbientLightSensorModel> AmbientLightSensors { get; set; }
        public ButtonSensorModel ButtonSensor { get; set; }
        public ButtonSensorModel FeedSensor { get; set; }
        public DateTime? LastCheckupDate => ButtonSensor?.LastReadDate;
        public DateTime? LastFeedDate => FeedSensor?.LastReadDate;
        public bool IsEnabled { get; set; }
        public string Descriptions { get; set; }
    }
}