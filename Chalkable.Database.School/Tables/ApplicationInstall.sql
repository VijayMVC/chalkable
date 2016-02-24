CREATE TABLE [dbo].[ApplicationInstall] (
    [Id]                    INT              IDENTITY (1, 1) NOT NULL,
    [ApplicationRef]        UNIQUEIDENTIFIER NOT NULL,
    [PersonRef]             INT              NOT NULL,
    [InstallDate]           DATETIME2 (7)    NOT NULL,
    [SchoolYearRef]         INT              NOT NULL,
    [OwnerRef]              INT              NOT NULL,
    [Active]                BIT              NOT NULL,
    [AppInstallActionRef]   INT              NOT NULL,
    [AppUninstallActionRef] INT              NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationInstall_ApplicationInstallAction] FOREIGN KEY ([AppInstallActionRef]) REFERENCES [dbo].[ApplicationInstallAction] ([Id]),
    CONSTRAINT [FK_ApplicationInstall_AppUninstallAction] FOREIGN KEY ([AppUninstallActionRef]) REFERENCES [dbo].[ApplicationInstallAction] ([Id]),
    CONSTRAINT [FK_ApplicationInstall_Owner] FOREIGN KEY ([OwnerRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_ApplicationInstall_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_ApplicationInstall_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_ApplicationInstall_All]
    ON [dbo].[ApplicationInstall]([ApplicationRef] ASC, [Active] ASC, [PersonRef] ASC, [SchoolYearRef] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ApplicationInstall_Application]
    ON [dbo].[ApplicationInstall]([ApplicationRef] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ApplicationInstall_Person]
    ON [dbo].[ApplicationInstall]([PersonRef] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ApplicationInstall_SchoolYear]
    ON [dbo].[ApplicationInstall]([SchoolYearRef] ASC);

