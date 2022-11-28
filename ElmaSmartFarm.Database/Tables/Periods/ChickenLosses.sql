﻿CREATE TABLE [dbo].[ChickenLosses]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [PeriodId] INT NOT NULL, 
    [LossCount] INT NOT NULL, 
    [DateHappened] DATETIME NOT NULL, 
    [DateRegistered] DATETIME NOT NULL, 
    [UserId] INT NULL,
    [Descriptions] NVARCHAR(200) NULL
)