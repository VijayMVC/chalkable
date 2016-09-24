CREATE TABLE [dbo].[Topic] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [Description]     NVARCHAR (MAX)   NULL,
    [IsDeepest]       BIT              NULL,
    [Level]           INT              NULL,
    [IsActive]        BIT              NULL,
    [AdoptYear]       INT              NULL,
    [SubjectRef]      NVARCHAR (25)    NOT NULL,
    [SubjectDocRef]   UNIQUEIDENTIFIER NOT NULL,
    [CourseRef]       UNIQUEIDENTIFIER NOT NULL,
    [ParentRef]       UNIQUEIDENTIFIER NULL,
    [GradeLevelLoRef] NVARCHAR (25)    NOT NULL,
    [GradeLevelHiRef] NVARCHAR (25)    NOT NULL,
    CONSTRAINT [PK_Topic] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Topic_Course] FOREIGN KEY ([CourseRef]) REFERENCES [dbo].[Course] ([Id]),
    CONSTRAINT [FK_Topic_GradeLevel_Hi] FOREIGN KEY ([GradeLevelHiRef]) REFERENCES [dbo].[GradeLevel] ([Code]),
    CONSTRAINT [FK_Topic_GradeLevel_Lo] FOREIGN KEY ([GradeLevelLoRef]) REFERENCES [dbo].[GradeLevel] ([Code]),
    CONSTRAINT [FK_Topic_Subject] FOREIGN KEY ([SubjectRef]) REFERENCES [dbo].[Subject] ([Code]),
    CONSTRAINT [FK_Topic_SubjectDoc] FOREIGN KEY ([SubjectDocRef]) REFERENCES [dbo].[SubjectDoc] ([Id]),
    CONSTRAINT [FK_Topic_Topic] FOREIGN KEY ([ParentRef]) REFERENCES [dbo].[Topic] ([Id])
);


GO
ALTER TABLE [dbo].[Topic] NOCHECK CONSTRAINT [FK_Topic_Topic];




GO
ALTER TABLE [dbo].[Topic] NOCHECK CONSTRAINT [FK_Topic_Topic];


GO
CREATE NONCLUSTERED INDEX [IX_Topic_SubjectDocRef]
    ON [dbo].[Topic]([SubjectDocRef] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Topic_CourseRef]
    ON [dbo].[Topic]([CourseRef] ASC);

