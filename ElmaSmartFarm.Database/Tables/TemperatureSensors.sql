CREATE TABLE [dbo].[TemperatureSensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [OffsetValue] DECIMAL(5, 2) NOT NULL DEFAULT 0, 
    [Description] NVARCHAR(50) NULL
)