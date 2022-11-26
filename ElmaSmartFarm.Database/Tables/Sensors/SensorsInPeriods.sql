﻿CREATE TABLE [dbo].[SensorsInPeriods]
(
	[Id] INT NOT NULL PRIMARY KEY,
    [PeriodId] INT NOT NULL, 
    [LocationId] INT NOT NULL,
    [Section] TINYINT NOT NULL,
    [WatchStartDay] TINYINT NOT NULL
)