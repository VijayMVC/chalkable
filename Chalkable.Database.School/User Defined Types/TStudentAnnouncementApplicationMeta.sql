CREATE TYPE [dbo].[TStudentAnnouncementApplicationMeta] AS TABLE(
	[AnnouncementApplicationRef] INT NOT NULL,
	[StudentRef] INT NOT NULL,
	[Text] NVARCHAR(MAX) NULL
)
