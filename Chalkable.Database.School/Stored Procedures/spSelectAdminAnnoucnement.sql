CREATE PROCEDURE [dbo].[spSelectAdminAnnoucnement] 
	@adminAnnouncementT TAdminAnnouncement ReadOnly,
	@annOrderTable TAnnouncementOrder ReadOnly,
	@desc bit,
	@start int,
	@count int
As

Declare @isOrderedSelect bit;
Set @isOrderedSelect = (Select Case When count(*) > 0 Then 1 Else 0 End From @annOrderTable);

if @isOrderedSelect = 1
Begin
	Select 
		t.*,
		(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = t.Id) as QnACount,	
		0 as StudentsCount, --todo get student from recipients table
		(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = t.Id) as AttachmentsCount,
		(select count(*) from vwAnnouncementAttachment where AnnouncementAttachment_AnnouncementRef = t.Id  and Attachment_PersonRef = t.AdminRef) as OwnerAttachmentsCount,
		0 as StudentsCountWithAttachments, --todo get student from recipients table
		(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = t.Id and Active = 1) as ApplicationCount--,
		--(Select ' ' + Attachment_Name From vwAnnouncementAttachment Where vwAnnouncementAttachment.AnnouncementAttachment_AnnouncementRef = t.Id) as AttachmentNames
	From @adminAnnouncementT t join @annOrderTable sortT
		On t.Id = sortT.Id
	Order By (Case When @desc is null or @desc = 1 Then sortT.SortedField End) DESC,
		     (Case When @desc = 0				   Then sortT.SortedField End) ASC
	OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
End
Else
Begin
	Select 
		t.*,
		(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = t.Id) as QnACount,	
		0 as StudentsCount, --todo get student from recipients table
		(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = t.Id) as AttachmentsCount,
		(select count(*) from vwAnnouncementAttachment where AnnouncementAttachment_AnnouncementRef = t.Id  and Attachment_PersonRef = t.AdminRef) as OwnerAttachmentsCount,
		0 as StudentsCountWithAttachments, --todo get student from recipients table
		(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = t.Id and Active = 1) as ApplicationCount--,
		--(Select ' ' + Attachment_Name From vwAnnouncementAttachment Where vwAnnouncementAttachment.AnnouncementAttachment_AnnouncementRef = t.Id) as AttachmentNames
	From @adminAnnouncementT t
End

Select * From vwAnnouncementAttachment
	Join @adminAnnouncementT t 
	on t.Id = vwAnnouncementAttachment.AnnouncementAttachment_AnnouncementRef