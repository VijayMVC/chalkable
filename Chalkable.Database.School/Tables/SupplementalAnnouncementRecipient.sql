CREATE TABLE [dbo].[SupplementalAnnouncementRecipient](
	[SupplementalAnnouncementRef] [int] NOT NULL CONSTRAINT [FK_SupplementalAnnouncementRecipient_SupplementalAnnouncement] FOREIGN KEY REFERENCES [SupplementalAnnouncement] ([Id]),
	[StudentRef] [int] NOT NULL CONSTRAINT [FK_SupplementalAnnouncementRecipient_Student] FOREIGN KEY REFERENCES [Student] ([Id]),
	CONSTRAINT [PK_SupplementalAnnouncementRecipient] PRIMARY KEY CLUSTERED ([SupplementalAnnouncementRef] ASC, [StudentRef] ASC)
)