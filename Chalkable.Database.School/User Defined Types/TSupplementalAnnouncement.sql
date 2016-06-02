﻿CREATE TYPE [dbo].[TSupplementalAnnouncement] AS TABLE (
	[Id]						INT              NOT NULL,
    [Created]					DATETIME2 (7)    NOT NULL,
    [State]						INT              NOT NULL,
    [Content]					NVARCHAR (MAX)   NULL,
    [Title]						NVARCHAR (30)   NULL,
    [ClassRef]					INT              NOT NULL,
    [SchoolYearRef]				INT              NOT NULL,
    [Expires]					DATETIME2 (7)    NULL,
    [ClassAnnouncementTypeRef]	INT              NULL,
    [VisibleForStudent]			BIT              NULL,
    [PrimaryTeacherName]		NVARCHAR (MAX)   NULL,
    [PrimaryTeacherGender]		NVARCHAR (MAX)   NULL,
    [ClassName]					NVARCHAR (30)    NULL,
    [FullClassName]				NVARCHAR (MAX)   NULL,
	[PrimaryTeacherRef]			INT				 NULL
)