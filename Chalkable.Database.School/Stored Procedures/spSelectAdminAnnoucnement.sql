


CREATE Procedure [dbo].[spSelectAdminAnnoucnement] @adminAnnouncementT TAdminAnnouncement readonly
 As
	Select 
		t.*,
		(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = t.Id) as QnACount,	
		0 as StudentsCount, --todo get student from recipients table
		(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = t.Id) as AttachmentsCount,
		(select count(*) from vwAnnouncementAttachment where AnnouncementAttachment_AnnouncementRef = t.Id  and Attachment_PersonRef = t.AdminRef) as OwnerAttachmentsCount,
		0 as StudentsCountWithAttachments, --todo get student from recipients table
		(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = t.Id and Active = 1) as ApplicationCount
	From @adminAnnouncementT t