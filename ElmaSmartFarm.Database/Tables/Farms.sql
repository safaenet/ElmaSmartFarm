CREATE TABLE [dbo].[Farms]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [PoultryId] INT NOT NULL,
    [FarmNumber] INT NULL ,
    [MaxCapacity] INT NULL ,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [Descriptions] NVARCHAR(200) NULL
)