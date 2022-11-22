CREATE TABLE [dbo].[FoodStatistics]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [PeriodId] INT NOT NULL, 
    [FoodWeight] INT NOT NULL, 
    [FeedDate] DATETIME NOT NULL, 
    [DateRegistered] DATETIME NOT NULL
)