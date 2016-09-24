CREATE TYPE [dbo].[TAuthority] AS TABLE (
    [Id]          UNIQUEIDENTIFIER NULL,
    [Code]        NVARCHAR (10)    NULL,
    [Description] NVARCHAR (256)   NULL);

