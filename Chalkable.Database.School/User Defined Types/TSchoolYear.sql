CREATE TYPE [dbo].[TSchoolYear] AS TABLE (
    [Id]          INT             NOT NULL,
    [Name]        NVARCHAR (255)  NOT NULL,
    [Description] NVARCHAR (1024) NULL,
    [StartDate]   DATETIME2 (7)   NULL,
    [EndDate]     DATETIME2 (7)   NULL,
    [ArchiveDate] DATETIME2 (7)   NULL,
    [SchoolRef]   INT             NOT NULL,
	[AcadYear]    INT             NOT NULL
	);

