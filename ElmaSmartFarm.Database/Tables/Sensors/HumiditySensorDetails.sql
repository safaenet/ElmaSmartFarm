CREATE TABLE [dbo].[HumiditySensorDetails]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[SensorId] INT NOT NULL,
    [OffsetValue] TINYINT NOT NULL DEFAULT 0
)