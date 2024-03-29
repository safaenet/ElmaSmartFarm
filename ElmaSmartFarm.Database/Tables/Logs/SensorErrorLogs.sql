﻿CREATE TABLE [dbo].[SensorErrorLogs]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [SensorId] INT NOT NULL, 
    [LocationId] INT NOT NULL,
    [Section] TINYINT NOT NULL,
    [ErrorType] TINYINT NOT NULL,
    [DateHappened] DATETIME NOT NULL, 
    [DateErased] DATETIME NULL, 
    [Descriptions] NVARCHAR(200) NULL
)