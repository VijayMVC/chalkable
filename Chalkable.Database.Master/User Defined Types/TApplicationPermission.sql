CREATE TYPE [dbo].[TApplicationPermission] AS TABLE (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [Permission]     INT              NOT NULL);

