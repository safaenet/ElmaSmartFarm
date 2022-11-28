CREATE TABLE [dbo].[Contacts]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NULL, 
    [Username] NVARCHAR(50) NOT NULL, 
    [PasswordHash] VARBINARY(1024) NOT NULL, 
    [PasswordSalt] VARBINARY(1024) NOT NULL, 
    [PhoneNumber] VARCHAR(15) NULL, 
    [EmailAddress] NVARCHAR(200) NULL, 
    [Role] TINYINT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [LastLoginDate] DATETIME NULL, 
    [IsActive] BIT NOT NULL,
    [Descriptions] NVARCHAR(200) NULL 

    CONSTRAINT unique_Username UNIQUE(Username)
)