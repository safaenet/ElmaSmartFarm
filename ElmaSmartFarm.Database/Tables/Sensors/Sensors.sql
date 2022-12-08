CREATE TABLE [dbo].[Sensors]
(
    [Id] INT NOT NULL PRIMARY KEY,
    [Type] TINYINT NOT NULL,
    [Name] NVARCHAR(100) NULL,
    [LocationId] INT NOT NULL,
    [Section] TINYINT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [Descriptions] NVARCHAR(1000) NULL
)