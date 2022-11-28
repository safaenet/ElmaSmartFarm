CREATE TABLE [dbo].[Locations]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(200) NULL,
    [Type] TINYINT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [Descriptions] NVARCHAR(200) NULL
)