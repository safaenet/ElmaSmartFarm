CREATE TABLE [dbo].[Humidities]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [SensorId] INT NOT NULL,
    [ReadDate] DATETIME NOT NULL, 
    [SensorValue] TINYINT NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'%',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Humidities',
    @level2type = N'COLUMN',
    @level2name = N'SensorValue'