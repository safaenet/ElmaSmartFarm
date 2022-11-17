CREATE TABLE [dbo].[Temperatures]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ReadDate] DATETIME NOT NULL, 
    [SensorValue] DECIMAL(5, 2) NOT NULL
)