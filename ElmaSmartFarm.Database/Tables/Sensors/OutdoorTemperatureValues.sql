CREATE TABLE [dbo].[OutdoorTemperatureValues]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [SensorId] INT NOT NULL,
    [ReadDate] DATETIME NOT NULL, 
    [SensorValue] DECIMAL(5, 2) NOT NULL
)