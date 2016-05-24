CREATE TABLE [dbo].[ClassPerson] (
    [ClassRef]         INT NOT NULL,
    [PersonRef]        INT NOT NULL,
    [MarkingPeriodRef] INT NOT NULL,
    [IsEnrolled]       BIT NOT NULL,
    CONSTRAINT [PK_ClassPerson] PRIMARY KEY CLUSTERED ([PersonRef] ASC, [ClassRef] ASC, [MarkingPeriodRef] ASC),
    CONSTRAINT [FK_ClassPerson_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_ClassPerson_MarkingPeriod] FOREIGN KEY ([MarkingPeriodRef]) REFERENCES [dbo].[MarkingPeriod] ([Id]),
    CONSTRAINT [FK_ClassPerson_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ClassPerson_Class]
    ON [dbo].[ClassPerson]([ClassRef] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ClassPerson_Person]
    ON [dbo].[ClassPerson]([PersonRef] ASC);

GO	
CREATE NONCLUSTERED INDEX IX_ClassPerson_MarkingPeriodRef
	ON dbo.ClassPerson( MarkingPeriodRef )
GO