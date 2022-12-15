namespace ElmaSmartFarm.SharedLibrary.Config;

public class Config
{
    public Config()
    {
        DefaultConnectionString = SettingsDataAccess.AppConfiguration().GetSection("ConnectionStrings:default").Value;
        BaseUrl = SettingsDataAccess.AppConfiguration().GetSection("BaseUrl").Value;
        PoultryName = SettingsDataAccess.AppConfiguration().GetSection("verbose_mode").Value ?? "Elma Smart Poultry";
        VerboseMode = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("verbose_mode").Value ?? "true");
        mqtt = new();
        system = new();
    }
    public string DefaultConnectionString { get; set; }
    public MQTT mqtt { get; init; }
    public System system { get; set; }
    public string BaseUrl { get; init; }
    public string PoultryName { get; set; }
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

        AlarmLevelLowBattery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:low_battery").Value ?? "1");
        AlarmLevelNotAlive = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:not_alive").Value ?? "2");
        AlarmLevelInvalidData = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:invalid_data").Value ?? "1");
        AlarmLevelInvalidValue = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:invalid_value").Value ?? "1");
        AlarmLevelHighTemperature = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:high_temp").Value ?? "3");
        AlarmLevelLowTemperature = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:low_temp").Value ?? "3");
        AlarmLevelHighHumid = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:high_humid").Value ?? "2");
        AlarmLevelLowHumid = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:low_humid").Value ?? "2");
        AlarmLevelHighAmmonia = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:high_ammonia").Value ?? "3");
        AlarmLevelHighCo2 = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:high_co2").Value ?? "3");
        AlarmLevelLongTimeBright = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:long_time_bright").Value ?? "2");
        AlarmLevelLongTimeDark = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:long_time_dark").Value ?? "2");
        AlarmLevelHighBrightness = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:high_brightness").Value ?? "2");
        AlarmLevelLowBrightness = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:low_brightness").Value ?? "2");
        AlarmLevelLongNoFeed = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:long_no_feed").Value ?? "3");
        AlarmLevelLongLeave = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:long_leave").Value ?? "3");
        AlarmLevelNoPower = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level:no_power").Value ?? "4");

        FarmAlarmDuration = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:farm_alarm_duration").Value ?? "20");
        PoultryAlarmDuration = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:poultry_alarm_duration").Value ?? "20");

        AlarmLevelOneEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:enable").Value ?? "true");
        AlarmLevelOneRaiseTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:raise_time").Value ?? "60");
        AlarmLevelOneFarmAlarmEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:farm_alarm:enable").Value ?? "true");
        AlarmLevelOneFarmAlarmRaiseTimeOffset = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:farm_alarm:raise_time_offset").Value ?? "300");
        AlarmLevelOneFarmAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:farm_alarm:every").Value ?? "300");
        AlarmLevelOneFarmAlarmSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:farm_alarm:snooze").Value ?? "3000");
        AlarmLevelOneFarmAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:farm_alarm:count_in_cycle").Value ?? "3");
        AlarmLevelOneSmsEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:sms:enable").Value ?? "true");
        AlarmLevelOneSmsRaiseTimeOffset = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:sms:raise_time_offset").Value ?? "300");
        AlarmLevelOneSmsEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:sms:every").Value ?? "300");
        AlarmLevelOneSmsSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:sms:snooze").Value ?? "3000");
        AlarmLevelOneSmsCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:sms:count_in_cycle").Value ?? "3");
        AlarmLevelOnePoultryAlarmEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:poultry_alarm:enable").Value ?? "false");
        AlarmLevelOnePoultryAlarmRaiseTimeOffset = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:poultry_alarm:raise_time_offset").Value ?? "300");
        AlarmLevelOnePoultryAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:poultry_alarm:every").Value ?? "300");
        AlarmLevelOnePoultryAlarmSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:poultry_alarm:snooze").Value ?? "3000");
        AlarmLevelOnePoultryAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:1:poultry_alarm:count_in_cycle").Value ?? "3");

        AlarmLevelTwoEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:enable").Value ?? "true");
        AlarmLevelTwoRaiseTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:raise_time").Value ?? "60");
        AlarmLevelTwoFarmAlarmEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:farm_alarm:enable").Value ?? "true");
        AlarmLevelTwoFarmAlarmRaiseTimeOffset = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:farm_alarm:raise_time_offset").Value ?? "300");
        AlarmLevelTwoFarmAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:farm_alarm:every").Value ?? "300");
        AlarmLevelTwoFarmAlarmSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:farm_alarm:snooze").Value ?? "3000");
        AlarmLevelTwoFarmAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:farm_alarm:count_in_cycle").Value ?? "3");
        AlarmLevelTwoSmsEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:sms:enable").Value ?? "true");
        AlarmLevelTwoSmsRaiseTimeOffset = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:sms:raise_time_offset").Value ?? "300");
        AlarmLevelTwoSmsEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:sms:every").Value ?? "300");
        AlarmLevelTwoSmsSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:sms:snooze").Value ?? "3000");
        AlarmLevelTwoSmsCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:sms:count_in_cycle").Value ?? "3");
        AlarmLevelTwoPoultryAlarmEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:poultry_alarm:enable").Value ?? "false");
        AlarmLevelTwoPoultryAlarmRaiseTimeOffset = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:poultry_alarm:raise_time_offset").Value ?? "300");
        AlarmLevelTwoPoultryAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:poultry_alarm:every").Value ?? "300");
        AlarmLevelTwoPoultryAlarmSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:poultry_alarm:snooze").Value ?? "3000");
        AlarmLevelTwoPoultryAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:2:poultry_alarm:count_in_cycle").Value ?? "3");

        AlarmLevelThreeEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:enable").Value ?? "true");
        AlarmLevelThreeRaiseTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:raise_time").Value ?? "60");
        AlarmLevelThreeFarmAlarmEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:farm_alarm:enable").Value ?? "true");
        AlarmLevelThreeFarmAlarmRaiseTimeOffset = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:farm_alarm:raise_time_offset").Value ?? "300");
        AlarmLevelThreeFarmAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:farm_alarm:every").Value ?? "300");
        AlarmLevelThreeFarmAlarmSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:farm_alarm:snooze").Value ?? "3000");
        AlarmLevelThreeFarmAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:farm_alarm:count_in_cycle").Value ?? "3");
        AlarmLevelThreeSmsEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:sms:enable").Value ?? "true");
        AlarmLevelThreeSmsRaiseTimeOffset = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:sms:raise_time_offset").Value ?? "300");
        AlarmLevelThreeSmsEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:sms:every").Value ?? "300");
        AlarmLevelThreeSmsSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:sms:snooze").Value ?? "3000");
        AlarmLevelThreeSmsCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:sms:count_in_cycle").Value ?? "3");
        AlarmLevelThreePoultryAlarmEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:poultry_alarm:enable").Value ?? "true");
        AlarmLevelThreePoultryAlarmRaiseTimeOffset = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:poultry_alarm:raise_time_offset").Value ?? "300");
        AlarmLevelThreePoultryAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:poultry_alarm:every").Value ?? "300");
        AlarmLevelThreePoultryAlarmSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:poultry_alarm:snooze").Value ?? "3000");
        AlarmLevelThreePoultryAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:3:poultry_alarm:count_in_cycle").Value ?? "3");

        AlarmLevelFourEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:enable").Value ?? "true");
        AlarmLevelFourRaiseTime = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:raise_time").Value ?? "60");
        AlarmLevelFourFarmAlarmEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:farm_alarm:enable").Value ?? "true");
        AlarmLevelFourFarmAlarmRaiseTimeOffset = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:farm_alarm:raise_time_offset").Value ?? "300");
        AlarmLevelFourFarmAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:farm_alarm:every").Value ?? "300");
        AlarmLevelFourFarmAlarmSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:farm_alarm:snooze").Value ?? "3000");
        AlarmLevelFourFarmAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:farm_alarm:count_in_cycle").Value ?? "3");
        AlarmLevelFourSmsEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:sms:enable").Value ?? "true");
        AlarmLevelFourSmsRaiseTimeOffset = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:sms:raise_time_offset").Value ?? "300");
        AlarmLevelFourSmsEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:sms:every").Value ?? "300");
        AlarmLevelFourSmsSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:sms:snooze").Value ?? "3000");
        AlarmLevelFourSmsCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:sms:count_in_cycle").Value ?? "3");
        AlarmLevelFourPoultryAlarmEnable = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:poultry_alarm:enable").Value ?? "true");
        AlarmLevelFourPoultryAlarmRaiseTimeOffset = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:poultry_alarm:raise_time_offset").Value ?? "300");
        AlarmLevelFourPoultryAlarmEvery = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:poultry_alarm:every").Value ?? "300");
        AlarmLevelFourPoultryAlarmSnooze = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:poultry_alarm:snooze").Value ?? "3000");
        AlarmLevelFourPoultryAlarmCountInCycle = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("alarm_level_timeout:4:poultry_alarm:count_in_cycle").Value ?? "3");

        ScalarReadInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("scalar_general:read_interval").Value ?? "30");
        WriteScalarToDbInterval = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("scalar_general:write_to_db_interval").Value ?? "30");
        WriteScalarToDbAlways = bool.Parse(SettingsDataAccess.AppConfiguration().GetSection("scalar_general:write_to_db_always").Value ?? "false");
        ScalarNotAliveWatchTimeout= int.Parse(SettingsDataAccess.AppConfiguration().GetSection("scalar_general:not_alive_watch_timeout").Value ?? "300");

        FarmTempMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:farm_min_value").Value ?? "20");
        FarmTempMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:farm_max_value").Value ?? "35");
        OutdoorTempMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:outdoor_min_value").Value ?? "-10");
        OutdoorTempMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:outdoor_max_value").Value ?? "60");
        TempMinWorkingValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:min_working_value").Value ?? "28");
        TempMaxWorkingValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:max_working_value").Value ?? "33");
        TempInvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:invalid_data_watch_timeout").Value ?? "300");
        TempInvalidValueWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("temperature:invalid_value_watch_timeout").Value ?? "300");


        HumidMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:min_value").Value ?? "20");
        HumidMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:max_value").Value ?? "35");
        HumidMinWorkingValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:min_working_value").Value ?? "30");
        HumidMaxWorkingValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:max_working_value").Value ?? "60");
        HumidInvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:invalid_data_watch_timeout").Value ?? "300");
        HumidInvalidValueWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("humidity:invalid_value_watch_timeout").Value ?? "300");

        AmbientLightMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:min_value").Value ?? "20");
        AmbientLightMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:max_value").Value ?? "35");
        AmbientLightInvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:invalid_data_watch_timeout").Value ?? "300");
        AmbientLightInvalidValueWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ambient_light:invalid_value_watch_timeout").Value ?? "300");

        AmmoniaMinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:min_value").Value ?? "0");
        AmmoniaMaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:max_value").Value ?? "100");
        AmmoniaMaxWorkingValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:max_working_value").Value ?? "600");
        AmmoniaInvalidDataWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:invalid_data_watch_timeout").Value ?? "300");
        AmmoniaInvalidValueWatchTimeout = int.Parse(SettingsDataAccess.AppConfiguration().GetSection("ammonia:invalid_value_watch_timeout").Value ?? "300");

        Co2MinValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:min_value").Value ?? "0");
        Co2MaxValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:max_value").Value ?? "100");
        Co2MaxWorkingValue = double.Parse(SettingsDataAccess.AppConfiguration().GetSection("co2:max_working_value").Value ?? "600");
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
    public int AlarmLevelOneRaiseTime { get; set; }
    public bool AlarmLevelOneFarmAlarmEnable { get; set; }
    public int AlarmLevelOneFarmAlarmRaiseTimeOffset { get; set; }
    public int AlarmLevelOneFarmAlarmEvery { get; set; }
    public int AlarmLevelOneFarmAlarmSnooze { get; set; }
    public int AlarmLevelOneFarmAlarmCountInCycle { get; set; }
    public bool AlarmLevelOneSmsEnable { get; set; }
    public int AlarmLevelOneSmsRaiseTimeOffset { get; set; }
    public int AlarmLevelOneSmsEvery { get; set; }
    public int AlarmLevelOneSmsSnooze { get; set; }
    public int AlarmLevelOneSmsCountInCycle { get; set; }
    public bool AlarmLevelOnePoultryAlarmEnable { get; set; }
    public int AlarmLevelOnePoultryAlarmRaiseTimeOffset { get; set; }
    public int AlarmLevelOnePoultryAlarmEvery { get; set; }
    public int AlarmLevelOnePoultryAlarmSnooze { get; set; }
    public int AlarmLevelOnePoultryAlarmCountInCycle { get; set; }

    public bool AlarmLevelTwoEnable { get; set; }
    public int AlarmLevelTwoRaiseTime { get; set; }
    public bool AlarmLevelTwoFarmAlarmEnable { get; set; }
    public int AlarmLevelTwoFarmAlarmRaiseTimeOffset { get; set; }
    public int AlarmLevelTwoFarmAlarmEvery { get; set; }
    public int AlarmLevelTwoFarmAlarmSnooze { get; set; }
    public int AlarmLevelTwoFarmAlarmCountInCycle { get; set; }
    public bool AlarmLevelTwoSmsEnable { get; set; }
    public int AlarmLevelTwoSmsRaiseTimeOffset { get; set; }
    public int AlarmLevelTwoSmsEvery { get; set; }
    public int AlarmLevelTwoSmsSnooze { get; set; }
    public int AlarmLevelTwoSmsCountInCycle { get; set; }
    public bool AlarmLevelTwoPoultryAlarmEnable { get; set; }
    public int AlarmLevelTwoPoultryAlarmRaiseTimeOffset { get; set; }
    public int AlarmLevelTwoPoultryAlarmEvery { get; set; }
    public int AlarmLevelTwoPoultryAlarmSnooze { get; set; }
    public int AlarmLevelTwoPoultryAlarmCountInCycle { get; set; }

    public bool AlarmLevelThreeEnable { get; set; }
    public int AlarmLevelThreeRaiseTime { get; set; }
    public bool AlarmLevelThreeFarmAlarmEnable { get; set; }
    public int AlarmLevelThreeFarmAlarmRaiseTimeOffset { get; set; }
    public int AlarmLevelThreeFarmAlarmEvery { get; set; }
    public int AlarmLevelThreeFarmAlarmSnooze { get; set; }
    public int AlarmLevelThreeFarmAlarmCountInCycle { get; set; }
    public bool AlarmLevelThreeSmsEnable { get; set; }
    public int AlarmLevelThreeSmsRaiseTimeOffset { get; set; }
    public int AlarmLevelThreeSmsEvery { get; set; }
    public int AlarmLevelThreeSmsSnooze { get; set; }
    public int AlarmLevelThreeSmsCountInCycle { get; set; }
    public bool AlarmLevelThreePoultryAlarmEnable { get; set; }
    public int AlarmLevelThreePoultryAlarmRaiseTimeOffset { get; set; }
    public int AlarmLevelThreePoultryAlarmEvery { get; set; }
    public int AlarmLevelThreePoultryAlarmSnooze { get; set; }
    public int AlarmLevelThreePoultryAlarmCountInCycle { get; set; }

    public bool AlarmLevelFourEnable { get; set; }
    public int AlarmLevelFourRaiseTime { get; set; }
    public bool AlarmLevelFourFarmAlarmEnable { get; set; }
    public int AlarmLevelFourFarmAlarmRaiseTimeOffset { get; set; }
    public int AlarmLevelFourFarmAlarmEvery { get; set; }
    public int AlarmLevelFourFarmAlarmSnooze { get; set; }
    public int AlarmLevelFourFarmAlarmCountInCycle { get; set; }
    public bool AlarmLevelFourSmsEnable { get; set; }
    public int AlarmLevelFourSmsRaiseTimeOffset { get; set; }
    public int AlarmLevelFourSmsEvery { get; set; }
    public int AlarmLevelFourSmsSnooze { get; set; }
    public int AlarmLevelFourSmsCountInCycle { get; set; }
    public bool AlarmLevelFourPoultryAlarmEnable { get; set; }
    public int AlarmLevelFourPoultryAlarmRaiseTimeOffset { get; set; }
    public int AlarmLevelFourPoultryAlarmEvery { get; set; }
    public int AlarmLevelFourPoultryAlarmSnooze { get; set; }
    public int AlarmLevelFourPoultryAlarmCountInCycle { get; set; }

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
    public double TempMinWorkingValue { get; set; }
    public double TempMaxWorkingValue { get; set; }
    public int TempInvalidDataWatchTimeout { get; set; }
    public int TempInvalidValueWatchTimeout { get; set; }
    #endregion

    #region Humidity Settings
    public double HumidMinValue { get; set; }
    public double HumidMaxValue { get; set; }
    public double HumidMinWorkingValue { get; set; }
    public double HumidMaxWorkingValue { get; set; }
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
    public double AmmoniaMaxWorkingValue { get; set; }
    public int AmmoniaInvalidDataWatchTimeout { get; set; }
    public int AmmoniaInvalidValueWatchTimeout { get; set; }
    #endregion

    #region Co2 Settings
    public double Co2MinValue { get; set; }
    public double Co2MaxValue { get; set; }
    public double Co2MaxWorkingValue { get; set; }
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