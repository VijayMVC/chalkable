CREATE TYPE [dbo].[TUserLoginInfo] AS TABLE (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [SisToken]          NVARCHAR (MAX)   NULL,
    [SisTokenExpires]   DATETIME2 (7)    NULL,
    [LastPasswordReset] DATETIME2 (7)    NULL);

