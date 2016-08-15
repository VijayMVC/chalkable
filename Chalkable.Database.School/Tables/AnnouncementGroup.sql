CREATE TABLE [dbo].[AnnouncementGroup] (
    [AnnouncementRef] INT NOT NULL,
    [GroupRef]        INT NOT NULL,
    CONSTRAINT [PK_AnnouncementGroup] PRIMARY KEY CLUSTERED ([AnnouncementRef] ASC, [GroupRef] ASC),
    CONSTRAINT [FK_AnnouncementGroup_AdminAnnouncement] FOREIGN KEY ([AnnouncementRef]) REFERENCES [dbo].[AdminAnnouncement] ([Id]),
    CONSTRAINT [FK_AnnouncementGroup_Group] FOREIGN KEY ([GroupRef]) REFERENCES [dbo].[Group] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_AnnouncementGroup_GroupRef
	ON dbo.AnnouncementGroup( GroupRef )
GO