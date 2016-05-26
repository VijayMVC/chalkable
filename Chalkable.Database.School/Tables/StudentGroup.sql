CREATE TABLE [dbo].[StudentGroup] (
    [GroupRef]   INT NOT NULL,
    [StudentRef] INT NOT NULL,
    CONSTRAINT [PK_StudentGroup] PRIMARY KEY CLUSTERED ([GroupRef] ASC, [StudentRef] ASC),
    CONSTRAINT [FK_StudentGroup_Group] FOREIGN KEY ([GroupRef]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_StudentGroup_Student] FOREIGN KEY ([StudentRef]) REFERENCES [dbo].[Student] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_StudentGroup_StudentRef
	ON dbo.StudentGroup( StudentRef )
GO