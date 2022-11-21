CREATE TABLE [dbo].[TemperatureSensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [EnvironmentType] INT NOT NULL DEFAULT 1,
    [EnvironmentId] INT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [OffsetValue] DECIMAL(5, 2) NOT NULL DEFAULT 0, 
    [Descriptions] NVARCHAR(50) NULL
)