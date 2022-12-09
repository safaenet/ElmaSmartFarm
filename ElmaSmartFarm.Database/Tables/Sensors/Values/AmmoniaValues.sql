CREATE TABLE [dbo].[AmmoniaValues]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [LocationId] INT NOT NULL,
    [Section] TINYINT NOT NULL,
    [SensorId] INT NOT NULL,
    [ReadDate] DATETIME NOT NULL,
    [ReadValue] DECIMAL(5, 2) NOT NULL
)