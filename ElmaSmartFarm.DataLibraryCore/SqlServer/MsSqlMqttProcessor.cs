using Dapper;
using ElmaSmartFarm.DataLibraryCore.Interfaces;
using ElmaSmartFarm.SharedLibrary;
using ElmaSmartFarm.SharedLibrary.DALModels;
using Microsoft.Extensions.Configuration;
using SharedLibrary.DALModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.SqlServer
{
    public class MsSqlMqttProcessor
    {
        public MsSqlMqttProcessor(IDataAccess dataAccess)
        {
            DataAccess = dataAccess;
            sensor_topic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:sensor_topic").Value ?? "Elma/ToServer/Sensors";
            temperature_sub_topic = SettingsDataAccess.AppConfiguration().GetSection("mqtt:temperature_sub_topic").Value ?? "/Temp";
        }

        private readonly IDataAccess DataAccess;
        private string sensor_topic;
        private string temperature_sub_topic;
        private readonly string SaveTempSensorData = @"DECLARE @isEnabled bit; SET @isEnabled = (SELECT [IsEnabled] FROM [Sensors] WHERE [Id] = @sensorId);
            IF @isEnabled = 1 BEGIN
                DECLARE @offset int; SET @offset = (SELECT [OffsetValue] FROM [Sensors] WHERE [Id] = @sensorId);
                DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [Temperatures]) + 1;
                SET @sensorValue = @sensorValue + @offset;
                INSERT INTO [Temperatures] ([Id], [SensorId], [ReadDate], [SensorValue]) VALUES (@newId, @sensorId, @readDate, @sensorValue);
            END";

        public async Task<int> ProcessMqttMessageAsync(MqttMessageModel mqtt)
        {
            if (mqtt == null) return -1;
            if (mqtt.Topic == sensor_topic + temperature_sub_topic)
            {
                DynamicParameters dp = new();
                var sensorId = mqtt.ClientId.Split('-')[1];
                dp.Add("@sensorId", sensorId);
                dp.Add("@readDate", mqtt.ReadDate);
                dp.Add("@sensorValue", mqtt.Payload);
                return await DataAccess.SaveDataAsync(SaveTempSensorData, dp);
            }
            return 0;
        }
    }
}