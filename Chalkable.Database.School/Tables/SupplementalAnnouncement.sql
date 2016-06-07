CREATE TABLE [dbo].[SupplementalAnnouncement](
	[Id] [int] NOT NULL,
	[Expires] [datetime2](7) NULL,
	[VisibleForStudent] [bit] NOT NULL,
	[ClassRef] [int] NOT NULL,
	[ClassAnnouncementTypeRef] [int] NULL,
	[SchoolYearRef] [INT] NOT NULL,
	PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_SupplementalAnnouncement_Announcement] FOREIGN KEY ([Id]) REFERENCES [Announcement] ([Id]),
	CONSTRAINT [FK_SupplementalAnnouncement_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class] ([Id]),
	CONSTRAINT [FK_SupplementalAnnouncement_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
)