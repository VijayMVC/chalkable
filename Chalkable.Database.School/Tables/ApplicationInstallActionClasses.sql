CREATE TABLE [dbo].[ApplicationInstallActionClasses] (
    [Id]                  INT IDENTITY (1, 1) NOT NULL,
    [ClassRef]            INT NOT NULL,
    [AppInstallActionRef] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationInstallActionClasses_ApplicationInstallAction] FOREIGN KEY ([AppInstallActionRef]) REFERENCES [dbo].[ApplicationInstallAction] ([Id]),
    CONSTRAINT [FK_ApplicationInstallActionClasses_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_ApplicationInstallActionClasses_AppInstallActionRef
	ON dbo.ApplicationInstallActionClasses( AppInstallActionRef )
GO


CREATE NONCLUSTERED INDEX IX_ApplicationInstallActionClasses_ClassRef
	ON dbo.ApplicationInstallActionClasses( ClassRef )
GO