CREATE TABLE [dbo].[Sensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [SensorTypeId] SMALLINT NOT NULL, 
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [OffsetValue] INT NOT NULL DEFAULT 0
)