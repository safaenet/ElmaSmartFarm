CREATE TABLE [dbo].[ScalarSensorDetails]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [HasTemperature] BIT NOT NULL,
    [HasHumidity] BIT NOT NULL,
    [HasLight] BIT NOT NULL,
    [HasAmmonia] BIT NOT NULL,
    [HasCo2] BIT NOT NULL,
    [TemperatureOffSet] DECIMAL(5, 2) NOT NULL DEFAULT 0,
    [HumidityOffset] TINYINT NOT NULL DEFAULT 0,
    [LightOffset] TINYINT NOT NULL DEFAULT 0,
    [AmmoniaOffSet] DECIMAL(5, 2) NOT NULL DEFAULT 0,
    [Co2OffSet] DECIMAL(5, 2) NOT NULL DEFAULT 0
)