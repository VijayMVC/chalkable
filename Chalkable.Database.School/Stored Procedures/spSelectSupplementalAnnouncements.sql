CREATE PROCEDURE [dbo].[spSelectSupplementalAnnouncements]
	@supplementalAnnouncementT TSupplementalAnnouncement ReadOnly,
	@orderTable TAnnouncementOrder ReadOnly,
	@desc bit,
	@start int,
	@count int
AS

Declare @isOrderedSelect bit;
Set @isOrderedSelect = (Select Case When count(*) > 0 Then 1 Else 0 End From @orderTable);


If @isOrderedSelect = 1
Begin
	Select 
		t.*,
		(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = t.Id) as QnACount,	
		(Select COUNT(*) from SupplementalAnnouncementRecipient where SupplementalAnnouncementRef = t.Id) as StudentsCount,
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
	From @supplementalAnnouncementT t join @orderTable as sortT
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
		(Select COUNT(*) from SupplementalAnnouncementRecipient where SupplementalAnnouncementRef = t.Id) as StudentsCount,
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
End

Select * From vwAnnouncementAttachment
	Join @supplementalAnnouncementT t 
	On t.Id = vwAnnouncementAttachment.AnnouncementAttachment_AnnouncementRef
GO
