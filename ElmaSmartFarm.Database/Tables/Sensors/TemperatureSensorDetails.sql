CREATE TABLE [dbo].[TemperatureSensorDetails]
(
	[SensorId] INT NOT NULL,
    [OffsetValue] DECIMAL(5, 2) NOT NULL DEFAULT 0, 
    PRIMARY KEY ([SensorId])
)