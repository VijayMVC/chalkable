CREATE TABLE [dbo].[MarkingPeriod] (
    [Id]            INT             NOT NULL,
    [Name]          NVARCHAR (255)  NOT NULL,
    [StartDate]     DATETIME2 (7)   NULL,
    [EndDate]       DATETIME2 (7)   NULL,
    [Description]   NVARCHAR (1024) NULL,
    [SchoolYearRef] INT             NOT NULL,
    [WeekDays]      INT             DEFAULT ((126)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MarkingPeriod_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_MarkingPeriod_SchoolYearRef
	ON dbo.MarkingPeriod( SchoolYearRef )
GO
