CREATE TYPE [dbo].[TSchoolProgram] AS TABLE
(
	[Id] INT IDENTITY (1,1) NOT NULL,
    [Code] VARCHAR(5) NOT NULL,
    [Name] VARCHAR(50) NOT NULL,
    [Description] VARCHAR(255) NOT NULL,
    [StateCode] VARCHAR(10) NOT NULL,
    [SIFCode] VARCHAR(10) NOT NULL,
    [NCESCode] VARCHAR(10) NOT NULL,
    [IsActive] BIT NOT NULL,
    [IsSystem] BIT NOT NULL
)