using Dapper;
using ElmaSmartFarm.DataLibraryCore.Interfaces;
using SharedLibrary.DALModels;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.SqlServer
{
    public class MsSqlDbProcessor : IDbProcessor
    {
        public MsSqlDbProcessor(IDataAccess dataAccess)
        {
            DataAccess = dataAccess;
        }

        private readonly IDataAccess DataAccess;
        private readonly string SaveTempSensorData = @"
            DECLARE @isEnabled bit; SET @isEnabled = (SELECT [IsEnabled] FROM [TemperatureSensors] WHERE [Id] = @sensorId);
            IF ISNULL(@isEnabled, 0) = 1 BEGIN
                DECLARE @offset decimal(5, 2); SET @offset = (SELECT [OffsetValue] FROM [TemperatureSensors] WHERE [Id] = @sensorId);
                DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [TemperatureValues]) + 1;
                SET @sensorValue = @sensorValue + @offset;
                INSERT INTO [TemperatureValues] ([Id], [SensorId], [ReadDate], [SensorValue]) VALUES (@newId, @sensorId, @readDate, @sensorValue);
            END";

        public async Task<int> SaveTemperatureToDb(TemperatureModel temp)
        {
            DynamicParameters dp = new();
            dp.Add("@sensorId", temp.SensorId);
            dp.Add("@readDate", temp.ReadDate);
            dp.Add("@sensorValue", temp.Celsius);
            return await DataAccess.SaveDataAsync(SaveTempSensorData, dp);
        }
    }
}