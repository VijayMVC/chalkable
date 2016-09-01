CREATE TABLE [dbo].[Standard] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [Description]     NVARCHAR (MAX)   NULL,
    [Number]          NVARCHAR (MAX)   NULL,
    [Label]           NVARCHAR (MAX)   NULL,
    [IsDeepest]       BIT              NULL,
    [Level]           INT              NULL,
    [Seq]             NVARCHAR (MAX)   NULL,
    [IsActive]        BIT              NULL,
    [Version]         NVARCHAR (MAX)   NULL,
    [DateModified]    DATETIME2 (7)    NULL,
    [AdoptYear]       INT              NULL,
    [AuthorityRef]    UNIQUEIDENTIFIER NOT NULL,
    [DocumentRef]     UNIQUEIDENTIFIER NOT NULL,
    [SubjectRef]      NVARCHAR (25)    NOT NULL,
    [SubjectDocRef]   UNIQUEIDENTIFIER NOT NULL,
    [CourseRef]       UNIQUEIDENTIFIER NOT NULL,
    [ParentRef]       UNIQUEIDENTIFIER NULL,
    [GradeLevelLoRef] NVARCHAR (25)    NOT NULL,
    [GradeLevelHiRef] NVARCHAR (25)    NOT NULL,
    CONSTRAINT [PK_Standard] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Standard_Authority] FOREIGN KEY ([AuthorityRef]) REFERENCES [dbo].[Authority] ([Id]),
    CONSTRAINT [FK_Standard_Course] FOREIGN KEY ([CourseRef]) REFERENCES [dbo].[Course] ([Id]),
    CONSTRAINT [FK_Standard_Document] FOREIGN KEY ([DocumentRef]) REFERENCES [dbo].[Document] ([Id]),
    CONSTRAINT [FK_Standard_GradeLevel_Hi] FOREIGN KEY ([GradeLevelHiRef]) REFERENCES [dbo].[GradeLevel] ([Code]),
    CONSTRAINT [FK_Standard_GradeLevel_Lo] FOREIGN KEY ([GradeLevelLoRef]) REFERENCES [dbo].[GradeLevel] ([Code]),
    CONSTRAINT [FK_Standard_Standard] FOREIGN KEY ([ParentRef]) REFERENCES [dbo].[Standard] ([Id]),
    CONSTRAINT [FK_Standard_Subject] FOREIGN KEY ([SubjectRef]) REFERENCES [dbo].[Subject] ([Code]),
    CONSTRAINT [FK_Standard_SubjectDoc] FOREIGN KEY ([SubjectDocRef]) REFERENCES [dbo].[SubjectDoc] ([Id])
);



