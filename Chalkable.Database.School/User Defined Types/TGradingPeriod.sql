CREATE TYPE [dbo].[TGradingPeriod] AS TABLE (
    [Id]                 INT            NOT NULL,
    [Code]               NVARCHAR (5)   NOT NULL,
    [Name]               NVARCHAR (20)  NOT NULL,
    [Description]        NVARCHAR (255) NULL,
    [MarkingPeriodRef]   INT            NOT NULL,
    [SchoolYearRef]      INT            NOT NULL,
    [StartDate]          DATETIME2 (7)  NOT NULL,
    [EndDate]            DATETIME2 (7)  NOT NULL,
    [EndTime]            DATETIME2 (7)  NOT NULL,
    [SchoolAnnouncement] NVARCHAR (255) NULL,
    [AllowGradePosting]  BIT            NOT NULL);

