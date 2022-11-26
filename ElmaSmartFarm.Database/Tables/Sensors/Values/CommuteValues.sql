CREATE TABLE [dbo].[CommuteValues]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [LocationId] INT NOT NULL,
    [Section] TINYINT NOT NULL,
    [SensorId] INT NOT NULL,
    [StepType] TINYINT NOT NULL, 
    [StepDate] DATETIME NOT NULL
)