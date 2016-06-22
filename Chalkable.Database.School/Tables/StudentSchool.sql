CREATE TABLE [dbo].[StudentSchool] (
    [StudentRef] INT NOT NULL,
    [SchoolRef]  INT NOT NULL,
    CONSTRAINT [PK_StudentSchool] PRIMARY KEY CLUSTERED ([StudentRef] ASC, [SchoolRef] ASC),
    CONSTRAINT [FK_StudentSchool_School] FOREIGN KEY ([SchoolRef]) REFERENCES [dbo].[School] ([Id]),
    CONSTRAINT [FK_StudentSchool_Student] FOREIGN KEY ([StudentRef]) REFERENCES [dbo].[Student] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [StudentSchool_Student]
    ON [dbo].[StudentSchool]([StudentRef] ASC);

GO	
CREATE NONCLUSTERED INDEX IX_StudentSchool_SchoolRef
	ON dbo.StudentSchool( SchoolRef )
GO