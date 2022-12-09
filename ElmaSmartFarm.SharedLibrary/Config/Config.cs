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
            ScalarSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:scalar_sub_topic").Value ?? "Scalar/";
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
        public string ScalarSubTopic { get; init; }
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
            SensorLowBatteryLevel = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("sensor_low_battery_level").Value ?? "10");

            ScalarReadInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:read_interval").Value ?? "30");
            WriteScalarToDbInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:write_to_db_interval").Value ?? "30");
            WriteScalarToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:write_to_db_always").Value ?? "false");

            FarmTempMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:farm_min_value").Value ?? "20");
            FarmTempMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:farm_max_value").Value ?? "35");
            OutdoorTempMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:outdoor_min_value").Value ?? "-10");
            OutdoorTempMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:outdoor_max_value").Value ?? "60");

            HumidMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:min_value").Value ?? "20");
            HumidMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:max_value").Value ?? "35");

            AmbientLightMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:min_value").Value ?? "20");
            AmbientLightMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:max_value").Value ?? "35");

            AmmoniaMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:min_value").Value ?? "0");
            AmmoniaMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:max_value").Value ?? "100");

            Co2MinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:min_value").Value ?? "0");
            Co2MaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:max_value").Value ?? "100");

            WriteCommuteToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:write_to_db_always").Value ?? "false");

            WritePushButtonToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("push_button:write_to_db_always").Value ?? "false");

            BinaryReadInterval = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("binary:read_interval").Value ?? "30");
            WriteBinaryOnValueChange = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("binary:write_on_value_change").Value ?? "true");
            WriteBinaryToDbInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("binary:write_to_db_interval").Value ?? "30");
            WriteBinaryToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("binary:write_to_db_always").Value ?? "false");

            ObserverCheckInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("observer:observer_check_interval").Value ?? "5");
            ObserveAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("observer:observe_always").Value ?? "false");
        }

        public int KeepAliveInterval { get; set; }
        public int MaxSensorErrorCount { get; set; }
        public int MaxSensorReadCount { get; set; }
        public int SensorLowBatteryLevel { get; set; }

        #region Scalar General Settings
        public double ScalarReadInterval { get; set; }
        public int WriteScalarToDbInterval { get; set; }
        public bool WriteScalarToDbAlways { get; set; }
        #endregion

        #region Temperature Settings
        public double FarmTempMinValue { get; set; }
        public double FarmTempMaxValue { get; set; }
        public double OutdoorTempMinValue { get; set; }
        public double OutdoorTempMaxValue { get; set; }
        #endregion

        #region Humidity Settings
        public double HumidMinValue { get; set; }
        public double HumidMaxValue { get; set; }
        #endregion

        #region Ambient Light Settings
        public double AmbientLightMinValue { get; set; }
        public double AmbientLightMaxValue { get; set; }
        #endregion

        #region Ammonia Settings
        public double AmmoniaMinValue { get; set; }
        public double AmmoniaMaxValue { get; set; }
        #endregion

        #region Co2 Settings
        public double Co2MinValue { get; set; }
        public double Co2MaxValue { get; set; }
        #endregion

        #region Commute Settings
        public bool WriteCommuteToDbAlways { get; set; }
        #endregion

        #region Push Button Settings
        public bool WritePushButtonToDbAlways { get; set; }
        #endregion

        #region Binary Settings
        public double BinaryReadInterval { get; set; }
        public bool WriteBinaryOnValueChange { get; set; }
        public int WriteBinaryToDbInterval { get; set; }
        public bool WriteBinaryToDbAlways { get; set; }
        #endregion

        #region Observer Settings
        public int ObserverCheckInterval { get; set; }
        public bool ObserveAlways { get; set; }
        #endregion
    }
}