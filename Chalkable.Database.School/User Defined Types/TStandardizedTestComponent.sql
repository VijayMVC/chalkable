CREATE TYPE [dbo].[TStandardizedTestComponent] AS TABLE
(
	[Id] INT NOT NULL, 
	[StandardizedTestRef] INT NOT NULL,
    [Name] NVARCHAR(64) NOT NULL, 
	[DisplayOnTranscript] BIT NULL, 
	[Code] NVARCHAR(64) NOT NULL,
	[StateCode] NVARCHAR(64) NULL, 
    [SifCode] NVARCHAR(64) NULL, 
    [NcesCode] NVARCHAR(64) NULL
)
