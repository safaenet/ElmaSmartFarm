CREATE TABLE [dbo].[FarmFeedSensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FarmId] INT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1,
    [Descriptions] NVARCHAR(50) NULL
)
