CREATE TABLE [dbo].[FeedSensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [EnvironmentId] INT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1,
    [Descriptions] NVARCHAR(50) NULL
)
