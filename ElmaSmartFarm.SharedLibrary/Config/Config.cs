namespace ElmaSmartFarm.SharedLibrary.Config
{
    public class Config
    {
        public Config()
        {
            DefaultConnectionString = SettingsDataAccess.AppConfiguration().GetSection("ConnectionStrings:default").Value;
            BaseUrl = SettingsDataAccess.AppConfiguration().GetSection("BaseUrl").Value;
            VerboseMode = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("verbose_mode").Value ?? "true");
            mqtt = new();
            system = new();
        }
        public string DefaultConnectionString { get; set; }
        public MQTT mqtt { get; init; }
        public System system { get; set; }
        public string BaseUrl { get; init; }
        public bool VerboseMode { get; set; }
    }

    public class MQTT
    {
        public MQTT()
        {
            Broker = SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_broker").Value ?? "192.168.1.106";
            Port = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_port").Value ?? "1883");
            AuthenticationEnabled = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("mqtt:authentication_enabled").Value ?? "false");
            Username = SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_username").Value ?? "admin";
            Password = SettingsDataAccess.AppConfiguration().GetSection("mqtt:mqtt_password").Value ?? "admin";
            RetryInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("mqtt:retry_seconds").Value ?? "2");
            ToServerTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:to_server_topic").Value ?? "Elma/ToServer/"; //e.g:Elma/ToServer/Sensors/Temp/{Id} Payload={Value}
            ToSensorTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:to_sensor_topic").Value ?? "Elma/ToSensor/"; //e.g:Elma/ToSensor/{Id}/Interval Payload={Interval}
            FromSensorSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:from_sensor_sub_topic").Value ?? "FromSensor/";
            TemperatureSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:temperature_sub_topic").Value ?? "Temp/";
            HumiditySubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:humidity_sub_topic").Value ?? "Humid/";
            AmbientLightSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:ambient_light_sub_topic").Value ?? "Ambient/";
            PushButtonSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:push_button_sub_topic").Value ?? "PushButton/";
            CommuteSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:commute_sub_topic").Value ?? "Commute/";
            BinarySubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:binary_sub_topic").Value ?? "Binary/";
            KeepAliveSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:keep_alive_sub_topic").Value ?? "Alive/"; //e.g:Elma/ToServer/Sensors/Temp/Alive/{Id}
            IPAddressSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:ip_addsess_sub_topic").Value ?? "IP/";
            BatteryLevelSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:battery_level_sub_topic").Value ?? "Battery/";
            MaxUnknownMqttCount = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("mqtt:max_unknown_mqtt_count").Value ?? "100");
        }
        public string Broker { get; init; }
        public int Port { get; init; }
        public bool AuthenticationEnabled { get; init; }
        public string Username { get; init; }
        public string Password { get; init; }
        public int RetryInterval { get; init; }
        public string ToServerTopic { get; init; }
        public string ToSensorTopic { get; init; }
        public string FromSensorSubTopic { get; init; }
        public string TemperatureSubTopic { get; init; }
        public string HumiditySubTopic { get; init; }
        public string AmbientLightSubTopic { get; init; }
        public string PushButtonSubTopic { get; init; }
        public string CommuteSubTopic { get; init; }
        public string BinarySubTopic { get; init; }
        public string KeepAliveSubTopic { get; init; }
        public string IPAddressSubTopic { get; init; }
        public string BatteryLevelSubTopic { get; init; }
        public int MaxUnknownMqttCount { get; init; }
    }

    public class System
    {
        public System()
        {
            MaxSensorErrorCount = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("max_sensor_error_count").Value ?? "10");
            KeepAliveInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("keep_alive_interval").Value ?? "15");
            MaxSensorReadCount = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("max_sensor_read_count").Value ?? "10");
            SensorLowBatteryLevel = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("sensor_low_battery_level").Value ?? "15");
            TempReadInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:read_interval").Value ?? "30");
            FarmTempMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:farm_min_value").Value ?? "20");
            FarmTempMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:farm_max_value").Value ?? "35");
            OutdoorTempMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:outdoor_min_value").Value ?? "-10");
            OutdoorTempMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:outdoor_max_value").Value ?? "60");
            TempMaxDifferValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:max_differ_value").Value ?? "1");
            WriteTempToDbInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:write_to_db_max_interval").Value ?? "30");
            WriteOnValueChangeByDiffer = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:write_on_value_change_by_differ").Value ?? "true");
        }
        public int KeepAliveInterval { get; set; }
        public int MaxSensorErrorCount { get; set; }
        public int MaxSensorReadCount { get; set; }
        public int SensorLowBatteryLevel { get; set; }

        public double TempReadInterval { get; set; }
        public double FarmTempMinValue { get; set; }
        public double FarmTempMaxValue { get; set; }
        public double OutdoorTempMinValue { get; set; }
        public double OutdoorTempMaxValue { get; set; }
        public double TempMaxDifferValue { get; set; }
        public bool WriteOnValueChangeByDiffer { get; set; }
        public int WriteTempToDbInterval { get; set; }

        public double HumidReadInterval { get; set; }
        public double HumidMinValue { get; set; }
        public double HumidMaxValue { get; set; }
        public double HumidMaxDifferValue { get; set; }
        public int WriteHumidToDbInterval { get; set; }

        public double AmbientLightReadInterval { get; set; }
        public double AmbientLightMinValue { get; set; }
        public double AmbientLightMaxValue { get; set; }
        public double AmbientLightMaxDifferValue { get; set; }
        public int WriteAmbientLightToDbInterval { get; set; }
    }
}