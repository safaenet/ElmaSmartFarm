CREATE TABLE [dbo].[BinarySensorValues]
(
	[Id] INT NOT NULL PRIMARY KEY,
    [LocationId] INT NOT NULL,
    [Section] TINYINT NOT NULL,
	[SensorId] INT NOT NULL,
	[Status] BIT NOT NULL,
    [ReadDate] DATETIME NOT NULL
)