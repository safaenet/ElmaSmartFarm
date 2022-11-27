CREATE TABLE [dbo].[Sensors]
(
    [Id] INT NOT NULL PRIMARY KEY,
    [Type] TINYINT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [Descriptions] NVARCHAR(200) NULL
)