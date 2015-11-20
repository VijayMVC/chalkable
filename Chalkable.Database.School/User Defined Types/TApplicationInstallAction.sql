CREATE TYPE [dbo].[TApplicationInstallAction] AS TABLE (
    [Id]             INT              NOT NULL,
    [OwnerRef]       INT              NOT NULL,
    [PersonRef]      INT              NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [Description]    NVARCHAR (MAX)   NULL,
    [Date]           DATETIME2 (7)    NOT NULL,
    [Installed]      BIT              NOT NULL);



