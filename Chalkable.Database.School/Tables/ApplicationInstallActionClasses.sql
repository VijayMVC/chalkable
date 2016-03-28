CREATE TABLE [dbo].[ApplicationInstallActionClasses] (
    [Id]                  INT IDENTITY (1, 1) NOT NULL,
    [ClassRef]            INT NOT NULL,
    [AppInstallActionRef] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationInstallActionClasses_ApplicationInstallAction] FOREIGN KEY ([AppInstallActionRef]) REFERENCES [dbo].[ApplicationInstallAction] ([Id]),
    CONSTRAINT [FK_ApplicationInstallActionClasses_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class] ([Id])
);

