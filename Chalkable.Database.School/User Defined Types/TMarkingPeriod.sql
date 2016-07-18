CREATE TYPE [dbo].[TMarkingPeriod] AS TABLE (
    [Id]            INT             NOT NULL,
    [Name]          NVARCHAR (255)  NOT NULL,
    [StartDate]     DATETIME2 (7)   NULL,
    [EndDate]       DATETIME2 (7)   NULL,
    [Description]   NVARCHAR (1024) NULL,
    [SchoolYearRef] INT             NOT NULL,
    [WeekDays]      INT             NOT NULL);

