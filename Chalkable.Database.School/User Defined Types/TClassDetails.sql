﻿CREATE TYPE [dbo].[TClassDetails] AS TABLE (
    [Class_Id]                     INT              NULL,
    [Class_ClassNumber]            NVARCHAR (MAX)   NULL,
    [Class_Name]                   NVARCHAR (MAX)   NULL,
    [Class_Description]            NVARCHAR (MAX)   NULL,
    [Class_SchoolYearRef]          INT              NULL,
    [Class_PrimaryTeacherRef]      INT              NULL,
    [Class_MinGradeLevelRef]       INT              NULL,
    [Class_MaxGradeLevelRef]       INT              NULL,
    [Class_ChalkableDepartmentRef] UNIQUEIDENTIFIER NULL,
    [Class_CourseRef]              INT              NULL,
    [Class_CourseTypeRef]          INT              NOT NULL,
    [Class_RoomRef]                INT              NULL,
    [Class_GradingScaleRef]        INT              NULL,
    [Person_Id]                    INT              NULL,
    [Person_FirstName]             NVARCHAR (MAX)   NULL,
    [Person_LastName]              NVARCHAR (MAX)   NULL,
    [Person_Gender]                NVARCHAR (MAX)   NULL,
    [Person_RoleRef]               INT              NULL,
    [SchoolYear_Id]                INT              NULL,
    [SchoolYear_SchoolRef]         INT              NULL,
    [SchoolYear_Name]              NVARCHAR (MAX)   NULL,
    [SchoolYear_Description]       NVARCHAR (MAX)   NULL,
    [SchoolYear_StartDate]         DATETIME2 (7)    NULL,
    [SchoolYear_EndDate]           DATETIME2 (7)    NULL,
    [SchoolYear_ArchiveDate]       DATETIME2 (7)    NULL,
    [Class_StudentsCount]          INT              NULL);

