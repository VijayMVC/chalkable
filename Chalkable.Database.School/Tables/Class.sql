CREATE TABLE [dbo].[Class] (
    [Id]                     INT              NOT NULL,
    [Name]                   NVARCHAR (255)   NOT NULL,
    [Description]            NVARCHAR (1024)  NULL,
    [ChalkableDepartmentRef] UNIQUEIDENTIFIER NULL,
    [SchoolYearRef]          INT              NULL,
    [PrimaryTeacherRef]      INT              NULL,
    [RoomRef]                INT              NULL,
    [CourseRef]              INT              NULL,
    [GradingScaleRef]        INT              NULL,
    [ClassNumber]            NVARCHAR (41)    NULL,
    [MinGradeLevelRef]       INT              NULL,
    [MaxGradeLevelRef]       INT              NULL,
    [CourseTypeRef]          INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Class_Course] FOREIGN KEY ([CourseRef]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_Class_CourseType] FOREIGN KEY ([CourseTypeRef]) REFERENCES [dbo].[CourseType] ([Id]),
    CONSTRAINT [FK_Class_GradingScale] FOREIGN KEY ([GradingScaleRef]) REFERENCES [dbo].[GradingScale] ([Id]),
    CONSTRAINT [FK_Class_MaxGradeLevel] FOREIGN KEY ([MaxGradeLevelRef]) REFERENCES [dbo].[GradeLevel] ([Id]),
    CONSTRAINT [FK_Class_MinGradeLevel] FOREIGN KEY ([MinGradeLevelRef]) REFERENCES [dbo].[GradeLevel] ([Id]),
    CONSTRAINT [FK_Class_Person] FOREIGN KEY ([PrimaryTeacherRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_Class_RoomRef] FOREIGN KEY ([RoomRef]) REFERENCES [dbo].[Room] ([Id]),
    CONSTRAINT [FK_Class_SchoolYearRef] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Class_PrimaryTeacher]
    ON [dbo].[Class]([PrimaryTeacherRef] ASC);

GO	
CREATE NONCLUSTERED INDEX IX_Class_CourseRef
	ON dbo.Class( CourseRef )
GO


CREATE NONCLUSTERED INDEX IX_Class_CourseTypeRef
	ON dbo.Class( CourseTypeRef )
GO


CREATE NONCLUSTERED INDEX IX_Class_GradingScaleRef
	ON dbo.Class( GradingScaleRef )
GO



CREATE NONCLUSTERED INDEX IX_Class_MaxGradeLevelRef
	ON dbo.Class( MaxGradeLevelRef )
GO



CREATE NONCLUSTERED INDEX IX_Class_MinGradeLevelRef
	ON dbo.Class( MinGradeLevelRef )
GO



CREATE NONCLUSTERED INDEX IX_Class_RoomRef
	ON dbo.Class( RoomRef )
GO



CREATE NONCLUSTERED INDEX IX_Class_SchoolYearRef
	ON dbo.Class( SchoolYearRef )
GO