CREATE TABLE [dbo].[TemperatureSensorDetails]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[SensorId] INT NOT NULL,
    [OffsetValue] DECIMAL(5, 2) NOT NULL DEFAULT 0
)