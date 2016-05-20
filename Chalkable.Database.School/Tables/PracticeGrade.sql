CREATE TABLE [dbo].[PracticeGrade] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [StandardId]     INT              NOT NULL,
    [StudentId]      INT              NOT NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [Score]          NVARCHAR (256)   NULL,
    [Date]           DATETIME2 (7)    NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PracticeGrade_Standard] FOREIGN KEY ([StandardId]) REFERENCES [dbo].[Standard] ([Id]),
    CONSTRAINT [FK_PracticeGrade_Student] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Student] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_PracticeGrade_StandardId
	ON dbo.PracticeGrade( StandardId )
GO

CREATE NONCLUSTERED INDEX IX_PracticeGrade_StudentId
	ON dbo.PracticeGrade( StudentId )
GO