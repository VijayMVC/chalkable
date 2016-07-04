CREATE VIEW [dbo].[vwAnnouncementComment]
As
Select distinct
	AnnouncementComment.Id as AnnouncementComment_Id,
	AnnouncementComment.AnnouncementRef as AnnouncementComment_AnnouncementRef,
	AnnouncementComment.AttachmentRef as AnnouncementComment_AttachmentRef,
	AnnouncementComment.PostedDate as AnnouncementComment_PostedDate,
	AnnouncementComment.[Text] as AnnouncementComment_Text,
	AnnouncementComment.Hidden as AnnouncementComment_Hidden,
	AnnouncementComment.Deleted as AnnouncementComment_Deleted,
	AnnouncementComment.ParentCommentRef as AnnouncementComment_ParentCommentRef,
	AnnouncementComment.PersonRef as AnnouncementComment_PersonRef,
	Announcement.DiscussionEnabled as Announcement_DiscussionEnabled,
	Announcement.PreviewCommentsEnabled as Announcement_PreviewCommentsEnabled,
	Announcement.RequireCommentsEnabled as Announcement_RequireCommentsEnabled,
	vwPerson.Id as Person_Id,
	vwPerson.FirstName as Person_FirstName,
	vwPerson.LastName as Person_LastName,
	vwPerson.Gender as Person_Gender,
	vwPerson.RoleRef as Person_RoleRef,
	vwPerson.SchoolRef as Person_SchoolRef,
	Attachment.Id as Attachment_Id,
	Attachment.Name as Attachment_Name,
	Attachment.PersonRef as Attachment_PersonRef,
	Attachment.LastAttachedDate as Attachment_LastAttachedDate,
	Attachment.RelativeBlobAddress as Attachment_RelativeBlobAddress,
	Attachment.UploadedDate as Attachment_UploadedDate,
	Attachment.Uuid as Attachment_Uuid,
	Attachment.SisAttachmentId as Attachment_SisAttachmentId
From 
	AnnouncementComment
Join vwPerson on vwPerson.Id = AnnouncementComment.PersonRef
Join Announcement on Announcement.Id = AnnouncementComment.AnnouncementRef
Left Join Attachment on Attachment.Id = AnnouncementComment.AttachmentRef

