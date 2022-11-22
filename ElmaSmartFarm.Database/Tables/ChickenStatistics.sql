CREATE TABLE [dbo].[ChickenStatistics]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [PeriodId] INT NOT NULL, 
    [FarmId] INT NOT NULL, 
    [DeadCount] INT NOT NULL, 
    [DateHappened] DATETIME NOT NULL, 
    [DateRegistered] DATETIME NOT NULL
)