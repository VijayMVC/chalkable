CREATE TYPE [dbo].[TStandardizedTest] AS TABLE
(
	[Id] INT NOT NULL, 
    [Code] NVARCHAR(30) NOT NULL, 
    [Name] NVARCHAR(30) NOT NULL, 
    [DisplayName] NVARCHAR(30) NULL, 
    [Description] NVARCHAR(MAX) NULL,
    [GradeLevelRef] INT NULL, 
    [StateCode] NVARCHAR(30) NULL, 
    [DisplayOnTranscript] BIT NULL, 
    [SifCode] NVARCHAR(30) NULL, 
    [NcesCode] NVARCHAR(30) NULL
)
