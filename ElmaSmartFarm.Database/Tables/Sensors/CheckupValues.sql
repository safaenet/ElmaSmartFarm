﻿CREATE TABLE [dbo].[CheckupValues]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[SensorId] INT NOT NULL,
    [PeriodId] INT NOT NULL,
    [ReadDate] DATETIME NOT NULL
)