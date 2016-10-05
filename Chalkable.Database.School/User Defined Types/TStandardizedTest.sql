CREATE TYPE [dbo].[TStandardizedTest] AS TABLE
(
	[Id] INT NOT NULL, 
    [Code] NVARCHAR(64) NOT NULL, 
    [Name] NVARCHAR(64) NOT NULL, 
    [DisplayName] NVARCHAR(64) NULL, 
    [Description] NVARCHAR(MAX) NULL,
    [GradeLevelRef] INT NULL, 
    [StateCode] NVARCHAR(64) NULL, 
    [DisplayOnTranscript] BIT NULL, 
    [SifCode] NVARCHAR(64) NULL, 
    [NcesCode] NVARCHAR(64) NULL
)
