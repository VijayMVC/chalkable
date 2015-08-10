CREATE TABLE [dbo].[ApplicationInstallActionGradeLevel] (
    [Id]                  INT IDENTITY (1, 1) NOT NULL,
    [GradeLevelRef]       INT NOT NULL,
    [AppInstallActionRef] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationInstallActionGradeLevel_ApplicationInstallAction] FOREIGN KEY ([AppInstallActionRef]) REFERENCES [dbo].[ApplicationInstallAction] ([Id]),
    CONSTRAINT [FK_ApplicationInstallActionGradeLevel_GradeLevel] FOREIGN KEY ([GradeLevelRef]) REFERENCES [dbo].[GradeLevel] ([Id])
);

