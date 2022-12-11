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

        ScalarReadInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:read_interval").Value ?? "30");
        WriteScalarToDbInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:write_to_db_interval").Value ?? "30");
        WriteScalarToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:write_to_db_always").Value ?? "false");

        FarmTempMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:farm_min_value").Value ?? "20");
        FarmTempMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:farm_max_value").Value ?? "35");
        OutdoorTempMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:outdoor_min_value").Value ?? "-10");
        OutdoorTempMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:outdoor_max_value").Value ?? "60");
        TempFirstAlarmTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm_first_time").Value ?? "60");
        TempAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm_every").Value ?? "60");
        TempAlarmSnoozeTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm_snooze").Value ?? "1000");
        TempAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm_count_in_cycle").Value ?? "3");


        HumidMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:min_value").Value ?? "20");
        HumidMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:max_value").Value ?? "35");
        HumidFirstAlarmTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm_first_time").Value ?? "60");
        HumidAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm_every").Value ?? "60");
        HumidAlarmSnoozeTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm_snooze").Value ?? "1000");
        HumidAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm_count_in_cycle").Value ?? "3");

        AmbientLightMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:min_value").Value ?? "20");
        AmbientLightMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:max_value").Value ?? "35");
        AmbientLightFirstAlarmTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm_first_time").Value ?? "60");
        AmbientLightAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm_every").Value ?? "60");
        AmbientLightAlarmSnoozeTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm_snooze").Value ?? "1000");
        AmbientLightAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm_count_in_cycle").Value ?? "3");

        AmmoniaMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:min_value").Value ?? "0");
        AmmoniaMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:max_value").Value ?? "100");
        AmmoniaFirstAlarmTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm_first_time").Value ?? "60");
        AmmoniaAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm_every").Value ?? "60");
        AmmoniaAlarmSnoozeTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm_snooze").Value ?? "1000");
        AmmoniaAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm_count_in_cycle").Value ?? "3");

        Co2MinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:min_value").Value ?? "0");
        Co2MaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:max_value").Value ?? "100");
        Co2FirstAlarmTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm_first_time").Value ?? "60");
        Co2AlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm_every").Value ?? "60");
        Co2AlarmSnoozeTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm_snooze").Value ?? "1000");
        Co2AlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm_count_in_cycle").Value ?? "3");

        WriteCommuteToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:write_to_db_always").Value ?? "false");
        CommuteFirstAlarmTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm_first_time").Value ?? "60");
        CommuteAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm_every").Value ?? "60");
        CommuteAlarmSnoozeTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm_snooze").Value ?? "1000");
        CommuteAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm_count_in_cycle").Value ?? "3");

        WritePushButtonToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("push_button:write_to_db_always").Value ?? "false");
        PushButtonFirstAlarmTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("push_button:alarm_first_time").Value ?? "60");
        PushButtonAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("push_button:alarm_every").Value ?? "60");
        PushButtonAlarmSnoozeTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("push_button:alarm_snooze").Value ?? "1000");
        PushButtonAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("push_button:alarm_count_in_cycle").Value ?? "3");

        BinaryReadInterval = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("binary:read_interval").Value ?? "30");
        WriteBinaryOnValueChange = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("binary:write_on_value_change").Value ?? "true");
        WriteBinaryToDbInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("binary:write_to_db_interval").Value ?? "30");
        WriteBinaryToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("binary:write_to_db_always").Value ?? "false");
        BinaryFirstAlarmTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("binary:alarm_first_time").Value ?? "60");
        BinaryAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("binary:alarm_every").Value ?? "60");
        BinaryAlarmSnoozeTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("binary:alarm_snooze").Value ?? "1000");
        BinaryAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("binary:alarm_count_in_cycle").Value ?? "3");

        AlarmLowBatteryError = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm:alarm_low_battery_error").Value ?? "true");
        AlarmNotAliveError = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm:alarm_not_alive_error").Value ?? "true");
        AlarmInvalidDataError = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm:alarm_invalid_data_error").Value ?? "true");
        AlarmInvalidValueError = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm:alarm_invalid_value_error").Value ?? "true");
        AlarmTempSensorError = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm:alarm_temperature_sensor_error").Value ?? "true");
        AlarmHumidSensorError = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm:alarm_humidity_sensor_error").Value ?? "true");
        AlarmLightSensorError = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm:alarm_light_sensor_error").Value ?? "true");
        AlarmAmmoniaSensorError = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm:alarm_ammonia_sensor_error").Value ?? "true");
        AlarmCo2SensorError = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm:alarm_co2_sensor_error").Value ?? "true");
        AlarmCommuteSensorError = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm:alarm_commute_sensor_error").Value ?? "true");
        AlarmPushButtonSensorError = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm:alarm_push_button_sensor_error").Value ?? "true");
        AlarmBinarySensorError = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm:alarm_binary_sensor_error").Value ?? "true");


        ObserverCheckInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("observer:observer_check_interval").Value ?? "5");
        ObserveAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("observer:observe_always").Value ?? "false");
    }

    public int KeepAliveInterval { get; set; }
    public bool IsKeepAliveEnabled => KeepAliveInterval > 0;
    public int KeepAliveWaitingTimeout { get; set; }
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
    public int TempFirstAlarmTime { get; set; }
    public int TempAlarmEvery { get; set; }
    public int TempAlarmSnoozeTime { get; set; }
    public int TempAlarmCountInCycle { get; set; }
    #endregion

    #region Humidity Settings
    public double HumidMinValue { get; set; }
    public double HumidMaxValue { get; set; }
    public int HumidFirstAlarmTime { get; set; }
    public int HumidAlarmEvery { get; set; }
    public int HumidAlarmSnoozeTime { get; set; }
    public int HumidAlarmCountInCycle { get; set; }
    #endregion

    #region Ambient Light Settings
    public double AmbientLightMinValue { get; set; }
    public double AmbientLightMaxValue { get; set; }
    public int AmbientLightFirstAlarmTime { get; set; }
    public int AmbientLightAlarmEvery { get; set; }
    public int AmbientLightAlarmSnoozeTime { get; set; }
    public int AmbientLightAlarmCountInCycle { get; set; }
    #endregion

    #region Ammonia Settings
    public double AmmoniaMinValue { get; set; }
    public double AmmoniaMaxValue { get; set; }
    public int AmmoniaFirstAlarmTime { get; set; }
    public int AmmoniaAlarmEvery { get; set; }
    public int AmmoniaAlarmSnoozeTime { get; set; }
    public int AmmoniaAlarmCountInCycle { get; set; }
    #endregion

    #region Co2 Settings
    public double Co2MinValue { get; set; }
    public double Co2MaxValue { get; set; }
    public int Co2FirstAlarmTime { get; set; }
    public int Co2AlarmEvery { get; set; }
    public int Co2AlarmSnoozeTime { get; set; }
    public int Co2AlarmCountInCycle { get; set; }
    #endregion

    #region Commute Settings
    public bool WriteCommuteToDbAlways { get; set; }
    public int CommuteFirstAlarmTime { get; set; }
    public int CommuteAlarmEvery { get; set; }
    public int CommuteAlarmSnoozeTime { get; set; }
    public int CommuteAlarmCountInCycle { get; set; }
    #endregion

    #region Push Button Settings
    public bool WritePushButtonToDbAlways { get; set; }
    public int PushButtonFirstAlarmTime { get; set; }
    public int PushButtonAlarmEvery { get; set; }
    public int PushButtonAlarmSnoozeTime { get; set; }
    public int PushButtonAlarmCountInCycle { get; set; }
    #endregion

    #region Binary Settings
    public double BinaryReadInterval { get; set; }
    public bool WriteBinaryOnValueChange { get; set; }
    public int WriteBinaryToDbInterval { get; set; }
    public bool WriteBinaryToDbAlways { get; set; }
    public int BinaryFirstAlarmTime { get; set; }
    public int BinaryAlarmEvery { get; set; }
    public int BinaryAlarmSnoozeTime { get; set; }
    public int BinaryAlarmCountInCycle { get; set; }
    #endregion

    #region Observer Settings
    public int ObserverCheckInterval { get; set; }
    public bool ObserveAlways { get; set; }
    #endregion

    #region Alarm Settings
    public bool AlarmLowBatteryError { get; set; }
    public bool AlarmNotAliveError { get; set; }
    public bool AlarmInvalidDataError { get; set; }
    public bool AlarmInvalidValueError { get; set; }
    public bool AlarmTempSensorError { get; set; }
    public bool AlarmHumidSensorError { get; set; }
    public bool AlarmLightSensorError { get; set; }
    public bool AlarmAmmoniaSensorError { get; set; }
    public bool AlarmCo2SensorError { get; set; }
    public bool AlarmCommuteSensorError { get; set; }
    public bool AlarmPushButtonSensorError { get; set; }
    public bool AlarmBinarySensorError { get; set; }
    #endregion
}