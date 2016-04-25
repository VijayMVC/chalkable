CREATE TABLE [dbo].[LessonPlan] (
    [Id]                 INT           NOT NULL,
    [StartDate]          DATETIME2 (7) NULL,
    [EndDate]            DATETIME2 (7) NULL,
    [ClassRef]           INT           NOT NULL,
    [GalleryCategoryRef] INT           NULL,
    [VisibleForStudent]  BIT           NOT NULL,
    [SchoolYearRef]      INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LessonPlan_Announcement] FOREIGN KEY ([Id]) REFERENCES [dbo].[Announcement] ([Id]),
    CONSTRAINT [FK_LessonPlan_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_LessonPlan_LPGalleryCategory] FOREIGN KEY ([GalleryCategoryRef]) REFERENCES [dbo].[LPGalleryCategory] ([Id]),
    CONSTRAINT [FK_LessonPlan_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_LessonPlan_ClassRef
	ON dbo.LessonPlan( ClassRef )
GO

CREATE NONCLUSTERED INDEX IX_LessonPlan_GalleryCategoryRef
	ON dbo.LessonPlan( GalleryCategoryRef )
GO

CREATE NONCLUSTERED INDEX IX_LessonPlan_SchoolYearRef
	ON dbo.LessonPlan( SchoolYearRef )
GO