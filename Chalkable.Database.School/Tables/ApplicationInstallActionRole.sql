CREATE TABLE [dbo].[ApplicationInstallActionRole] (
    [Id]                  INT IDENTITY (1, 1) NOT NULL,
    [RoleId]              INT NOT NULL,
    [AppInstallActionRef] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationInstallActionRole_ApplicationInstallAction] FOREIGN KEY ([AppInstallActionRef]) REFERENCES [dbo].[ApplicationInstallAction] ([Id])
);

