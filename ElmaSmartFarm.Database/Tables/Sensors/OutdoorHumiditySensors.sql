CREATE TABLE [dbo].[OutdoorHumiditySensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [PoultryId] INT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [OffsetValue] TINYINT NOT NULL DEFAULT 0, 
    [Descriptions] NVARCHAR(200) NULL
)