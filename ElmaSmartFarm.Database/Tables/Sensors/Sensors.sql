CREATE TABLE [dbo].[Sensors]
(
    [Id] INT NOT NULL PRIMARY KEY,
    [Type] TINYINT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [KeepAliveInterval] SMALLINT NOT NULL DEFAULT 0,
    [Descriptions] NVARCHAR(200) NULL
)