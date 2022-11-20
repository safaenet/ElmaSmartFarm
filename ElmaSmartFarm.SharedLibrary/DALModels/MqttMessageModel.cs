using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class MqttMessageModel
    {
        public string ClientId { get; set; }
        public string Topic { get; set; }
        public string Payload { get; set; }
        public DateTime ReadDate { get; set; }
    }
}