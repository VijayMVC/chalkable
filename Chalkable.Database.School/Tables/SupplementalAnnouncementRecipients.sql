CREATE TABLE [dbo].[SupplementalAnnouncementRecipients](
	[SupplementalAnnouncementRef] [int] NOT NULL CONSTRAINT [FK_SupplementalAnnouncementRecipients_SupplementalAnnouncement] FOREIGN KEY REFERENCES [SupplementalAnnouncement] ([Id]),
	[StudentRef] [int] NOT NULL CONSTRAINT [FK_SupplementalAnnouncementRecipients_Student] FOREIGN KEY REFERENCES [Student] ([Id]),
	CONSTRAINT [PK_SupplementalAnnouncementRecipients] PRIMARY KEY CLUSTERED ([SupplementalAnnouncementRef] ASC, [StudentRef] ASC)
)