CREATE TABLE [dbo].[OutdoorTemperatureSensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [PoultryId] INT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [OffsetValue] DECIMAL(5, 2) NOT NULL DEFAULT 0, 
    [Descriptions] NVARCHAR(200) NULL
)