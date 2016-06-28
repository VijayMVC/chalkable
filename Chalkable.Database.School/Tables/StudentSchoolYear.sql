CREATE TABLE [dbo].[StudentSchoolYear] (
    [SchoolYearRef]    INT NOT NULL,
    [GradeLevelRef]    INT NOT NULL,
    [StudentRef]       INT NOT NULL,
    [EnrollmentStatus] INT NOT NULL,
	[IsRetained]       BIT NOT NULL DEFAULT 0,
	[HomeroomRef]	   INT NULL,
    CONSTRAINT [PK_StudentSchoolYear] PRIMARY KEY CLUSTERED ([SchoolYearRef] ASC, [StudentRef] ASC),
    CONSTRAINT [FK_StudentSchoolYear_GradeLevel] FOREIGN KEY ([GradeLevelRef]) REFERENCES [dbo].[GradeLevel] ([Id]),
    CONSTRAINT [FK_StudentSchoolYear_Person] FOREIGN KEY ([StudentRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_StudentSchoolYear_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id]),
	CONSTRAINT [FK_StudentSchoolYear_Homeroom] FOREIGN KEY ([HomeroomRef]) REFERENCES [dbo].[Homeroom] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_StudentSchoolYear_GradeLevelRef
	ON dbo.StudentSchoolYear( GradeLevelRef )
GO
	
CREATE NONCLUSTERED INDEX IX_StudentSchoolYear_StudentRef
	ON dbo.StudentSchoolYear( StudentRef )
GO