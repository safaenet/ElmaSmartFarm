CREATE TABLE [dbo].[Farms]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(200) NULL,
    [FarmNumber] TINYINT NULL ,
    [MaxCapacity] INT NULL ,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [Descriptions] NVARCHAR(1000) NULL
)