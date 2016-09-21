CREATE TABLE [dbo].[GradingPeriod] (
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
    [AllowGradePosting]  BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GradingPeriod_MarkingPeriod] FOREIGN KEY ([MarkingPeriodRef]) REFERENCES [dbo].[MarkingPeriod] ([Id]),
    CONSTRAINT [FK_GradingPeriod_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_GradingPeriod_MarkingPeriodRef
	ON dbo.GradingPeriod( MarkingPeriodRef )
GO


CREATE NONCLUSTERED INDEX IX_GradingPeriod_SchoolYearRef
	ON dbo.GradingPeriod( SchoolYearRef )
GO