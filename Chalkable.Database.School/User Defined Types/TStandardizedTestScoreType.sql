CREATE TYPE [dbo].[TStandardizedTestScoreType] AS TABLE
(
	[Id] INT NOT NULL, 
	[StandardizedTestRef] INT NOT NULL,
    [Name] NVARCHAR(30) NOT NULL, 
	[Description] NVARCHAR(MAX) NULL,
	[Code] NVARCHAR(30) NOT NULL,
	[StateCode] NVARCHAR(30) NULL, 
    [SifCode] NVARCHAR(30) NULL, 
    [NcesCode] NVARCHAR(30) NULL
)
