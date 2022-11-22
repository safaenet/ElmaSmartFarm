CREATE TABLE [dbo].[AmbientLightValues]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[SensorId] INT NOT NULL,
    [PeriodId] INT NOT NULL,
    [ReadDate] DATETIME NOT NULL, 
    [SensorValue] TINYINT NOT NULL
)