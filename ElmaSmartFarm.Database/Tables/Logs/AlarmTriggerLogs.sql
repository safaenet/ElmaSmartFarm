CREATE TABLE [dbo].[AlarmTriggerLogs]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [AlarmId] INT NOT NULL, 
    [LocationId] INT NOT NULL, 
    [Action] TINYINT NOT NULL, 
    [DateHappened] DATETIME NOT NULL, 
    [Ack] BIT NULL 
)