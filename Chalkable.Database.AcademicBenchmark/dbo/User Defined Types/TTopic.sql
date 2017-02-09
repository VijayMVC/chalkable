CREATE TYPE [dbo].[TTopic] AS TABLE (
    [Id]              UNIQUEIDENTIFIER NULL,
    [Description]     NVARCHAR (MAX)   NULL,
    [IsDeepest]       BIT              NULL,
    [Level]           INT              NULL,
    [IsActive]        BIT              NULL,
    [AdoptYear]       INT              NULL,
    [SubjectRef]      NVARCHAR (25)    NULL,
    [SubjectDocRef]   UNIQUEIDENTIFIER NULL,
    [CourseRef]       UNIQUEIDENTIFIER NULL,
    [ParentRef]       UNIQUEIDENTIFIER NULL,
    [GradeLevelLoRef] NVARCHAR (25)    NULL,
    [GradeLevelHiRef] NVARCHAR (25)    NULL);

