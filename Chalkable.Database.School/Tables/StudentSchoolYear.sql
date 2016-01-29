﻿CREATE TABLE [dbo].[StudentSchoolYear] (
    [SchoolYearRef]    INT NOT NULL,
    [GradeLevelRef]    INT NOT NULL,
    [StudentRef]       INT NOT NULL,
    [EnrollmentStatus] INT NOT NULL,
    CONSTRAINT [PK_StudentSchoolYear] PRIMARY KEY CLUSTERED ([SchoolYearRef] ASC, [StudentRef] ASC),
    CONSTRAINT [FK_StudentSchoolYear_GradeLevel] FOREIGN KEY ([GradeLevelRef]) REFERENCES [dbo].[GradeLevel] ([Id]),
    CONSTRAINT [FK_StudentSchoolYear_Person] FOREIGN KEY ([StudentRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_StudentSchoolYear_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);

