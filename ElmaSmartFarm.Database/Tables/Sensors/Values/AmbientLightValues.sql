CREATE TABLE [dbo].[AmbientLightValues]
(
	[Id] INT NOT NULL PRIMARY KEY,
    [LocationId] INT NOT NULL,
    [Section] TINYINT NOT NULL,
	[SensorId] INT NOT NULL,
    [ReadDate] DATETIME NOT NULL, 
    [SensorValue] TINYINT NOT NULL
)