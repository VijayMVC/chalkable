CREATE TABLE [dbo].[StudentAnnouncementApplicationMeta]
(
	[AnnouncementApplicationRef] INT NOT NULL,
	[StudentRef] INT NOT NULL,
	[Text] NVARCHAR(MAX) NULL
	PRIMARY KEY CLUSTERED ([AnnouncementApplicationRef] ASC, [StudentRef] ASC),
	CONSTRAINT [FK_StudentAnnouncementApplicationMeta_AnnouncementApplication] FOREIGN KEY ([AnnouncementApplicationRef]) REFERENCES [dbo].[AnnouncementApplication] ([Id]),
	CONSTRAINT [FK_StudentAnnouncementApplicationMeta_Student] FOREIGN KEY ([StudentRef]) REFERENCES [dbo].[Student] ([Id])
)
