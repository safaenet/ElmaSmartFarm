﻿CREATE TABLE [dbo].[Sensors]
(
    [Id] INT NOT NULL PRIMARY KEY,
    [LocationId] INT NOT NULL,
    [Type] TINYINT NOT NULL,
    [Name] NVARCHAR(200) NULL,
    [Section] TINYINT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [Descriptions] NVARCHAR(200) NULL
)