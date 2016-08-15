CREATE TABLE [dbo].[PersonSetting] (
	[Id] INT NOT NULL IDENTITY(1,1),
    [PersonRef]     INT            NULL,
    [SchoolYearRef] INT            NULL,
	[ClassRef]		INT			   NULL,
    [Key]       NVARCHAR (256) NOT NULL,
    [Value]     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_PersonSetting_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PersonSetting_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_PersonSetting_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id]),
	CONSTRAINT [FK_PersonSetting_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class]([Id]),
	CONSTRAINT [QU_PersonRef_SchoolYearRef_ClassRef_Key] UNIQUE ([PersonRef],[SchoolYearRef], [ClassRef], [Key])
);

GO	
CREATE NONCLUSTERED INDEX IX_PersonSetting_SchoolYearRef
	ON dbo.PersonSetting( SchoolYearRef )
GO