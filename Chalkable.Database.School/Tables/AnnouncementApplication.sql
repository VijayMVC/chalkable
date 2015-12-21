CREATE TABLE [dbo].[AnnouncementApplication] (
    [Id]              INT              IDENTITY (1, 1) NOT NULL,
    [AnnouncementRef] INT              NOT NULL,
    [ApplicationRef]  UNIQUEIDENTIFIER NOT NULL,
    [Active]          BIT              NOT NULL,
    [Order]           INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AnnouncementApplication_Announcement] FOREIGN KEY ([AnnouncementRef]) REFERENCES [dbo].[Announcement] ([Id])
);

