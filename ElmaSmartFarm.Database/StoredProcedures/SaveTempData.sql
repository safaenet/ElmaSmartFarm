CREATE PROCEDURE [dbo].[SaveTempData]
	@sensorId int,
	@sensorValue int,
    @readDate datetime
AS
	DECLARE @isEnabled bit; SET @isEnabled = (SELECT [IsEnabled] FROM [Sensors] WHERE [Id] = @sensorId);
            IF @isEnabled = 1
            BEGIN
                DECLARE @offset int; SET @offset = (SELECT [OffsetValue] FROM [Sensors] WHERE [Id] = @sensorId);
                DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [Temperatures]) + 1;
                SET @sensorValue = @sensorValue + @offset;
                INSERT INTO [Temperatures] ([Id], [SensorId], [ReadDate], [SensorValue]) VALUES (@newId, @sensorId, @readDate, @sensorValue);
            END
            ELSE RETURN 0;
RETURN 1