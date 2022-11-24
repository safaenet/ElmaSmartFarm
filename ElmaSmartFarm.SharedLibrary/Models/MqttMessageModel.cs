using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class MqttMessageModel
    {
        public string Topic { get; set; }
        public string Payload { get; set; }
        public DateTime ReadDate { get; set; }
        public bool Retained { get; set; }
        public int QoS { get; set; }
    }
}