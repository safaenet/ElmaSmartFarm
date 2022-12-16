CREATE TABLE [dbo].[FarmInPeriodErrorLogs]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FarmId] INT NOT NULL, 
    [PeriodId] INT NOT NULL, 
    [ErrorType] TINYINT NOT NULL,
    [DateHappened] DATETIME NOT NULL, 
    [DateErased] DATETIME NULL, 
    [CausedSensorId] INT NOT NULL, 
    [Descriptions] NVARCHAR(200) NULL
)