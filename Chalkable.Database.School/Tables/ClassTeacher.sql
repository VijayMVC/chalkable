CREATE TABLE [dbo].[ClassTeacher] (
    [PersonRef]         INT NOT NULL,
    [ClassRef]          INT NOT NULL,
    [IsHighlyQualified] BIT NOT NULL,
    [IsCertified]       BIT NOT NULL,
    [IsPrimary]         BIT NOT NULL,
    CONSTRAINT [PK_ClassTeacher] PRIMARY KEY CLUSTERED ([PersonRef] ASC, [ClassRef] ASC),
    CONSTRAINT [FK_ClassTeacher_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_ClassTeacher_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ClassTeacher_Class]
    ON [dbo].[ClassTeacher]([ClassRef] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ClassTeacher_Teracher]
    ON [dbo].[ClassTeacher]([PersonRef] ASC);

