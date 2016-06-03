CREATE TABLE [dbo].[AdminAnnouncement] (
    [Id]       INT           NOT NULL,
    [AdminRef] INT           NOT NULL,
    [Expires]  DATETIME2 (7) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AdminAnnouncement_Announcement] FOREIGN KEY ([Id]) REFERENCES [dbo].[Announcement] ([Id]),
    CONSTRAINT [FK_AdminAnnouncement_Person] FOREIGN KEY ([AdminRef]) REFERENCES [dbo].[Person] ([Id])
);

GO
CREATE NONCLUSTERED INDEX IX_AdminAnnouncement_AdminRef
	ON dbo.AdminAnnouncement( AdminRef )
GO
