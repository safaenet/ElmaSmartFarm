CREATE TABLE [dbo].[FarmHumidityValues]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [SensorId] INT NOT NULL,
    [ReadDate] DATETIME NOT NULL, 
    [SensorValue] TINYINT NOT NULL
)