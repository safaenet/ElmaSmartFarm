CREATE TABLE [dbo].[HumiditySensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FarmId] INT NOT NULL,
    [Section] TINYINT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [OffsetValue] TINYINT NOT NULL DEFAULT 0, 
    [Descriptions] NVARCHAR(200) NULL
)