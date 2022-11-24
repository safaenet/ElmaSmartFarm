CREATE TABLE [dbo].[FarmCommuteSensors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [DeviceId] INT NOT NULL,
    [FarmId] INT NOT NULL,
    [Name] NVARCHAR(200) NULL,
    [Section] TINYINT NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [Descriptions] NVARCHAR(200) NULL
)