CREATE TABLE [dbo].[LessonPlan] (
    [Id]                 INT           NOT NULL,
    [StartDate]          DATETIME2 (7) NULL,
    [EndDate]            DATETIME2 (7) NULL,
    [ClassRef]           INT           NULL,
    [LpGalleryCategoryRef] INT           NULL,
    [VisibleForStudent]  BIT           NOT NULL,
    [SchoolYearRef]      INT           NOT NULL,
	[InGallery]			 BIT		   NOT NULL DEFAULT 0,
	[GalleryOwnerRef]	 INT		   NULL
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LessonPlan_Announcement] FOREIGN KEY ([Id]) REFERENCES [dbo].[Announcement] ([Id]),
    CONSTRAINT [FK_LessonPlan_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_LessonPlan_LPGalleryCategory] FOREIGN KEY ([LpGalleryCategoryRef]) REFERENCES [dbo].[LPGalleryCategory] ([Id]),
    CONSTRAINT [FK_LessonPlan_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id]),
	CONSTRAINT [FK_LessonPlan_Person] FOREIGN KEY ([GalleryOwnerRef]) REFERENCES [dbo].[Person] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_LessonPlan_ClassRef
	ON dbo.LessonPlan( ClassRef )
GO

CREATE NONCLUSTERED INDEX IX_LessonPlan_LpGalleryCategoryRef
	ON dbo.LessonPlan( LpGalleryCategoryRef )
GO

CREATE NONCLUSTERED INDEX IX_LessonPlan_SchoolYearRef
	ON dbo.LessonPlan( SchoolYearRef )
GO