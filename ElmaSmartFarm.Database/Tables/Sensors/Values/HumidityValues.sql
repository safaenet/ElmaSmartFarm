CREATE TABLE [dbo].[HumidityValues]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [LocationId] INT NOT NULL,
    [Section] TINYINT NOT NULL,
    [SensorId] INT NOT NULL,
    [ReadDate] DATETIME NOT NULL,
    [ReadValue] TINYINT NOT NULL
)