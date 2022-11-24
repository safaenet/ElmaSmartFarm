CREATE TABLE [dbo].[OutdoorTemperatureSensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [DeviceId] INT NOT NULL,
    [PoultryId] INT NOT NULL,
    [Name] NVARCHAR(200) NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [OffsetValue] DECIMAL(5, 2) NOT NULL DEFAULT 0, 
    [Descriptions] NVARCHAR(200) NULL
)