CREATE TABLE [dbo].[FarmCommuteValues]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [SensorId] INT NOT NULL,
    [IsStepIn] BIT NOT NULL, 
    [StepDate] DATETIME NOT NULL
)
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'if true: step in; if false: step out.',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CommuteValues',
    @level2type = N'COLUMN',
    @level2name = N'IsStepIn'