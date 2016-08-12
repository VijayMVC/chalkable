﻿CREATE TABLE [dbo].[District] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (256)   NOT NULL,
    [SisUrl]           NVARCHAR (1024)  NULL,
    [SisUserName]      NVARCHAR (1024)  NULL,
    [SisPassword]      NVARCHAR (1024)  NULL,
    [Status]           INT              NOT NULL,
    [TimeZone]         NVARCHAR (1024)  NOT NULL,
    [ServerUrl]        NVARCHAR (256)   NOT NULL,
    [SisRedirectUrl]   NVARCHAR (1024)  NULL,
    [LastSync]         DATETIME2 (7)    NULL,
    [MaxSyncTime]      INT              NOT NULL,
    [SyncLogFlushSize] INT              NOT NULL,
    [SyncHistoryDays]  INT              NOT NULL,
    [SyncFrequency]    INT              NULL,
    [MaxSyncFrequency] INT              NULL,
    [FailCounter]      INT              NOT NULL,
    [FailDelta]        INT              NOT NULL,
    [StateCode]        NVARCHAR (2)     NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

