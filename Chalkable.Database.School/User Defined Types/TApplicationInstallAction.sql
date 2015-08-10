CREATE TYPE [dbo].[TApplicationInstallAction] AS TABLE (
    [Id]             INT              NOT NULL,
    [OwnerRef]       INT              NOT NULL,
    [PersonRef]      INT              NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [Description]    NVARCHAR (MAX)   NULL);

