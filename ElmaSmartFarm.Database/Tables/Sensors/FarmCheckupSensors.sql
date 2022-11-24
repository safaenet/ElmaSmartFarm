CREATE TABLE [dbo].[FarmCheckupSensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FarmId] INT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1,
    [Descriptions] NVARCHAR(200) NULL
)