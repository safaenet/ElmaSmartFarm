CREATE TABLE [dbo].[CheckupSensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [EnvironmentType] INT NOT NULL DEFAULT 1,
    [EnvironmentId] INT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1,
    [Descriptions] NVARCHAR(50) NULL
)