CREATE TABLE [dbo].[SchoolProgram]
(
	[Id] INT  NOT NULL CONSTRAINT [CPK_SchoolProgram] PRIMARY KEY CLUSTERED ( [Id] ),
    [Code] VARCHAR(5) NOT NULL,
    [Name] VARCHAR(50) NOT NULL,
    [Description] VARCHAR(255) NOT NULL CONSTRAINT [DF_SchoolProgram_Description] DEFAULT (''),
    [StateCode] VARCHAR(10) NOT NULL CONSTRAINT [DF_SchoolProgram_StateCode] DEFAULT(''),
    [SIFCode] VARCHAR(10) NOT NULL CONSTRAINT [DF_SchoolProgram_SIFCode] DEFAULT(''),
    [NCESCode] VARCHAR(10) NOT NULL CONSTRAINT [DF_SchoolProgram_NCESCode] DEFAULT(''),
    [IsActive] BIT NOT NULL CONSTRAINT [DF_SchoolProgram_IsActive] DEFAULT (1),
    [IsSystem] BIT NOT NULL CONSTRAINT [DF_SchoolProgram_IsSystem] DEFAULT (0),
    CONSTRAINT [UNCC_SchoolProgram_Name] UNIQUE NONCLUSTERED ( [Name] ),
    CONSTRAINT [UNCC_SchoolProgram_Code] UNIQUE NONCLUSTERED ( [Code] )
)
