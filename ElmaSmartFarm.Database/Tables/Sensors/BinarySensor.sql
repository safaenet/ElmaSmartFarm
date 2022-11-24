﻿CREATE TABLE [dbo].[BinarySensor]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Type] TINYINT NOT NULL,
    [LocationId] INT NOT NULL,
    [Name] NVARCHAR(200) NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1,
    [Descriptions] NVARCHAR(200) NULL
)