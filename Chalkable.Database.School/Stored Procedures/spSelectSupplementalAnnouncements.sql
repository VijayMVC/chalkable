CREATE PROCEDURE [dbo].[spSelectSupplementalAnnouncements] 
	@supplementalAnnouncementT TSupplementalAnnouncement readonly
AS
Select 
	t.*,
	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = t.Id) as QnACount,	
	(Select COUNT(*) from ClassPerson where ClassRef = t.ClassRef) as StudentsCount,
	(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = t.Id) as AttachmentsCount,
	(select count(*) from vwAnnouncementAttachment where AnnouncementAttachment_AnnouncementRef = t.Id  and Attachment_PersonRef in
		(select PersonRef from ClassTeacher where ClassRef = t.ClassRef)
	) as OwnerAttachmentsCount,
	(	select COUNT(*) from
			(Select distinct Attachment_PersonRef from vwAnnouncementAttachment 
			 where AnnouncementAttachment_AnnouncementRef = t.Id and 
				   ClassRef in (select ClassPerson.ClassRef from ClassPerson 
											 where ClassPerson.PersonRef = Attachment_PersonRef)) as x
	) as StudentsCountWithAttachments,
	(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = t.Id and Active = 1) as ApplicationCount 
From @supplementalAnnouncementT t
