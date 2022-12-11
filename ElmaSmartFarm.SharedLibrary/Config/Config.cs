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
        AlarmScalarLowBatteryEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:low_battery_alarm_enable").Value ?? "true");
        AlarmScalarLowBatteryFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:low_battery_alarm_first_time").Value ?? "120");
        AlarmScalarLowBatteryEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:low_battery_alarm_every").Value ?? "300");
        AlarmScalarLowBatterySnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:low_battery_alarm_snooze").Value ?? "3000");
        AlarmScalarLowBatteryCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:low_battery_alarm_count_in_cycle").Value ?? "3");
        AlarmScalarNotAliveEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:not_alive_alarm_enable").Value ?? "true");
        AlarmScalarNotAliveFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:not_alive_alarm_first_time").Value ?? "120");
        AlarmScalarNotAliveEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:not_alive_alarm_every").Value ?? "300");
        AlarmScalarNotAliveSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:not_alive_alarm_snooze").Value ?? "3000");
        AlarmScalarNotAliveCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:not_alive_alarm_count_in_cycle").Value ?? "3");

        FarmTempMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:farm_min_value").Value ?? "20");
        FarmTempMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:farm_max_value").Value ?? "35");
        OutdoorTempMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:outdoor_min_value").Value ?? "-10");
        OutdoorTempMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:outdoor_max_value").Value ?? "60");
        AlarmTempInvalidDataEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:invalid_data_alarm_enable").Value ?? "true");
        AlarmTempInvalidDataFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:invalid_data_alarm_first_time").Value ?? "120");
        AlarmTempInvalidDataEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:invalid_data_alarm_every").Value ?? "300");
        AlarmTempInvalidDataSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:invalid_data_alarm_snooze").Value ?? "3000");
        AlarmTempInvalidDataCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:invalid_data_alarm_count_in_cycle").Value ?? "3");
        AlarmTempInvalidValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:invalid_value_alarm_enable").Value ?? "true");
        AlarmTempInvalidValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:invalid_value_alarm_first_time").Value ?? "120");
        AlarmTempInvalidValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:invalid_value_alarm_every").Value ?? "300");
        AlarmTempInvalidValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:invalid_value_alarm_snooze").Value ?? "3000");
        AlarmTempInvalidValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:invalid_value_alarm_count_in_cycle").Value ?? "3");
        AlarmTempHighValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:high_value_alarm_enable").Value ?? "true");
        AlarmTempHighValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:high_value_alarm_first_time").Value ?? "120");
        AlarmTempHighValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:high_value_alarm_every").Value ?? "300");
        AlarmTempHighValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:high_value_alarm_snooze").Value ?? "3000");
        AlarmTempHighValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:high_value_alarm_count_in_cycle").Value ?? "3");
        AlarmTempLowValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:low_value_alarm_enable").Value ?? "true");
        AlarmTempLowValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:low_value_alarm_first_time").Value ?? "120");
        AlarmTempLowValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:low_value_alarm_every").Value ?? "300");
        AlarmTempLowValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:low_value_alarm_snooze").Value ?? "3000");
        AlarmTempLowValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:alarm:low_value_alarm_count_in_cycle").Value ?? "3");


        HumidMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:min_value").Value ?? "20");
        HumidMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:max_value").Value ?? "35");
        AlarmHumidInvalidDataEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:invalid_data_alarm_enable").Value ?? "true");
        AlarmHumidInvalidDataFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:invalid_data_alarm_first_time").Value ?? "120");
        AlarmHumidInvalidDataEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:invalid_data_alarm_every").Value ?? "300");
        AlarmHumidInvalidDataSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:invalid_data_alarm_snooze").Value ?? "3000");
        AlarmHumidInvalidDataCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:invalid_data_alarm_count_in_cycle").Value ?? "3");
        AlarmHumidInvalidValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:invalid_value_alarm_enable").Value ?? "true");
        AlarmHumidInvalidValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:invalid_value_alarm_first_time").Value ?? "120");
        AlarmHumidInvalidValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:invalid_value_alarm_every").Value ?? "300");
        AlarmHumidInvalidValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:invalid_value_alarm_snooze").Value ?? "3000");
        AlarmHumidInvalidValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:invalid_value_alarm_count_in_cycle").Value ?? "3");
        AlarmHumidHighValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:high_value_alarm_enable").Value ?? "true");
        AlarmHumidHighValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:high_value_alarm_first_time").Value ?? "120");
        AlarmHumidHighValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:high_value_alarm_every").Value ?? "300");
        AlarmHumidHighValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:high_value_alarm_snooze").Value ?? "3000");
        AlarmHumidHighValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:high_value_alarm_count_in_cycle").Value ?? "3");
        AlarmHumidLowValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:low_value_alarm_enable").Value ?? "true");
        AlarmHumidLowValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:low_value_alarm_first_time").Value ?? "120");
        AlarmHumidLowValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:low_value_alarm_every").Value ?? "300");
        AlarmHumidLowValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:low_value_alarm_snooze").Value ?? "3000");
        AlarmHumidLowValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:alarm:low_value_alarm_count_in_cycle").Value ?? "3");

        AmbientLightMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:min_value").Value ?? "20");
        AmbientLightMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:max_value").Value ?? "35");
        AlarmAmbientLightInvalidDataEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:invalid_data_alarm_enable").Value ?? "true");
        AlarmAmbientLightInvalidDataFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:invalid_data_alarm_first_time").Value ?? "120");
        AlarmAmbientLightInvalidDataEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:invalid_data_alarm_every").Value ?? "300");
        AlarmAmbientLightInvalidDataSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:invalid_data_alarm_snooze").Value ?? "3000");
        AlarmAmbientLightInvalidDataCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:invalid_data_alarm_count_in_cycle").Value ?? "3");
        AlarmAmbientLightInvalidValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:invalid_value_alarm_enable").Value ?? "true");
        AlarmAmbientLightInvalidValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:invalid_value_alarm_first_time").Value ?? "120");
        AlarmAmbientLightInvalidValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:invalid_value_alarm_every").Value ?? "300");
        AlarmAmbientLightInvalidValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:invalid_value_alarm_snooze").Value ?? "3000");
        AlarmAmbientLightInvalidValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:invalid_value_alarm_count_in_cycle").Value ?? "3");
        AlarmAmbientLightLongBrightEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:long_bright_alarm_enable").Value ?? "true");
        AlarmAmbientLightLongBrightFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:long_bright_alarm_first_time").Value ?? "120");
        AlarmAmbientLightLongBrightEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:long_bright_alarm_every").Value ?? "300");
        AlarmAmbientLightLongBrightSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:long_bright_alarm_snooze").Value ?? "3000");
        AlarmAmbientLightLongBrightCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:long_bright_alarm_count_in_cycle").Value ?? "3");
        AlarmAmbientLightLongDarkEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:long_dark_alarm_enable").Value ?? "true");
        AlarmAmbientLightLongDarkFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:long_dark_alarm_first_time").Value ?? "120");
        AlarmAmbientLightLongDarkEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:long_dark_alarm_every").Value ?? "300");
        AlarmAmbientLightLongDarkSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:long_dark_alarm_snooze").Value ?? "3000");
        AlarmAmbientLightLongDarkCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:long_dark_alarm_count_in_cycle").Value ?? "3");
        AlarmAmbientLightHighValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:high_value_alarm_enable").Value ?? "true");
        AlarmAmbientLightHighValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:high_value_alarm_first_time").Value ?? "120");
        AlarmAmbientLightHighValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:high_value_alarm_every").Value ?? "300");
        AlarmAmbientLightHighValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:high_value_alarm_snooze").Value ?? "3000");
        AlarmAmbientLightHighValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:high_value_alarm_count_in_cycle").Value ?? "3");
        AlarmAmbientLightLowValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:low_value_alarm_enable").Value ?? "true");
        AlarmAmbientLightLowValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:low_value_alarm_first_time").Value ?? "120");
        AlarmAmbientLightLowValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:low_value_alarm_every").Value ?? "300");
        AlarmAmbientLightLowValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:low_value_alarm_snooze").Value ?? "3000");
        AlarmAmbientLightLowValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:alarm:low_value_alarm_count_in_cycle").Value ?? "3");

        AmmoniaMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:min_value").Value ?? "0");
        AmmoniaMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:max_value").Value ?? "100");
        AlarmAmmoniaInvalidDataEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:invalid_data_alarm_enable").Value ?? "true");
        AlarmAmmoniaInvalidDataFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:invalid_data_alarm_first_time").Value ?? "120");
        AlarmAmmoniaInvalidDataEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:invalid_data_alarm_every").Value ?? "300");
        AlarmAmmoniaInvalidDataSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:invalid_data_alarm_snooze").Value ?? "3000");
        AlarmAmmoniaInvalidDataCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:invalid_data_alarm_count_in_cycle").Value ?? "3");
        AlarmAmmoniaInvalidValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:invalid_value_alarm_enable").Value ?? "true");
        AlarmAmmoniaInvalidValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:invalid_value_alarm_first_time").Value ?? "120");
        AlarmAmmoniaInvalidValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:invalid_value_alarm_every").Value ?? "300");
        AlarmAmmoniaInvalidValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:invalid_value_alarm_snooze").Value ?? "3000");
        AlarmAmmoniaInvalidValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:invalid_value_alarm_count_in_cycle").Value ?? "3");
        AlarmAmmoniaHighValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:high_value_alarm_enable").Value ?? "true");
        AlarmAmmoniaHighValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:high_value_alarm_first_time").Value ?? "120");
        AlarmAmmoniaHighValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:high_value_alarm_every").Value ?? "300");
        AlarmAmmoniaHighValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:high_value_alarm_snooze").Value ?? "3000");
        AlarmAmmoniaHighValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:high_value_alarm_count_in_cycle").Value ?? "3");
        AlarmAmmoniaLowValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:low_value_alarm_enable").Value ?? "true");
        AlarmAmmoniaLowValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:low_value_alarm_first_time").Value ?? "120");
        AlarmAmmoniaLowValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:low_value_alarm_every").Value ?? "300");
        AlarmAmmoniaLowValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:low_value_alarm_snooze").Value ?? "3000");
        AlarmAmmoniaLowValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:alarm:low_value_alarm_count_in_cycle").Value ?? "3");

        Co2MinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:min_value").Value ?? "0");
        Co2MaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:max_value").Value ?? "100");
        AlarmCo2InvalidDataEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:invalid_data_alarm_enable").Value ?? "true");
        AlarmCo2InvalidDataFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:invalid_data_alarm_first_time").Value ?? "120");
        AlarmCo2InvalidDataEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:invalid_data_alarm_every").Value ?? "300");
        AlarmCo2InvalidDataSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:invalid_data_alarm_snooze").Value ?? "3000");
        AlarmCo2InvalidDataCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:invalid_data_alarm_count_in_cycle").Value ?? "3");
        AlarmCo2InvalidValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:invalid_value_alarm_enable").Value ?? "true");
        AlarmCo2InvalidValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:invalid_value_alarm_first_time").Value ?? "120");
        AlarmCo2InvalidValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:invalid_value_alarm_every").Value ?? "300");
        AlarmCo2InvalidValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:invalid_value_alarm_snooze").Value ?? "3000");
        AlarmCo2InvalidValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:invalid_value_alarm_count_in_cycle").Value ?? "3");
        AlarmCo2HighValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:high_value_alarm_enable").Value ?? "true");
        AlarmCo2HighValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:high_value_alarm_first_time").Value ?? "120");
        AlarmCo2HighValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:high_value_alarm_every").Value ?? "300");
        AlarmCo2HighValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:high_value_alarm_snooze").Value ?? "3000");
        AlarmCo2HighValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:high_value_alarm_count_in_cycle").Value ?? "3");
        AlarmCo2LowValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:low_value_alarm_enable").Value ?? "true");
        AlarmCo2LowValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:low_value_alarm_first_time").Value ?? "120");
        AlarmCo2LowValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:low_value_alarm_every").Value ?? "300");
        AlarmCo2LowValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:low_value_alarm_snooze").Value ?? "3000");
        AlarmCo2LowValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:alarm:low_value_alarm_count_in_cycle").Value ?? "3");

        WriteCommuteToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:write_to_db_always").Value ?? "false");
        AlarmCommuteLowBatteryEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:low_battery_alarm_enable").Value ?? "true");
        AlarmCommuteLowBatteryFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:low_battery_alarm_first_time").Value ?? "120");
        AlarmCommuteLowBatteryEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:low_battery_alarm_every").Value ?? "300");
        AlarmCommuteLowBatterySnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:low_battery_alarm_snooze").Value ?? "3000");
        AlarmCommuteLowBatteryCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:low_battery_alarm_count_in_cycle").Value ?? "3");
        AlarmCommuteNotAliveEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:not_alive_alarm_enable").Value ?? "true");
        AlarmCommuteNotAliveFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:not_alive_alarm_first_time").Value ?? "120");
        AlarmCommuteNotAliveEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:not_alive_alarm_every").Value ?? "300");
        AlarmCommuteNotAliveSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:not_alive_alarm_snooze").Value ?? "3000");
        AlarmCommuteNotAliveCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:not_alive_alarm_count_in_cycle").Value ?? "3");
        AlarmCommuteInvalidDataEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:invalid_data_alarm_enable").Value ?? "true");
        AlarmCommuteInvalidDataFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:invalid_data_alarm_first_time").Value ?? "120");
        AlarmCommuteInvalidDataEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:invalid_data_alarm_every").Value ?? "300");
        AlarmCommuteInvalidDataSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:invalid_data_alarm_snooze").Value ?? "3000");
        AlarmCommuteInvalidDataCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:invalid_data_alarm_count_in_cycle").Value ?? "3");
        AlarmCommuteInvalidValueEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:invalid_value_alarm_enable").Value ?? "true");
        AlarmCommuteInvalidValueFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:invalid_value_alarm_first_time").Value ?? "120");
        AlarmCommuteInvalidValueEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:invalid_value_alarm_every").Value ?? "300");
        AlarmCommuteInvalidValueSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:invalid_value_alarm_snooze").Value ?? "3000");
        AlarmCommuteInvalidValueCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("commute:alarm:invalid_value_alarm_count_in_cycle").Value ?? "3");

        WriteFeedToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:write_to_db_always").Value ?? "false");
        AlarmFeedLowBatteryEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:low_battery_alarm_enable").Value ?? "true");
        AlarmFeedLowBatteryFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:low_battery_alarm_first_time").Value ?? "120");
        AlarmFeedLowBatteryEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:low_battery_alarm_every").Value ?? "300");
        AlarmFeedLowBatterySnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:low_battery_alarm_snooze").Value ?? "3000");
        AlarmFeedLowBatteryCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:low_battery_alarm_count_in_cycle").Value ?? "3");
        AlarmFeedNotAliveEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:not_alive_alarm_enable").Value ?? "true");
        AlarmFeedNotAliveFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:not_alive_alarm_first_time").Value ?? "120");
        AlarmFeedNotAliveEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:not_alive_alarm_every").Value ?? "300");
        AlarmFeedNotAliveSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:not_alive_alarm_snooze").Value ?? "3000");
        AlarmFeedNotAliveCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:not_alive_alarm_count_in_cycle").Value ?? "3");
        AlarmFeedLongNoFeedEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:long_nofeed_alarm_enable").Value ?? "true");
        AlarmFeedLongNoFeedFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:long_nofeed_alarm_first_time").Value ?? "120");
        AlarmFeedLongNoFeedEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:long_nofeed_alarm_every").Value ?? "300");
        AlarmFeedLongNoFeedSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:long_nofeed_alarm_snooze").Value ?? "3000");
        AlarmFeedLongNoFeedCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("feed:alarm:long_nofeed_alarm_count_in_cycle").Value ?? "3");

        WriteCheckupToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:write_to_db_always").Value ?? "false");
        AlarmCheckupLowBatteryEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:low_battery_alarm_enable").Value ?? "true");
        AlarmCheckupLowBatteryFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:low_battery_alarm_first_time").Value ?? "120");
        AlarmCheckupLowBatteryEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:low_battery_alarm_every").Value ?? "300");
        AlarmCheckupLowBatterySnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:low_battery_alarm_snooze").Value ?? "3000");
        AlarmCheckupLowBatteryCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:low_battery_alarm_count_in_cycle").Value ?? "3");
        AlarmCheckupNotAliveEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:not_alive_alarm_enable").Value ?? "true");
        AlarmCheckupNotAliveFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:not_alive_alarm_first_time").Value ?? "120");
        AlarmCheckupNotAliveEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:not_alive_alarm_every").Value ?? "300");
        AlarmCheckupNotAliveSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:not_alive_alarm_snooze").Value ?? "3000");
        AlarmCheckupNotAliveCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:not_alive_alarm_count_in_cycle").Value ?? "3");
        AlarmCheckupLongLeaveEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:long_nofeed_alarm_enable").Value ?? "true");
        AlarmCheckupLongLeaveFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:long_nofeed_alarm_first_time").Value ?? "120");
        AlarmCheckupLongLeaveEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:long_nofeed_alarm_every").Value ?? "300");
        AlarmCheckupLongLeaveSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:long_nofeed_alarm_snooze").Value ?? "3000");
        AlarmCheckupLongLeaveCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("checkup:alarm:long_nofeed_alarm_count_in_cycle").Value ?? "3");

        FarmPowerReadInterval=int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:read_interval").Value ?? "60");
        WriteFarmPowerOnValueChange= bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:write_on_value_change").Value ?? "true");
        WriteFarmPowerToDbInterval=int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:write_to_db_interval").Value ?? "60");
        WriteFarmPowerToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:write_to_db_always").Value ?? "false");
        AlarmFarmPowerLowBatteryEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:low_battery_alarm_enable").Value ?? "true");
        AlarmFarmPowerLowBatteryFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:low_battery_alarm_first_time").Value ?? "120");
        AlarmFarmPowerLowBatteryEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:low_battery_alarm_every").Value ?? "300");
        AlarmFarmPowerLowBatterySnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:low_battery_alarm_snooze").Value ?? "3000");
        AlarmFarmPowerLowBatteryCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:low_battery_alarm_count_in_cycle").Value ?? "3");
        AlarmFarmPowerNotAliveEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:not_alive_alarm_enable").Value ?? "true");
        AlarmFarmPowerNotAliveFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:not_alive_alarm_first_time").Value ?? "120");
        AlarmFarmPowerNotAliveEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:not_alive_alarm_every").Value ?? "300");
        AlarmFarmPowerNotAliveSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:not_alive_alarm_snooze").Value ?? "3000");
        AlarmFarmPowerNotAliveCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:not_alive_alarm_count_in_cycle").Value ?? "3");
        AlarmFarmPowerNoPowerEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:long_nofeed_alarm_enable").Value ?? "true");
        AlarmFarmPowerNoPowerFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:long_nofeed_alarm_first_time").Value ?? "120");
        AlarmFarmPowerNoPowerEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:long_nofeed_alarm_every").Value ?? "300");
        AlarmFarmPowerNoPowerSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:long_nofeed_alarm_snooze").Value ?? "3000");
        AlarmFarmPowerNoPowerCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("farm_power:alarm:long_nofeed_alarm_count_in_cycle").Value ?? "3");

        MainPowerReadInterval=int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:read_interval").Value ?? "60");
        WriteMainPowerOnValueChange= bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:write_on_value_change").Value ?? "true");
        WriteMainPowerToDbInterval=int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:write_to_db_interval").Value ?? "60");
        WriteMainPowerToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:write_to_db_always").Value ?? "false");
        AlarmMainPowerLowBatteryEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:low_battery_alarm_enable").Value ?? "true");
        AlarmMainPowerLowBatteryFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:low_battery_alarm_first_time").Value ?? "120");
        AlarmMainPowerLowBatteryEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:low_battery_alarm_every").Value ?? "300");
        AlarmMainPowerLowBatterySnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:low_battery_alarm_snooze").Value ?? "3000");
        AlarmMainPowerLowBatteryCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:low_battery_alarm_count_in_cycle").Value ?? "3");
        AlarmMainPowerNotAliveEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:not_alive_alarm_enable").Value ?? "true");
        AlarmMainPowerNotAliveFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:not_alive_alarm_first_time").Value ?? "120");
        AlarmMainPowerNotAliveEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:not_alive_alarm_every").Value ?? "300");
        AlarmMainPowerNotAliveSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:not_alive_alarm_snooze").Value ?? "3000");
        AlarmMainPowerNotAliveCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:not_alive_alarm_count_in_cycle").Value ?? "3");
        AlarmMainPowerNoPowerEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:long_nofeed_alarm_enable").Value ?? "true");
        AlarmMainPowerNoPowerFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:long_nofeed_alarm_first_time").Value ?? "120");
        AlarmMainPowerNoPowerEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:long_nofeed_alarm_every").Value ?? "300");
        AlarmMainPowerNoPowerSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:long_nofeed_alarm_snooze").Value ?? "3000");
        AlarmMainPowerNoPowerCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("main_power:alarm:long_nofeed_alarm_count_in_cycle").Value ?? "3");

        BackupPowerReadInterval=int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:read_interval").Value ?? "60");
        WriteBackupPowerOnValueChange= bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:write_on_value_change").Value ?? "true");
        WriteBackupPowerToDbInterval=int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:write_to_db_interval").Value ?? "60");
        WriteBackupPowerToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:write_to_db_always").Value ?? "false");
        AlarmBackupPowerLowBatteryEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:low_battery_alarm_enable").Value ?? "true");
        AlarmBackupPowerLowBatteryFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:low_battery_alarm_first_time").Value ?? "120");
        AlarmBackupPowerLowBatteryEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:low_battery_alarm_every").Value ?? "300");
        AlarmBackupPowerLowBatterySnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:low_battery_alarm_snooze").Value ?? "3000");
        AlarmBackupPowerLowBatteryCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:low_battery_alarm_count_in_cycle").Value ?? "3");
        AlarmBackupPowerNotAliveEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:not_alive_alarm_enable").Value ?? "true");
        AlarmBackupPowerNotAliveFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:not_alive_alarm_first_time").Value ?? "120");
        AlarmBackupPowerNotAliveEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:not_alive_alarm_every").Value ?? "300");
        AlarmBackupPowerNotAliveSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:not_alive_alarm_snooze").Value ?? "3000");
        AlarmBackupPowerNotAliveCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:not_alive_alarm_count_in_cycle").Value ?? "3");
        AlarmBackupPowerNoPowerEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:long_nofeed_alarm_enable").Value ?? "true");
        AlarmBackupPowerNoPowerFirstTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:long_nofeed_alarm_first_time").Value ?? "120");
        AlarmBackupPowerNoPowerEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:long_nofeed_alarm_every").Value ?? "300");
        AlarmBackupPowerNoPowerSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:long_nofeed_alarm_snooze").Value ?? "3000");
        AlarmBackupPowerNoPowerCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("backup_power:alarm:long_nofeed_alarm_count_in_cycle").Value ?? "3");

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
    public bool AlarmScalarLowBatteryEnable { get; set; }
    public int AlarmScalarLowBatteryFirstTime { get; set; }
    public int AlarmScalarLowBatteryEvery { get; set; }
    public int AlarmScalarLowBatterySnooze { get; set; }
    public int AlarmScalarLowBatteryCountInCycle { get; set; }
    public bool AlarmScalarNotAliveEnable { get; set; }
    public int AlarmScalarNotAliveFirstTime { get; set; }
    public int AlarmScalarNotAliveEvery { get; set; }
    public int AlarmScalarNotAliveSnooze { get; set; }
    public int AlarmScalarNotAliveCountInCycle { get; set; }
    #endregion

    #region Temperature Settings
    public double FarmTempMinValue { get; set; }
    public double FarmTempMaxValue { get; set; }
    public double OutdoorTempMinValue { get; set; }
    public double OutdoorTempMaxValue { get; set; }
    public bool AlarmTempInvalidDataEnable { get; set; }
    public int AlarmTempInvalidDataFirstTime { get; set; }
    public int AlarmTempInvalidDataEvery { get; set; }
    public int AlarmTempInvalidDataSnooze { get; set; }
    public int AlarmTempInvalidDataCountInCycle { get; set; }
    public bool AlarmTempInvalidValueEnable { get; set; }
    public int AlarmTempInvalidValueFirstTime { get; set; }
    public int AlarmTempInvalidValueEvery { get; set; }
    public int AlarmTempInvalidValueSnooze { get; set; }
    public int AlarmTempInvalidValueCountInCycle { get; set; }
    public bool AlarmTempHighValueEnable { get; set; }
    public int AlarmTempHighValueFirstTime { get; set; }
    public int AlarmTempHighValueEvery { get; set; }
    public int AlarmTempHighValueSnooze { get; set; }
    public int AlarmTempHighValueCountInCycle { get; set; }
    public bool AlarmTempLowValueEnable { get; set; }
    public int AlarmTempLowValueFirstTime { get; set; }
    public int AlarmTempLowValueEvery { get; set; }
    public int AlarmTempLowValueSnooze { get; set; }
    public int AlarmTempLowValueCountInCycle { get; set; }
    #endregion

    #region Humidity Settings
    public double HumidMinValue { get; set; }
    public double HumidMaxValue { get; set; }
    public bool AlarmHumidInvalidDataEnable { get; set; }
    public int AlarmHumidInvalidDataFirstTime { get; set; }
    public int AlarmHumidInvalidDataEvery { get; set; }
    public int AlarmHumidInvalidDataSnooze { get; set; }
    public int AlarmHumidInvalidDataCountInCycle { get; set; }
    public bool AlarmHumidInvalidValueEnable { get; set; }
    public int AlarmHumidInvalidValueFirstTime { get; set; }
    public int AlarmHumidInvalidValueEvery { get; set; }
    public int AlarmHumidInvalidValueSnooze { get; set; }
    public int AlarmHumidInvalidValueCountInCycle { get; set; }
    public bool AlarmHumidHighValueEnable { get; set; }
    public int AlarmHumidHighValueFirstTime { get; set; }
    public int AlarmHumidHighValueEvery { get; set; }
    public int AlarmHumidHighValueSnooze { get; set; }
    public int AlarmHumidHighValueCountInCycle { get; set; }
    public bool AlarmHumidLowValueEnable { get; set; }
    public int AlarmHumidLowValueFirstTime { get; set; }
    public int AlarmHumidLowValueEvery { get; set; }
    public int AlarmHumidLowValueSnooze { get; set; }
    public int AlarmHumidLowValueCountInCycle { get; set; }
    #endregion

    #region Ambient Light Settings
    public double AmbientLightMinValue { get; set; }
    public double AmbientLightMaxValue { get; set; }
    public bool AlarmAmbientLightInvalidDataEnable { get; set; }
    public int AlarmAmbientLightInvalidDataFirstTime { get; set; }
    public int AlarmAmbientLightInvalidDataEvery { get; set; }
    public int AlarmAmbientLightInvalidDataSnooze { get; set; }
    public int AlarmAmbientLightInvalidDataCountInCycle { get; set; }
    public bool AlarmAmbientLightInvalidValueEnable { get; set; }
    public int AlarmAmbientLightInvalidValueFirstTime { get; set; }
    public int AlarmAmbientLightInvalidValueEvery { get; set; }
    public int AlarmAmbientLightInvalidValueSnooze { get; set; }
    public int AlarmAmbientLightInvalidValueCountInCycle { get; set; }
    public bool AlarmAmbientLightLongBrightEnable { get; set; }
    public int AlarmAmbientLightLongBrightFirstTime { get; set; }
    public int AlarmAmbientLightLongBrightEvery { get; set; }
    public int AlarmAmbientLightLongBrightSnooze { get; set; }
    public int AlarmAmbientLightLongBrightCountInCycle { get; set; }
    public bool AlarmAmbientLightLongDarkEnable { get; set; }
    public int AlarmAmbientLightLongDarkFirstTime { get; set; }
    public int AlarmAmbientLightLongDarkEvery { get; set; }
    public int AlarmAmbientLightLongDarkSnooze { get; set; }
    public int AlarmAmbientLightLongDarkCountInCycle { get; set; }
    public bool AlarmAmbientLightHighValueEnable { get; set; }
    public int AlarmAmbientLightHighValueFirstTime { get; set; }
    public int AlarmAmbientLightHighValueEvery { get; set; }
    public int AlarmAmbientLightHighValueSnooze { get; set; }
    public int AlarmAmbientLightHighValueCountInCycle { get; set; }
    public bool AlarmAmbientLightLowValueEnable { get; set; }
    public int AlarmAmbientLightLowValueFirstTime { get; set; }
    public int AlarmAmbientLightLowValueEvery { get; set; }
    public int AlarmAmbientLightLowValueSnooze { get; set; }
    public int AlarmAmbientLightLowValueCountInCycle { get; set; }
    #endregion

    #region Ammonia Settings
    public double AmmoniaMinValue { get; set; }
    public double AmmoniaMaxValue { get; set; }
    public bool AlarmAmmoniaInvalidDataEnable { get; set; }
    public int AlarmAmmoniaInvalidDataFirstTime { get; set; }
    public int AlarmAmmoniaInvalidDataEvery { get; set; }
    public int AlarmAmmoniaInvalidDataSnooze { get; set; }
    public int AlarmAmmoniaInvalidDataCountInCycle { get; set; }
    public bool AlarmAmmoniaInvalidValueEnable { get; set; }
    public int AlarmAmmoniaInvalidValueFirstTime { get; set; }
    public int AlarmAmmoniaInvalidValueEvery { get; set; }
    public int AlarmAmmoniaInvalidValueSnooze { get; set; }
    public int AlarmAmmoniaInvalidValueCountInCycle { get; set; }
    public bool AlarmAmmoniaHighValueEnable { get; set; }
    public int AlarmAmmoniaHighValueFirstTime { get; set; }
    public int AlarmAmmoniaHighValueEvery { get; set; }
    public int AlarmAmmoniaHighValueSnooze { get; set; }
    public int AlarmAmmoniaHighValueCountInCycle { get; set; }
    public bool AlarmAmmoniaLowValueEnable { get; set; }
    public int AlarmAmmoniaLowValueFirstTime { get; set; }
    public int AlarmAmmoniaLowValueEvery { get; set; }
    public int AlarmAmmoniaLowValueSnooze { get; set; }
    public int AlarmAmmoniaLowValueCountInCycle { get; set; }
    #endregion

    #region Co2 Settings
    public double Co2MinValue { get; set; }
    public double Co2MaxValue { get; set; }
    public bool AlarmCo2InvalidDataEnable { get; set; }
    public int AlarmCo2InvalidDataFirstTime { get; set; }
    public int AlarmCo2InvalidDataEvery { get; set; }
    public int AlarmCo2InvalidDataSnooze { get; set; }
    public int AlarmCo2InvalidDataCountInCycle { get; set; }
    public bool AlarmCo2InvalidValueEnable { get; set; }
    public int AlarmCo2InvalidValueFirstTime { get; set; }
    public int AlarmCo2InvalidValueEvery { get; set; }
    public int AlarmCo2InvalidValueSnooze { get; set; }
    public int AlarmCo2InvalidValueCountInCycle { get; set; }
    public bool AlarmCo2HighValueEnable { get; set; }
    public int AlarmCo2HighValueFirstTime { get; set; }
    public int AlarmCo2HighValueEvery { get; set; }
    public int AlarmCo2HighValueSnooze { get; set; }
    public int AlarmCo2HighValueCountInCycle { get; set; }
    public bool AlarmCo2LowValueEnable { get; set; }
    public int AlarmCo2LowValueFirstTime { get; set; }
    public int AlarmCo2LowValueEvery { get; set; }
    public int AlarmCo2LowValueSnooze { get; set; }
    public int AlarmCo2LowValueCountInCycle { get; set; }
    #endregion

    #region Commute Settings
    public bool WriteCommuteToDbAlways { get; set; }
    public bool AlarmCommuteLowBatteryEnable { get; set; }
    public int AlarmCommuteLowBatteryFirstTime { get; set; }
    public int AlarmCommuteLowBatteryEvery { get; set; }
    public int AlarmCommuteLowBatterySnooze { get; set; }
    public int AlarmCommuteLowBatteryCountInCycle { get; set; }
    public bool AlarmCommuteNotAliveEnable { get; set; }
    public int AlarmCommuteNotAliveFirstTime { get; set; }
    public int AlarmCommuteNotAliveEvery { get; set; }
    public int AlarmCommuteNotAliveSnooze { get; set; }
    public int AlarmCommuteNotAliveCountInCycle { get; set; }
    public bool AlarmCommuteInvalidDataEnable { get; set; }
    public int AlarmCommuteInvalidDataFirstTime { get; set; }
    public int AlarmCommuteInvalidDataEvery { get; set; }
    public int AlarmCommuteInvalidDataSnooze { get; set; }
    public int AlarmCommuteInvalidDataCountInCycle { get; set; }
    public bool AlarmCommuteInvalidValueEnable { get; set; }
    public int AlarmCommuteInvalidValueFirstTime { get; set; }
    public int AlarmCommuteInvalidValueEvery { get; set; }
    public int AlarmCommuteInvalidValueSnooze { get; set; }
    public int AlarmCommuteInvalidValueCountInCycle { get; set; }
    #endregion

    #region Feed Settings
    public bool WriteFeedToDbAlways { get; set; }
    public bool AlarmFeedLowBatteryEnable { get; set; }
    public int AlarmFeedLowBatteryFirstTime { get; set; }
    public int AlarmFeedLowBatteryEvery { get; set; }
    public int AlarmFeedLowBatterySnooze { get; set; }
    public int AlarmFeedLowBatteryCountInCycle { get; set; }
    public bool AlarmFeedNotAliveEnable { get; set; }
    public int AlarmFeedNotAliveFirstTime { get; set; }
    public int AlarmFeedNotAliveEvery { get; set; }
    public int AlarmFeedNotAliveSnooze { get; set; }
    public int AlarmFeedNotAliveCountInCycle { get; set; }
    public bool AlarmFeedLongNoFeedEnable { get; set; }
    public int AlarmFeedLongNoFeedFirstTime { get; set; }
    public int AlarmFeedLongNoFeedEvery { get; set; }
    public int AlarmFeedLongNoFeedSnooze { get; set; }
    public int AlarmFeedLongNoFeedCountInCycle { get; set; }
    #endregion

    #region Checkup Settings
    public bool WriteCheckupToDbAlways { get; set; }
    public bool AlarmCheckupLowBatteryEnable { get; set; }
    public int AlarmCheckupLowBatteryFirstTime { get; set; }
    public int AlarmCheckupLowBatteryEvery { get; set; }
    public int AlarmCheckupLowBatterySnooze { get; set; }
    public int AlarmCheckupLowBatteryCountInCycle { get; set; }
    public bool AlarmCheckupNotAliveEnable { get; set; }
    public int AlarmCheckupNotAliveFirstTime { get; set; }
    public int AlarmCheckupNotAliveEvery { get; set; }
    public int AlarmCheckupNotAliveSnooze { get; set; }
    public int AlarmCheckupNotAliveCountInCycle { get; set; }
    public bool AlarmCheckupLongLeaveEnable { get; set; }
    public int AlarmCheckupLongLeaveFirstTime { get; set; }
    public int AlarmCheckupLongLeaveEvery { get; set; }
    public int AlarmCheckupLongLeaveSnooze { get; set; }
    public int AlarmCheckupLongLeaveCountInCycle { get; set; }
    #endregion

    #region Farm Power Sensor Settings
    public double FarmPowerReadInterval { get; set; }
    public bool WriteFarmPowerOnValueChange { get; set; }
    public int WriteFarmPowerToDbInterval { get; set; }
    public bool WriteFarmPowerToDbAlways { get; set; }
    public bool AlarmFarmPowerLowBatteryEnable { get; set; }
    public int AlarmFarmPowerLowBatteryFirstTime { get; set; }
    public int AlarmFarmPowerLowBatteryEvery { get; set; }
    public int AlarmFarmPowerLowBatterySnooze { get; set; }
    public int AlarmFarmPowerLowBatteryCountInCycle { get; set; }
    public bool AlarmFarmPowerNotAliveEnable { get; set; }
    public int AlarmFarmPowerNotAliveFirstTime { get; set; }
    public int AlarmFarmPowerNotAliveEvery { get; set; }
    public int AlarmFarmPowerNotAliveSnooze { get; set; }
    public int AlarmFarmPowerNotAliveCountInCycle { get; set; }
    public bool AlarmFarmPowerNoPowerEnable { get; set; }
    public int AlarmFarmPowerNoPowerFirstTime { get; set; }
    public int AlarmFarmPowerNoPowerEvery { get; set; }
    public int AlarmFarmPowerNoPowerSnooze { get; set; }
    public int AlarmFarmPowerNoPowerCountInCycle { get; set; }
    #endregion

    #region Main Power Sensor Settings
    public double MainPowerReadInterval { get; set; }
    public bool WriteMainPowerOnValueChange { get; set; }
    public int WriteMainPowerToDbInterval { get; set; }
    public bool WriteMainPowerToDbAlways { get; set; }
    public bool AlarmMainPowerLowBatteryEnable { get; set; }
    public int AlarmMainPowerLowBatteryFirstTime { get; set; }
    public int AlarmMainPowerLowBatteryEvery { get; set; }
    public int AlarmMainPowerLowBatterySnooze { get; set; }
    public int AlarmMainPowerLowBatteryCountInCycle { get; set; }
    public bool AlarmMainPowerNotAliveEnable { get; set; }
    public int AlarmMainPowerNotAliveFirstTime { get; set; }
    public int AlarmMainPowerNotAliveEvery { get; set; }
    public int AlarmMainPowerNotAliveSnooze { get; set; }
    public int AlarmMainPowerNotAliveCountInCycle { get; set; }
    public bool AlarmMainPowerNoPowerEnable { get; set; }
    public int AlarmMainPowerNoPowerFirstTime { get; set; }
    public int AlarmMainPowerNoPowerEvery { get; set; }
    public int AlarmMainPowerNoPowerSnooze { get; set; }
    public int AlarmMainPowerNoPowerCountInCycle { get; set; }
    #endregion

    #region Backup Power Sensor Settings
    public double BackupPowerReadInterval { get; set; }
    public bool WriteBackupPowerOnValueChange { get; set; }
    public int WriteBackupPowerToDbInterval { get; set; }
    public bool WriteBackupPowerToDbAlways { get; set; }
    public bool AlarmBackupPowerLowBatteryEnable { get; set; }
    public int AlarmBackupPowerLowBatteryFirstTime { get; set; }
    public int AlarmBackupPowerLowBatteryEvery { get; set; }
    public int AlarmBackupPowerLowBatterySnooze { get; set; }
    public int AlarmBackupPowerLowBatteryCountInCycle { get; set; }
    public bool AlarmBackupPowerNotAliveEnable { get; set; }
    public int AlarmBackupPowerNotAliveFirstTime { get; set; }
    public int AlarmBackupPowerNotAliveEvery { get; set; }
    public int AlarmBackupPowerNotAliveSnooze { get; set; }
    public int AlarmBackupPowerNotAliveCountInCycle { get; set; }
    public bool AlarmBackupPowerNoPowerEnable { get; set; }
    public int AlarmBackupPowerNoPowerFirstTime { get; set; }
    public int AlarmBackupPowerNoPowerEvery { get; set; }
    public int AlarmBackupPowerNoPowerSnooze { get; set; }
    public int AlarmBackupPowerNoPowerCountInCycle { get; set; }
    #endregion

    #region Observer Settings
    public int ObserverCheckInterval { get; set; }
    public bool ObserveAlways { get; set; }
    #endregion
}