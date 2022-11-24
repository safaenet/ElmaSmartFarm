CREATE TABLE [dbo].[FarmTemperatureSensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [DeviceId] INT NOT NULL,
    [FarmId] INT NOT NULL,
    [Name] NVARCHAR(200) NULL,
    [Section] TINYINT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [OffsetValue] DECIMAL(5, 2) NOT NULL DEFAULT 0, 
    [Descriptions] NVARCHAR(200) NULL
)