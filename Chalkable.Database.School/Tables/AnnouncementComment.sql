CREATE TABLE [dbo].[AnnouncementComment] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	AnnouncementRef INT NOT NULL CONSTRAINT FK_AnnouncementComment_Announcement FOREIGN KEY REFERENCES Announcement(Id),
	AttachmentRef INT NULL CONSTRAINT FK_AnnouncementComment_Attachment FOREIGN KEY REFERENCES Attachment(Id),
	PersonRef INT NOT NULL CONSTRAINT FK_AnnouncementComment_Person FOREIGN KEY REFERENCES Person(Id),
	ParentCommentRef INT NULL CONSTRAINT FK_AnnouncementComment_ParentComment FOREIGN KEY REFERENCES [dbo].[AnnouncementComment](Id),
	[Text] NVARCHAR(MAX) NULL, 
	PostedDate DATETIME2 NOT NULL,
	Hidden BIT NOT NULL,
	Deleted BIT NOT NULL
);
