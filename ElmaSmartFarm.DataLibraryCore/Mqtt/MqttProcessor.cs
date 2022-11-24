using ElmaSmartFarm.DataLibraryCore.Interfaces;
using ElmaSmartFarm.SharedLibrary.Config;
using ElmaSmartFarm.SharedLibrary.Models;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.Mqtt
{
    public class MqttProcessor : IMqttProcessor
    {
        public MqttProcessor(IDbProcessor dbProcessor)
        {
            DbProcessor = dbProcessor;
            sensor_topic = Config.MQTT.SensorTopic;
            temperature_sub_topic = Config.MQTT.TemperatureSubTopic;
        }

        private readonly IDbProcessor DbProcessor;
        private readonly string sensor_topic;
        private readonly string temperature_sub_topic;

        public async Task<int> ProcessMqttMessageAsync(MqttMessageModel mqtt)
        {
            if (mqtt == null) return -1;
            if (mqtt.Topic.StartsWith(sensor_topic + temperature_sub_topic))
            {
                var sensorId = mqtt.Topic.Replace(sensor_topic + temperature_sub_topic + "/", "");
                if (string.IsNullOrEmpty(sensorId)) return -1;
                if (!int.TryParse(sensorId, out int SensorId)) return -1;
                if (!double.TryParse(mqtt.Payload, out double Payload)) return -1;

                TemperatureModel temp = new()
                {
                    SensorId = SensorId,
                    Celsius = Payload,
                    ReadDate = mqtt.ReadDate
                };
                return await DbProcessor.SaveTemperatureToDb(temp);
            }
            return 0;
        }
    }
}