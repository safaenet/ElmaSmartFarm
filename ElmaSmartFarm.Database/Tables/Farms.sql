CREATE TABLE [dbo].[Farms]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [PoultryId] INT NOT NULL,
    [FarmNumber] TINYINT NULL ,
    [MaxCapacity] INT NULL 
)