namespace ElmaSmartFarm.SharedLibrary.Config
{
    public class Config
    {
        public Config()
        {
            BaseUrl = SettingsDataAccess.AppConfiguration().GetSection("BaseUrl").Value;
            MaxSensorErrorCount = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("max_sensor_error_count").Value ?? "10");
            MaxSensorReadCount = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("max_sensor_read_count").Value ?? "10");
            mqtt = new();
            system = new();
        }
        public MQTT mqtt { get; init; }
        public System system { get; set; }
        public string BaseUrl { get; init; }
        public int MaxSensorErrorCount { get; set; }
        public int MaxSensorReadCount { get; set; }
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
            TemperatureSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:temperature_sub_topic").Value ?? "Temp/";
            HumiditySubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:humidity_sub_topic").Value ?? "Humid/";
            //FarmTemperatureSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:farm_temperature_sub_topic").Value ?? "FarmTemp/";
            //FarmHumiditySubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:farm_humidity_sub_topic").Value ?? "FarmHumid/";
            //OutdoorTemperatureSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:outdoor_temperature_sub_topic").Value ?? "OutTemp/";
            //OutdoorHumiditySubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:outdoor_humidity_sub_topic").Value ?? "OutHumid/";
            AmbientLightSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:ambient_light_sub_topic").Value ?? "Ambient/";
            PushButtonSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:push_button_sub_topic").Value ?? "PushButton/";
            //FeedSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:feed_sub_topic").Value ?? "Feed/";
            //CheckupSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:checkup_sub_topic").Value ?? "Checkup/";
            CommuteSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:commute_sub_topic").Value ?? "Commute/";
            BinarySubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:binary_sub_topic").Value ?? "Binary/";
            //FarmElectricSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:farm_electric_sub_topic").Value ?? "FarmElec/";
            //MainElectricSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:main_electric_sub_topic").Value ?? "MainElec/";
            //BackupElectricSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:backup_electric_sub_topic").Value ?? "BackElec/";
            KeepAliveSubTopic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:keep_alive_sub_topic").Value ?? "Alive/"; //e.g:Elma/ToServer/Sensors/Temp/Alive/{Id}
        }
        public string Broker { get; init; }
        public int Port { get; init; }
        public bool AuthenticationEnabled { get; init; }
        public string Username { get; init; }
        public string Password { get; init; }
        public int RetryInterval { get; init; }
        public string ToServerTopic { get; init; }
        public string ToSensorTopic { get; init; }
        public string TemperatureSubTopic { get; init; }
        public string HumiditySubTopic { get; init; }
        //public string FarmTemperatureSubTopic { get; init; }
        //public string FarmHumiditySubTopic { get; init; }
        //public string OutdoorTemperatureSubTopic { get; init; }
        //public string OutdoorHumiditySubTopic { get; init; }
        public string AmbientLightSubTopic { get; init; }
        public string PushButtonSubTopic { get; init; }
        //public string FeedSubTopic { get; init; }
        //public string CheckupSubTopic { get; init; }
        public string CommuteSubTopic { get; init; }
        public string BinarySubTopic { get; init; }
        //public string FarmElectricSubTopic { get; init; }
        //public string MainElectricSubTopic { get; init; }
        //public string BackupElectricSubTopic { get; init; }
        public string KeepAliveSubTopic { get; init; }
        public string FullTemperatureTopic => ToServerTopic + TemperatureSubTopic;
        public string FullHumidityTopic => ToServerTopic + HumiditySubTopic;
        public string FullAmbientLightTopic => ToServerTopic + AmbientLightSubTopic;
        //public string FullFeedTopic => ToServerTopic + FeedSubTopic;
        //public string FullCheckupTopic => ToServerTopic + CheckupSubTopic;
        public string FullPushButtonTopic => ToServerTopic + PushButtonSubTopic;
        public string FullCommuteTopic => ToServerTopic + CommuteSubTopic;
        public string FullBinaryTopic => ToServerTopic + BinarySubTopic;
        //public string FullFarmElectricTopic => ToServerTopic + FarmElectricSubTopic;
        //public string FullMainElectricTopic => ToServerTopic + MainElectricSubTopic;
        //public string FullBackupElectricTopic => ToServerTopic + BackupElectricSubTopic;
    }

    public class System
    {
        public double TempReadInterval { get; set; }
        public double FarmTempMinValue { get; set; }
        public double FarmTempMaxValue { get; set; }
        public double OutdoorTempMinValue { get; set; }
        public double OutdoorTempMaxValue { get; set; }
        public double TempMaxDifferValue { get; set; }
        public int WriteTempToDbInterval { get; set; }

        public double HumidReadInterval { get; set; }
        public double HumidMinValue { get; set; }
        public double HumidMaxValue { get; set; }
        public double HumidMaxDifferValue { get; set; }
        public int WriteHumidToDbInterval { get; set; }

        public double AmbientLightReadInterval { get; set; }
        public double AmbientLightMinValue { get; set; }
        public double AmbientLightMaxValue { get; set; }
        public int WriteAmbientLightToDbInterval { get; set; }
    }
}