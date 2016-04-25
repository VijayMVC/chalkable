CREATE TABLE [dbo].[PersonSetting] (
    [PersonRef]     INT            NOT NULL,
    [SchoolYearRef] INT            NOT NULL,
    [Key]           NVARCHAR (256) NOT NULL,
    [Value]         NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_PersonSetting_PersonSchoolYearKey] PRIMARY KEY CLUSTERED ([PersonRef] ASC, [SchoolYearRef] ASC, [Key] ASC),
    CONSTRAINT [FK_PersonSetting_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_PersonSetting_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_PersonSetting_SchoolYearRef
	ON dbo.PersonSetting( SchoolYearRef )
GO