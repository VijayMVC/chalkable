CREATE TABLE [dbo].[ApplicationInstallActionDepartment] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [DepartmentRef]       UNIQUEIDENTIFIER NOT NULL,
    [AppInstallActionRef] INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationInstallActionDepartment_ApplicationInstallAction] FOREIGN KEY ([AppInstallActionRef]) REFERENCES [dbo].[ApplicationInstallAction] ([Id])
);

