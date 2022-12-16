﻿CREATE TABLE [dbo].[FarmErrorLogs]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FarmId] INT NOT NULL, 
    [PeriodId] INT NULL, 
    [ErrorType] TINYINT NOT NULL,
    [DateHappened] DATETIME NOT NULL, 
    [DateErased] DATETIME NULL, 
    [CausedSensorId] INT NOT NULL, 
    [Descriptions] NVARCHAR(200) NULL
)