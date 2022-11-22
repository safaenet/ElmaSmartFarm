CREATE TABLE [dbo].[FarmsInPeriods]
(
    [FarmId] INT NOT NULL, 
    [PeriodId] INT NOT NULL, 
    [ChickenCount] INT NOT NULL, 
    CONSTRAINT [PK_FarmsInPeriods] PRIMARY KEY ([FarmId], [PeriodId])
)