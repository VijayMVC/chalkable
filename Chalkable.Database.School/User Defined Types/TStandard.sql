CREATE TYPE [dbo].[TStandard] AS TABLE (
    [Id]                  INT              NOT NULL,
    [ParentStandardRef]   INT              NULL,
    [Name]                NVARCHAR (100)   NOT NULL,
    [Description]         NVARCHAR (MAX)   NOT NULL,
    [StandardSubjectRef]  INT              NOT NULL,
    [LowerGradeLevelRef]  INT              NULL,
    [UpperGradeLevelRef]  INT              NULL,
    [IsActive]            BIT              NOT NULL,
    [AcademicBenchmarkId] UNIQUEIDENTIFIER NULL);

