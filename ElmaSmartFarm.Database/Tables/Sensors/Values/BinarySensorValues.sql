CREATE TABLE [dbo].[BinarySensorValues]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[SensorId] INT NOT NULL,
	[Status] BIT NOT NULL,
    [ReadDate] DATETIME NOT NULL
)