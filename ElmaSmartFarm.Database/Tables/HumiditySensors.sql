﻿CREATE TABLE [dbo].[HumiditySensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [EnvironmentType] INT NOT NULL DEFAULT 1,
    [EnvironmentId] INT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [OffsetValue] TINYINT NOT NULL DEFAULT 0, 
    [Descriptions] NVARCHAR(50) NULL
)