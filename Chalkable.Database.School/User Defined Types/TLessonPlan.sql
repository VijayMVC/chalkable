﻿CREATE TYPE [dbo].[TLessonPlan] AS TABLE (
    [Id]                   INT              NOT NULL,
    [Created]              DATETIME2 (7)    NOT NULL,
    [State]                INT              NOT NULL,
    [Content]              NVARCHAR (MAX)   NULL,
    [Title]                NVARCHAR (101)    NULL,
    [ClassRef]             INT              NULL,
    [SchoolYearRef]        INT              NOT NULL,
    [StartDate]            DATETIME2 (7)    NULL,
    [EndDate]              DATETIME2 (7)    NULL,
    [LPGalleryCategoryRef]   INT              NULL,
    [VisibleForStudent]    BIT              NULL,
	[InGallery]			   BIT				NOT NULL,
    [PrimaryTeacherName]   NVARCHAR (MAX)   NULL,
    [PrimaryTeacherGender] NVARCHAR (MAX)   NULL,
    [ClassName]            NVARCHAR (30)   NULL,
    [FullClassName]        NVARCHAR (MAX)   NULL,
    [MinGradeLevelId]      INT              NULL,
    [MaxGradeLevelId]      INT              NULL,
    [PrimaryTeacherRef]    INT              NULL,
    [DepartmentId]         UNIQUEIDENTIFIER NULL,
    [IsOwner]              BIT              NULL,
    [Complete]             BIT              NULL,
    [AllCount]             INT              NULL);

