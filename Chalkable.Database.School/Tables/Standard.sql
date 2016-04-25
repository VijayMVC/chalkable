CREATE TABLE [dbo].[Standard] (
    [Id]                  INT              NOT NULL,
    [ParentStandardRef]   INT              NULL,
    [Name]                NVARCHAR (100)   NOT NULL,
    [Description]         NVARCHAR (MAX)   NOT NULL,
    [StandardSubjectRef]  INT              NOT NULL,
    [LowerGradeLevelRef]  INT              NULL,
    [UpperGradeLevelRef]  INT              NULL,
    [IsActive]            BIT              NOT NULL,
    [AcademicBenchmarkId] UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Standard_LowerGradeLevel] FOREIGN KEY ([LowerGradeLevelRef]) REFERENCES [dbo].[GradeLevel] ([Id]),
    CONSTRAINT [FK_Standard_ParentStandard] FOREIGN KEY ([ParentStandardRef]) REFERENCES [dbo].[Standard] ([Id]),
    CONSTRAINT [FK_Standard_StandardSubject] FOREIGN KEY ([StandardSubjectRef]) REFERENCES [dbo].[StandardSubject] ([Id]),
    CONSTRAINT [FK_Standard_UpperGradeLevel] FOREIGN KEY ([UpperGradeLevelRef]) REFERENCES [dbo].[GradeLevel] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Standard_AcademicBenchmarkId]
    ON [dbo].[Standard]([AcademicBenchmarkId] ASC);

GO	

CREATE NONCLUSTERED INDEX IX_Standard_LowerGradeLevelRef
	ON dbo.Standard( LowerGradeLevelRef )
GO

CREATE NONCLUSTERED INDEX IX_Standard_ParentStandardRef
	ON dbo.Standard( ParentStandardRef )
GO

CREATE NONCLUSTERED INDEX IX_Standard_StandardSubjectRef
	ON dbo.Standard( StandardSubjectRef )
GO

CREATE NONCLUSTERED INDEX IX_Standard_UpperGradeLevelRef
	ON dbo.Standard( UpperGradeLevelRef )
GO