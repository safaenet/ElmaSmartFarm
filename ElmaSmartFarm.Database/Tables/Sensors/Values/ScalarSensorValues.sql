CREATE TABLE [dbo].[ScalarSensorValues]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [LocationId] INT NOT NULL,
    [Section] TINYINT NOT NULL,
    [SensorId] INT NOT NULL,
    [ReadDate] DATETIME NOT NULL, 
    [Temperature] DECIMAL(5, 2) NULL,
    [Humidity] TINYINT NULL,
    [Light] TINYINT NULL, 
    [Ammonia] DECIMAL(5, 2) NULL,
    [Co2] DECIMAL(5, 2) NULL
)