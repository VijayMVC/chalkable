CREATE TABLE [dbo].[AnnouncementAttachment] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [AnnouncementRef] INT           NOT NULL,
    [AttachedDate]    DATETIME2 (7) NOT NULL,
    [Order]           INT           NOT NULL,
    [AttachmentRef]   INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AnnouncementAttachment_Announcement] FOREIGN KEY ([AnnouncementRef]) REFERENCES [dbo].[Announcement] ([Id]),
    CONSTRAINT [FK_AnnouncementAttachment_Attachment] FOREIGN KEY ([AttachmentRef]) REFERENCES [dbo].[Attachment] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_AnnouncementAttachment_Announcement]
    ON [dbo].[AnnouncementAttachment]([AnnouncementRef] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AnnouncementAttachment_AttachmentRef]
    ON [dbo].[AnnouncementAttachment]([AttachmentRef] ASC);

