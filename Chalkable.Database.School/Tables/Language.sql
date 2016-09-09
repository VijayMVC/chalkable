CREATE TABLE [dbo].[Language]
(
	[Id] INT NOT NULL, 
    [Code] NVARCHAR(64) NULL, 
    [Name] NVARCHAR(128) NULL, 
    [Description] NVARCHAR(512) NULL, 
    [StateCode] NVARCHAR(64) NULL, 
    [SIFCode] NVARCHAR(64) NULL, 
    [NCESCode] NVARCHAR(64) NULL, 
    [IsActive] BIT NOT NULL DEFAULT 0, 
    [IsSystem] BIT NOT NULL DEFAULT 0,
	CONSTRAINT PK_Language PRIMARY KEY(Id)
)
