CREATE TABLE [dbo].[AnnouncementAttachment] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (255) NOT NULL,
    [PersonRef]       INT            NOT NULL,
    [AnnouncementRef] INT            NOT NULL,
    [AttachedDate]    DATETIME2 (7)  NOT NULL,
    [Uuid]            NVARCHAR (255) NULL,
    [Order]           INT            NOT NULL,
    [SisAttachmentId] INT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AnnouncementAttachment_Announcement] FOREIGN KEY ([AnnouncementRef]) REFERENCES [dbo].[Announcement] ([Id]),
    CONSTRAINT [FK_AnnouncementAttachment_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_AnnouncementAttachment_Announcement]
    ON [dbo].[AnnouncementAttachment]([AnnouncementRef] ASC);

