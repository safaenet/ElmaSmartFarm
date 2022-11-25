CREATE TABLE [dbo].[CommuteValues]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [SensorId] INT NOT NULL,
    [StepType] TINYINT NOT NULL, 
    [StepDate] DATETIME NOT NULL
)