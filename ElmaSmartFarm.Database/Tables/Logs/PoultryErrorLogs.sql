CREATE TABLE [dbo].[PoultryErrorLogs]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ErrorType] TINYINT NOT NULL,
    [DateHappened] DATETIME NOT NULL, 
    [DateErased] DATETIME NULL, 
    [Descriptions] NVARCHAR(200) NULL
)