CREATE TABLE [dbo].[Periods]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [PoultryId] INT NOT NULL, 
    [StartDate] DATETIME NOT NULL, 
    [EndDate] DATETIME NULL, 
    [Descriptions] NVARCHAR(200) NULL
)