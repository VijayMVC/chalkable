﻿CREATE TYPE [dbo].[TClassAnnouncement] AS TABLE (
    [Id]                       INT              NOT NULL,
    [Created]                  DATETIME2 (7)    NOT NULL,
    [State]                    INT              NOT NULL,
    [Content]                  NVARCHAR (MAX)   NULL,
    [Title]                    NVARCHAR (MAX)   NULL,
    [Expires]                  DATETIME2 (7)    NOT NULL,
    [Order]                    INT              NOT NULL,
    [Dropped]                  BIT              NOT NULL,
    [ClassAnnouncementTypeRef] INT              NULL,
    [SchoolYearRef]            INT              NULL,
    [SisActivityId]            INT              NULL,
    [MaxScore]                 DECIMAL (18)     NULL,
    [WeightAddition]           DECIMAL (9, 6)   NULL,
    [WeightMultiplier]         DECIMAL (9, 6)   NULL,
    [MayBeDropped]             BIT              NULL,
    [VisibleForStudent]        BIT              NULL,
    [ClassRef]                 INT              NOT NULL,
    [PrimaryTeacherName]       NVARCHAR (MAX)   NULL,
    [PrimaryTeacherGender]     NVARCHAR (MAX)   NULL,
    [ClassName]                NVARCHAR (MAX)   NULL,
    [FullClassName]            NVARCHAR (MAX)   NULL,
    [MinGradeLevelId]          INT              NULL,
    [MaxGradeLevelId]          INT              NULL,
    [PrimaryTeacherRef]        INT              NULL,
    [DepartmentId]             UNIQUEIDENTIFIER NULL,
    [IsOwner]                  BIT              NULL,
    [Complete]                 BIT              NULL,
    [AllCount]                 INT              NULL);
