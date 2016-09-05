CREATE TYPE [dbo].[TStandard] AS TABLE (
    [Id]              UNIQUEIDENTIFIER NULL,
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
    [AuthorityRef]    UNIQUEIDENTIFIER NULL,
    [DocumentRef]     UNIQUEIDENTIFIER NULL,
    [SubjectRef]      NVARCHAR (25)    NULL,
    [SubjectDocRef]   UNIQUEIDENTIFIER NULL,
    [CourseRef]       UNIQUEIDENTIFIER NULL,
    [ParentRef]       UNIQUEIDENTIFIER NULL,
    [GradeLevelLoRef] NVARCHAR (25)    NULL,
    [GradeLevelHiRef] NVARCHAR (25)    NULL);



