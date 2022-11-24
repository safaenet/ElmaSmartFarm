CREATE TABLE [dbo].[Periods]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(200) NULL,
    [FarmId] INT NOT NULL, 
    [StartDate] DATETIME NOT NULL, 
    [EndDate] DATETIME NULL, 
    [UserId] INT NULL,
    [Descriptions] NVARCHAR(200) NULL
)