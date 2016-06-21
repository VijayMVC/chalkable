CREATE TYPE [dbo].[TStandardizedTestComponent] AS TABLE
(
	[Id] INT NOT NULL, 
	[StandardizedTestRef] INT NOT NULL,
    [Name] NVARCHAR(30) NOT NULL, 
	[DisplayOnTranscript] BIT NULL, 
	[Code] NVARCHAR(30) NOT NULL,
	[StateCode] NVARCHAR(30) NULL, 
    [SifCode] NVARCHAR(30) NULL, 
    [NcesCode] NVARCHAR(30) NULL
)
