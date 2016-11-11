Create Type [dbo].[TAnnouncementComment]  As Table(
	[Id] INT NOT NULL,
	AnnouncementRef INT NOT NULL,
	PersonRef INT NOT NULL,
	ParentCommentRef INT NULL,
	[Text] NVARCHAR(MAX) NULL, 
	PostedDate DATETIME2 NOT NULL,
	Hidden BIT NOT NULL,
	Deleted BIT NOT NULL
)