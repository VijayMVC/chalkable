CREATE TABLE [dbo].[Date] (
    [Day]             DATETIME2 (7) NOT NULL,
    [DayTypeRef]      INT           NULL,
    [SchoolYearRef]   INT           NOT NULL,
    [IsSchoolDay]     BIT           NOT NULL,
    [BellScheduleRef] INT           NULL,
    CONSTRAINT [PK_Date] PRIMARY KEY CLUSTERED ([Day] ASC, [SchoolYearRef] ASC),
    CONSTRAINT [FK_Date_BellSchedule] FOREIGN KEY ([BellScheduleRef]) REFERENCES [dbo].[BellSchedule] ([Id]),
    CONSTRAINT [FK_Date_DayType] FOREIGN KEY ([DayTypeRef]) REFERENCES [dbo].[DayType] ([Id]),
    CONSTRAINT [FK_Date_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_Date_BellScheduleRef
	ON dbo.Date( BellScheduleRef )
GO
CREATE NONCLUSTERED INDEX IX_Date_DayTypeRef
	ON dbo.Date( DayTypeRef )
GO


CREATE NONCLUSTERED INDEX IX_Date_SchoolYearRef
	ON dbo.Date( SchoolYearRef )
GO