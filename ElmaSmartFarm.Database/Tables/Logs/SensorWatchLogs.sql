CREATE TABLE [dbo].[SensorWatchLogs]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [SensorId] INT NOT NULL, 
    [LocationId] INT NOT NULL, 
    [Section] TINYINT NOT NULL DEFAULT 10,
    [Action] TINYINT NOT NULL, 
    [DateHappened] DATETIME NOT NULL, 
    [Descriptions] NVARCHAR(200) NULL
)