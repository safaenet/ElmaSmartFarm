CREATE TABLE [dbo].[AmbientLightSensorDetails]
(
	[SensorId] INT NOT NULL,
    [OffsetValue] TINYINT NOT NULL DEFAULT 0, 
    PRIMARY KEY ([SensorId])
)