CREATE TYPE [dbo].[TLimitedEnglish] AS TABLE (
    [Id]  INT  NOT NULL,
	[Code] VARCHAR(5) NULL,
    [Name] VARCHAR(50)  NULL,
    [Description] VARCHAR(255) NULL,
    [StateCode] VARCHAR(10) NULL,
    [SifCode] VARCHAR(10) NULL,
    [NcesCode] VARCHAR(10) NULL,
    [IsActive] BIT NOT NULL,
    [IsSystem] BIT NOT NULL
);