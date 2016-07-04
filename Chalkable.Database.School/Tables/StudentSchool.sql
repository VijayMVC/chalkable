CREATE TABLE [dbo].[StudentSchool] (
    [StudentRef] INT NOT NULL,
    [SchoolRef]  INT NOT NULL,
    [CounselorRef] INT NULL,
	[IsTitle1Eligible] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_StudentSchool] PRIMARY KEY CLUSTERED ([StudentRef] ASC, [SchoolRef] ASC),
    CONSTRAINT [FK_StudentSchool_School] FOREIGN KEY ([SchoolRef]) REFERENCES [dbo].[School] ([Id]),
    CONSTRAINT [FK_StudentSchool_Student] FOREIGN KEY ([StudentRef]) REFERENCES [dbo].[Student] ([Id]),
    CONSTRAINT [FK_StudentSchool_Staff] FOREIGN KEY([CounselorRef]) REFERENCES [dbo].[Staff] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [StudentSchool_Student]
    ON [dbo].[StudentSchool]([StudentRef] ASC);

GO

CREATE NONCLUSTERED INDEX IX_StudentSchool_SchoolRef
	ON dbo.StudentSchool( SchoolRef )
GO