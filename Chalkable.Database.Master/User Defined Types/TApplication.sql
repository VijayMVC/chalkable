﻿CREATE TYPE [dbo].[TApplication] AS TABLE (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Description]      NVARCHAR (MAX)   NULL,
    [CreateDateTime]   DATETIME2 (7)    NOT NULL,
    [Url]              NVARCHAR (2048)  NULL,
    [SecretKey]        NVARCHAR (2048)  NULL,
    [ShortDescription] NVARCHAR (MAX)   NULL,
    [AdditionalInfo]   NVARCHAR (MAX)   NULL,
    [Price]            MONEY            NOT NULL,
    [VideoUrl]         NVARCHAR (255)   NULL,
    [SmallPictureRef]  UNIQUEIDENTIFIER NULL,
    [BigPictureRef]    UNIQUEIDENTIFIER NULL,
    [DeveloperRef]     UNIQUEIDENTIFIER NOT NULL,
    [State]            INT              NOT NULL,
    [CanAttach]        BIT              NOT NULL,
    [ShowInGradeView]  BIT              NOT NULL,
    [HasStudentMyApps] BIT              NOT NULL,
    [HasTeacherMyApps] BIT              NOT NULL,
    [HasAdminMyApps]   BIT              NOT NULL,
    [PricePerClass]    MONEY            NULL,
    [PricePerSchool]   MONEY            NULL,
    [OriginalRef]      UNIQUEIDENTIFIER NULL,
    [IsInternal]       BIT              NOT NULL,
    [HasParentMyApps]  BIT              NOT NULL);

