Create Procedure [dbo].[spSelectLessonPlans] 
	@lessonPlanT TLessonPlan ReadOnly,
	@annOrderTable TAnnouncementOrder ReadOnly,
	@desc bit,
	@start int,
	@count int
As

Declare @isOrderedSelect bit;
Set		@isOrderedSelect = (select Case When count(*) > 0 Then 1 Else 0 End from @annOrderTable);

If @isOrderedSelect = 1
Begin
	Select 
		t.*,
		(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = t.Id) as QnACount,	
		(Select COUNT(*) from ClassPerson where ClassRef = t.ClassRef) as StudentsCount,
		(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = t.Id) as AttachmentsCount,
		(select count(*) from vwAnnouncementAttachment where AnnouncementAttachment_AnnouncementRef = t.Id  and Attachment_PersonRef in
			(select PersonRef from ClassTeacher where ClassRef = t.ClassRef)
		) as OwnerAttachmentsCount,
		(select COUNT(*) from
			(Select distinct Attachment_PersonRef from vwAnnouncementAttachment 
				where AnnouncementAttachment_AnnouncementRef = t.Id and 
					ClassRef in (select ClassPerson.ClassRef from ClassPerson 
												where ClassPerson.PersonRef = Attachment_PersonRef)) as x
		) as StudentsCountWithAttachments,
		(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = t.Id and Active = 1) as ApplicationCount,
		(Select COUNT(*) From AnnouncementStandard Where AnnouncementRef = t.Id) as StandardsCount
	From @lessonPlanT t join @annOrderTable LpOrder On t.Id = LpOrder.Id
	Order By (Case When @desc is null or @desc = 1 Then LpOrder.SortedField end) DESC,
		     (Case When @desc = 0 Then LpOrder.SortedField end) ASC
	OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
End
Else
Begin
	Select 
		t.*,
		(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = t.Id) as QnACount,	
		(Select COUNT(*) from ClassPerson where ClassRef = t.ClassRef) as StudentsCount,
		(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = t.Id) as AttachmentsCount,
		(select count(*) from vwAnnouncementAttachment where AnnouncementAttachment_AnnouncementRef = t.Id  and Attachment_PersonRef in
			(select PersonRef from ClassTeacher where ClassRef = t.ClassRef)
		) as OwnerAttachmentsCount,
		(select COUNT(*) from
			(Select distinct Attachment_PersonRef from vwAnnouncementAttachment 
				where AnnouncementAttachment_AnnouncementRef = t.Id and 
					ClassRef in (select ClassPerson.ClassRef from ClassPerson 
												where ClassPerson.PersonRef = Attachment_PersonRef)) as x
		) as StudentsCountWithAttachments,
		(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = t.Id and Active = 1) as ApplicationCount,
		(Select COUNT(*) From AnnouncementStandard Where AnnouncementRef = t.Id) as StandardsCount
	From @lessonPlanT t
End

Select * From vwAnnouncementAttachment
	Join @lessonPlanT t on t.Id = vwAnnouncementAttachment.AnnouncementAttachment_AnnouncementRef
GO