CREATE TABLE [dbo].[SensorDetails]
(
	[Id] INT NOT NULL PRIMARY KEY,
    [SensorId] INT NOT NULL,
    [Name] NVARCHAR(200) NULL,
    [LocationId] INT NOT NULL,
    [Section] TINYINT NOT NULL
)