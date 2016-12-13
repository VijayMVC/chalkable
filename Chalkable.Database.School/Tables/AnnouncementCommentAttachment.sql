CREATE TABLE [dbo].[AnnouncementCommentAttachment]
(
	[AnnouncementCommentRef] Int not null Constraint FK_AnnouncementCommentAttachment_AnnouncementComment Foreign Key References AnnouncementComment(Id),
	[AttachmentRef] Int Not Null Constraint FK_AnnouncementCommentAttachment_Attachment Foreign Key References Attachment(Id),
	Constraint PK_AnnouncementCommentAttachment Primary Key([AnnouncementCommentRef], [AttachmentRef])
)

