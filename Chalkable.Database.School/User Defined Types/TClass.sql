CREATE TYPE [dbo].[TClass] AS TABLE (
    [Id]                     INT              NOT NULL,
    [Name]                   NVARCHAR (255)   NOT NULL,
    [Description]            NVARCHAR (1024)  NULL,
    [ChalkableDepartmentRef] UNIQUEIDENTIFIER NULL,
    [SchoolYearRef]          INT              NULL,
    [PrimaryTeacherRef]      INT              NULL,
    [MinGradeLevelRef]       INT              NULL,
    [MaxGradeLevelRef]       INT              NULL,
    [RoomRef]                INT              NULL,
    [CourseRef]              INT              NULL,
    [CourseTypeRef]          INT              NOT NULL,
    [GradingScaleRef]        INT              NULL,
    [ClassNumber]            NVARCHAR (41)    NULL);

