CREATE TABLE [dbo].[FarmCommuteSensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FarmId] INT NOT NULL,
    [Section] TINYINT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [Descriptions] NVARCHAR(200) NULL
)