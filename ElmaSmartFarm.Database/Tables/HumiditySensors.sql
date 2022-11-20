CREATE TABLE [dbo].[HumiditySensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [OffsetValue] TINYINT NOT NULL DEFAULT 0, 
    [Description] NVARCHAR(50) NULL
)