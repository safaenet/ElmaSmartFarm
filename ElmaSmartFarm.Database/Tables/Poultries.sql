CREATE TABLE [dbo].[Poultries]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(200) NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [Descriptions] NVARCHAR(200) NULL
)