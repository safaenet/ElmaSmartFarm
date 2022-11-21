CREATE TABLE [dbo].[Farms]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [Descriptions] NVARCHAR(50) NULL
)