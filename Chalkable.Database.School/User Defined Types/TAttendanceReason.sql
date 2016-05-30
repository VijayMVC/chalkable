﻿CREATE TYPE [dbo].[TAttendanceReason] AS TABLE (
    [Id]          INT            NOT NULL,
    [Code]        NVARCHAR (255) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Category]    NVARCHAR (255) NOT NULL,
    [IsSystem]    BIT            NOT NULL,
    [IsActive]    BIT            NOT NULL);

