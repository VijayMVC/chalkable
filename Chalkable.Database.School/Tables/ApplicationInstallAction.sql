CREATE TABLE [dbo].[ApplicationInstallAction] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [OwnerRef]       INT              NOT NULL,
    [PersonRef]      INT              NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [Description]    NVARCHAR (MAX)   NULL,
    [Date]           DATETIME2 (7)    NOT NULL,
    [OwnerRoleId]    INT              NOT NULL,
    [Installed]      BIT              NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationInstallAction_Owner] FOREIGN KEY ([OwnerRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_ApplicationInstallAction_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id])
);



