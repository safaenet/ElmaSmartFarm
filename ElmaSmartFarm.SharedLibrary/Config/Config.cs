using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.SharedLibrary.Config
{
    public static class Config
    {
        public static string BaseUrl => SettingsDataAccess.AppConfiguration().GetSection("BaseUrl").Value ?? "http://localhost:44342/api/v1/";

        public static class MQTT
        {
            public static string Broker => SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_broker").Value ?? "192.168.1.106";
            public static int Port => int.Parse(SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_port").Value ?? "1883");
            public static bool AuthenticationEnabled => bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("mqtt:authentication_enabled").Value ?? "false");
            public static string Username => SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_username").Value ?? "admin";
            public static string Password => SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_password").Value ?? "admin";
            public static int RetryInterval => int.Parse(SettingsDataAccess.AppConfiguration().GetSection("mqtt:retry_seconds").Value ?? "2");
            public static int WriteToDbInterval => int.Parse(SettingsDataAccess.AppConfiguration().GetSection("mqtt:write_to_db_interval").Value ?? "30");
            public static string SensorTopic => SettingsDataAccess.AppConfiguration().GetSection("mqtt:sensor_topic").Value ?? "Elma/ToServer/Sensors";
            public static string TemperatureSubTopic => SettingsDataAccess.AppConfiguration().GetSection("mqtt:temperature_sub_topic").Value ?? "/Temp";
            public static string HumiditySubTopic => SettingsDataAccess.AppConfiguration().GetSection("mqtt:humidity_sub_topic").Value ?? "/Humid";
            public static string FullTemperatureTopic => SensorTopic + TemperatureSubTopic;
            public static string FullHumidityTopic => SensorTopic + HumiditySubTopic;
        }
    }
}