﻿CREATE TABLE [dbo].[FarmHumiditySensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FarmId] INT NOT NULL,
    [Name] NVARCHAR(200) NULL,
    [Section] TINYINT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [OffsetValue] TINYINT NOT NULL DEFAULT 0, 
    [Descriptions] NVARCHAR(200) NULL
)