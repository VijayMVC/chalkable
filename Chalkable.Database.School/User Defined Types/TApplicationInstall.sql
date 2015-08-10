CREATE TYPE [dbo].[TApplicationInstall] AS TABLE (
    [ApplicationRef]      UNIQUEIDENTIFIER NOT NULL,
    [PersonRef]           INT              NOT NULL,
    [InstallDate]         DATETIME2 (7)    NOT NULL,
    [SchoolYearRef]       INT              NOT NULL,
    [OwnerRef]            INT              NOT NULL,
    [Active]              BIT              NOT NULL,
    [AppInstallActionRef] INT              NOT NULL);

