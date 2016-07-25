CREATE TABLE [dbo].[SchoolPerson] (
    [SchoolRef] INT NOT NULL,
    [PersonRef] INT NOT NULL,
    [RoleRef]   INT NOT NULL,
    CONSTRAINT [PK_SchoolPerson] PRIMARY KEY CLUSTERED ([SchoolRef] ASC, [PersonRef] ASC),
    CONSTRAINT [FK_SchoolStaff_PERSON] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_SchoolStaff_School] FOREIGN KEY ([SchoolRef]) REFERENCES [dbo].[School] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_SchoolPerson_PersonRef
	ON dbo.SchoolPerson( PersonRef )
GO