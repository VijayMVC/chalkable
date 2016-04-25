CREATE TABLE [dbo].[AnnouncementQnA] (
    [id]              INT            IDENTITY (1, 1) NOT NULL,
    [AnnouncementRef] INT            NOT NULL,
    [AskerRef]        INT            NOT NULL,
    [Question]        NVARCHAR (MAX) NOT NULL,
    [Answer]          NVARCHAR (MAX) NULL,
    [State]           INT            NOT NULL,
    [AnsweredTime]    DATETIME2 (7)  NULL,
    [QuestionTime]    DATETIME2 (7)  NULL,
    [AnswererRef]     INT            NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_AnnouncementQnA_Announcement] FOREIGN KEY ([AnnouncementRef]) REFERENCES [dbo].[Announcement] ([Id]),
    CONSTRAINT [FK_AnnouncementQnA_Person] FOREIGN KEY ([AskerRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_AnnouncementQnA_PersonRef] FOREIGN KEY ([AnswererRef]) REFERENCES [dbo].[Person] ([Id])
);


GO

CREATE NONCLUSTERED INDEX [IX_AnnouncementQnA_Announcement]
    ON [dbo].[AnnouncementQnA]([AnnouncementRef] ASC);

GO

CREATE NONCLUSTERED INDEX IX_AnnouncementQnA_AnswererRef
	ON dbo.AnnouncementQnA( AnswererRef )
GO


CREATE NONCLUSTERED INDEX IX_AnnouncementQnA_AskerRef
	ON dbo.AnnouncementQnA( AskerRef )
GO
