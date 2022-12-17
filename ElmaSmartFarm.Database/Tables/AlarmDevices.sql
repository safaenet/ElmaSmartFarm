CREATE TABLE [dbo].[AlarmDevices]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(200) NULL,
    [Type] TINYINT NOT NULL, 
	[LocationId] INT NOT NULL, 
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [Descriptions] NVARCHAR(200) NULL
)