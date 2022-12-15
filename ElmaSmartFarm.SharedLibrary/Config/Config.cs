namespace ElmaSmartFarm.SharedLibrary.Config;

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
        KeepAliveInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("keep_alive_interval").Value ?? "15");
        KeepAliveWaitingTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("keep_alive_waiting_timeout").Value ?? "45");
        MaxSensorErrorCount = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("max_sensor_error_count").Value ?? "10");
        MaxSensorReadCount = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("max_sensor_read_count").Value ?? "10");
        SensorLowBatteryLevel = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("sensor_low_battery_level").Value ?? "10");

        ScalarReadInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("scalar_general:read_interval").Value ?? "30");
        WriteScalarToDbInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("scalar_general:write_to_db_interval").Value ?? "30");
        WriteScalarToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("scalar_general:write_to_db_always").Value ?? "false");
        ScalarNotAliveWatchTimeout= int.Parse(SettingsDataAccess.AppConfiguration().GetSection("scalar_general:not_alive_watch_timeout").Value ?? "300");

        FarmTempMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:farm_min_value").Value ?? "20");
        FarmTempMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:farm_max_value").Value ?? "35");
        OutdoorTempMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:outdoor_min_value").Value ?? "-10");
        OutdoorTempMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:outdoor_max_value").Value ?? "60");
        TempInvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:invalid_data_watch_timeout").Value ?? "300");
        TempInvalidValueWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:invalid_value_watch_timeout").Value ?? "300");


        HumidMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:min_value").Value ?? "20");
        HumidMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:max_value").Value ?? "35");
        HumidInvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:invalid_data_watch_timeout").Value ?? "300");
        HumidInvalidValueWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:invalid_value_watch_timeout").Value ?? "300");

        AmbientLightMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:min_value").Value ?? "20");
        AmbientLightMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:max_value").Value ?? "35");
        AmbientLightInvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:invalid_data_watch_timeout").Value ?? "300");
        AmbientLightInvalidValueWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:invalid_value_watch_timeout").Value ?? "300");

        AmmoniaMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:min_value").Value ?? "0");
        AmmoniaMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:max_value").Value ?? "100");
        AmmoniaInvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:invalid_data_watch_timeout").Value ?? "300");
        AmmoniaInvalidValueWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:invalid_value_watch_timeout").Value ?? "300");

        Co2MinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:min_value").Value ?? "0");
        Co2MaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:max_value").Value ?? "100");
        Co2InvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:invalid_data_watch_timeout").Value ?? "300");
        Co2InvalidValueWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:invalid_value_watch_timeout").Value ?? "300");

        WriteCommuteToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:write_to_db_always").Value ?? "false");
        CommuteNotAliveWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:not_alive_watch_timeout").Value ?? "300");
        CommuteInvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:invalid_data_watch_timeout").Value ?? "300");
        CommuteInvalidValueWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:invalid_value_watch_timeout").Value ?? "300");

        WriteFeedToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:write_to_db_always").Value ?? "false");
        FeedNotAliveWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:not_alive_watch_timeout").Value ?? "300");

        WriteCheckupToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:write_to_db_always").Value ?? "false");
        CheckupNotAliveWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:not_alive_watch_timeout").Value ?? "300");

        FarmPowerReadInterval=int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:read_interval").Value ?? "60");
        WriteFarmPowerOnValueChange= bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:write_on_value_change").Value ?? "true");
        WriteFarmPowerToDbInterval=int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:write_to_db_interval").Value ?? "60");
        WriteFarmPowerToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:write_to_db_always").Value ?? "false");
        FarmPowerNotAliveWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:not_alive_watch_timeout").Value ?? "300");
        FarmPowerInvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:invalid_data_watch_timeout").Value ?? "300");

        MainPowerReadInterval=int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:read_interval").Value ?? "60");
        WriteMainPowerOnValueChange= bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:write_on_value_change").Value ?? "true");
        WriteMainPowerToDbInterval=int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:write_to_db_interval").Value ?? "60");
        WriteMainPowerToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:write_to_db_always").Value ?? "false");
        MainPowerNotAliveWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:not_alive_watch_timeout").Value ?? "300");
        MainPowerInvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:invalid_data_watch_timeout").Value ?? "300");

        BackupPowerReadInterval=int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:read_interval").Value ?? "60");
        WriteBackupPowerOnValueChange= bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:write_on_value_change").Value ?? "true");
        WriteBackupPowerToDbInterval=int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:write_to_db_interval").Value ?? "60");
        WriteBackupPowerToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:write_to_db_always").Value ?? "false");
        BackupPowerNotAliveWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:not_alive_watch_timeout").Value ?? "300");
        BackupPowerInvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:invalid_data_watch_timeout").Value ?? "300");

        ObserverCheckInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("observer:observer_check_interval").Value ?? "5");
        ObserveAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("observer:observe_always").Value ?? "false");
    }

    public int KeepAliveInterval { get; set; }
    public bool IsKeepAliveEnabled => KeepAliveInterval > 0;
    public int KeepAliveWaitingTimeout { get; set; }
    public int MaxSensorErrorCount { get; set; }
    public int MaxSensorReadCount { get; set; }
    public int SensorLowBatteryLevel { get; set; }

    public int AlarmLevelLowBattery { get; set; }
    public int AlarmLevelNotAlive { get; set; }
    public int AlarmLevelInvalidData { get; set; }
    public int AlarmLevelInvalidValue { get; set; }
    public int AlarmLevelHighTemperature { get; set; }
    public int AlarmLevelLowTemperature { get; set; }
    public int AlarmLevelHighHumid { get; set; }
    public int AlarmLevelLowHumid { get; set; }
    public int AlarmLevelHighAmmonia { get; set; }
    public int AlarmLevelHighCo2 { get; set; }
    public int AlarmLevelLongTimeBright { get; set; }
    public int AlarmLevelLongTimeDark { get; set; }
    public int AlarmLevelHighBrightness { get; set; }
    public int AlarmLevelLowBrightness { get; set; }
    public int AlarmLevelLongNoFeed { get; set; }
    public int AlarmLevelLongLeave { get; set; }
    public int AlarmLevelNoPower { get; set; }

    public int FarmAlarmDuration { get; set; }
    public int PoultryAlarmDuration { get; set; }
    public bool AlarmLevelOneEnable { get; set; }
    public int AlarmLevelOneFirstTime { get; set; }
    public int AlarmLevelOneEvery { get; set; }
    public int AlarmLevelOneSnooze { get; set; }
    public int AlarmLevelOneCountInCycle { get; set; }
    public bool AlarmLevelTwoEnable { get; set; }
    public int AlarmLevelTwoFirstTime { get; set; }
    public int AlarmLevelTwoEvery { get; set; }
    public int AlarmLevelTwoSnooze { get; set; }
    public int AlarmLevelTwoCountInCycle { get; set; }
    public bool AlarmLevelThreeEnable { get; set; }
    public int AlarmLevelThreeFirstTime { get; set; }
    public int AlarmLevelThreeEvery { get; set; }
    public int AlarmLevelThreeSnooze { get; set; }
    public int AlarmLevelThreeCountInCycle { get; set; }
    public bool AlarmLevelFourEnable { get; set; }
    public int AlarmLevelFourFirstTime { get; set; }
    public int AlarmLevelFourEvery { get; set; }
    public int AlarmLevelFourSnooze { get; set; }
    public int AlarmLevelFourCountInCycle { get; set; }

    #region Scalar General Settings
    public double ScalarReadInterval { get; set; }
    public int WriteScalarToDbInterval { get; set; }
    public bool WriteScalarToDbAlways { get; set; }
    public int ScalarNotAliveWatchTimeout { get; set; }
    #endregion

    #region Temperature Settings
    public double FarmTempMinValue { get; set; }
    public double FarmTempMaxValue { get; set; }
    public double OutdoorTempMinValue { get; set; }
    public double OutdoorTempMaxValue { get; set; }
    public int TempInvalidDataWatchTimeout { get; set; }
    public int TempInvalidValueWatchTimeout { get; set; }
    #endregion

    #region Humidity Settings
    public double HumidMinValue { get; set; }
    public double HumidMaxValue { get; set; }
    public int HumidInvalidDataWatchTimeout { get; set; }
    public int HumidInvalidValueWatchTimeout { get; set; }
    #endregion

    #region Ambient Light Settings
    public double AmbientLightMinValue { get; set; }
    public double AmbientLightMaxValue { get; set; }
    public int AmbientLightInvalidDataWatchTimeout { get; set; }
    public int AmbientLightInvalidValueWatchTimeout { get; set; }
    #endregion

    #region Ammonia Settings
    public double AmmoniaMinValue { get; set; }
    public double AmmoniaMaxValue { get; set; }
    public int AmmoniaInvalidDataWatchTimeout { get; set; }
    public int AmmoniaInvalidValueWatchTimeout { get; set; }
    #endregion

    #region Co2 Settings
    public double Co2MinValue { get; set; }
    public double Co2MaxValue { get; set; }
    public int Co2InvalidDataWatchTimeout { get; set; }
    public int Co2InvalidValueWatchTimeout { get; set; }
    #endregion

    #region Commute Settings
    public bool WriteCommuteToDbAlways { get; set; }
    public int CommuteNotAliveWatchTimeout { get; set; }
    public int CommuteInvalidDataWatchTimeout { get; set; }
    public int CommuteInvalidValueWatchTimeout { get; set; }
    #endregion

    #region Feed Settings
    public bool WriteFeedToDbAlways { get; set; }
    public int FeedNotAliveWatchTimeout { get; set; }
    #endregion

    #region Checkup Settings
    public bool WriteCheckupToDbAlways { get; set; }
    public int CheckupNotAliveWatchTimeout { get; set; }
    #endregion

    #region Farm Power Sensor Settings
    public double FarmPowerReadInterval { get; set; }
    public bool WriteFarmPowerOnValueChange { get; set; }
    public int WriteFarmPowerToDbInterval { get; set; }
    public bool WriteFarmPowerToDbAlways { get; set; }
    public int FarmPowerNotAliveWatchTimeout { get; set; }
    public int FarmPowerInvalidDataWatchTimeout { get; set; }
    #endregion

    #region Main Power Sensor Settings
    public double MainPowerReadInterval { get; set; }
    public bool WriteMainPowerOnValueChange { get; set; }
    public int WriteMainPowerToDbInterval { get; set; }
    public bool WriteMainPowerToDbAlways { get; set; }
    public int MainPowerNotAliveWatchTimeout { get; set; }
    public int MainPowerInvalidDataWatchTimeout { get; set; }
    #endregion

    #region Backup Power Sensor Settings
    public double BackupPowerReadInterval { get; set; }
    public bool WriteBackupPowerOnValueChange { get; set; }
    public int WriteBackupPowerToDbInterval { get; set; }
    public bool WriteBackupPowerToDbAlways { get; set; }
    public int BackupPowerNotAliveWatchTimeout { get; set; }
    public int BackupPowerInvalidDataWatchTimeout { get; set; }
    #endregion

    #region Observer Settings
    public int ObserverCheckInterval { get; set; }
    public bool ObserveAlways { get; set; }
    #endregion
}