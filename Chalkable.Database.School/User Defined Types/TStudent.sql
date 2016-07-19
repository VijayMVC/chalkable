﻿CREATE TYPE [dbo].[TStudent] AS TABLE (
    [Id]					 INT            NOT NULL,
    [FirstName]				 NVARCHAR (510) NOT NULL,
	[MiddleName]			 NVARCHAR (255) NULL,
    [LastName]				 NVARCHAR (510) NOT NULL,
    [BirthDate]				 DATETIME2 (7)  NULL,
    [Gender]				 NVARCHAR (510) NULL,
    [HasMedicalAlert]		 BIT            NOT NULL,
    [IsAllowedInetAccess]	 BIT            NOT NULL,
    [SpecialInstructions]	 NVARCHAR (MAX) NOT NULL,
    [SpEdStatus]			 NVARCHAR (512) NULL,
    [PhotoModifiedDate]		 DATETIME2 (7)  NULL,
    [UserId]				 INT            NOT NULL,
	[IsHispanic]			 BIT			NOT NULL,
	[IEPBeginDate]			 DATETIME2(7)	NULL,
	[IEPEndDate]			 DATETIME2(7)	NULL,
	[GenderDescriptor]		 NVARCHAR(128)  NULL,
	[IsHomeless]	    	 BIT			NOT NULL DEFAULT 0,
	[IsImmigrant]			 BIT			NOT NULL DEFAULT 0,
	[LimitedEnglishRef]		 INT			NULL,
	[IsForeignExchange]		 BIT			NOT NULL DEFAULT 0,
	[StateIdNumber]		     NVARCHAR(128)  NULL,
	[AltStudentNumber]	     NVARCHAR(128)  NULL,
	[StudentNumber]		     NVARCHAR(128)	NULL,
	[OriginalEnrollmentDate] DATETIME2(7)	NULL
);

