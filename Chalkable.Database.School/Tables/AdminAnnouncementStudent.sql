CREATE TABLE [dbo].[AdminAnnouncementStudent]
(
	[AdminAnnouncementRef] INT NOT NULL,
	[StudentRef] INT NOT NULL,
	PRIMARY KEY ([AdminAnnouncementRef], [StudentRef]),
	CONSTRAINT [FK_AdminAnnouncementStudent_AdminAnnouncement] FOREIGN KEY ([AdminAnnouncementRef]) REFERENCES [dbo].[AdminAnnouncement] ([Id]),
	CONSTRAINT [FK_AdminAnnouncementStudent_Student] FOREIGN KEY ([StudentRef]) REFERENCES [dbo].[Student] ([Id])
)