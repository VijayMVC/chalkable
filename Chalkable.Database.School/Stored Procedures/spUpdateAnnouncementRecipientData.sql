

CREATE Procedure [dbo].[spUpdateAnnouncementRecipientData] 
@announcementId Int, 
@personId Int,
@roleId Int, 
@complete Bit,
@tillDate datetime2,
@schoolYearId Int,
@annType int,
@classId int
As

declare @annToMark table (Id int, Complete bit)

Begin Transaction
if @announcementId is null
Begin
	if @annType = 3
	Begin
		Insert Into @annToMark
		Select 
			Id, annRecData.Complete
		From 
			vwLessonPlan
			left join (select * from AnnouncementRecipientData where PersonRef = @personId) as annRecData on Id = annRecData.AnnouncementRef
		Where 
			(@tillDate is null or EndDate <= @tillDate)
			and ((@roleId = 2 
				  and exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef))
				 or (@roleId = 3 
					 and exists(select * from ClassPerson where PersonRef = @personId and ClassPerson.ClassRef = vwLessonPlan.ClassRef) and VisibleForStudent = 1))
			and SchoolYearRef = @schoolYearId
			and [State] = 1
			and (@classId is null or vwLessonPlan.ClassRef = @classId)
		Group by vwLessonPlan.Id, annRecData.Complete
	End
	
	If @annType = 2
	Begin
		Insert Into @annToMark
		Select 
			Id, annRecData.Complete
		From
			vwAdminAnnouncement
			left join(select * from AnnouncementRecipientData where PersonRef = @personId) as annRecData on Id = annRecData.AnnouncementRef
		Where
			(@tillDate is null or Expires<=@tillDate)
			and ((@roleId = 3 
				  and exists(select * from AnnouncementGroup ar
			 				join StudentGroup on StudentGroup.GroupRef = ar.GroupRef
			 				where StudentGroup.StudentRef = @personId and AnnouncementRef = vwAdminAnnouncement.Id))
				 or (@roleId = 10 and vwAdminAnnouncement.AdminRef = @personId))
			and [State] = 1
		Group by vwAdminAnnouncement.Id, annRecData.Complete
	End
End
Else
Begin
	insert into @annToMark 
	values 
	(
		@announcementId,
		(Select Top 1 Complete From AnnouncementRecipientData
		 Where AnnouncementRef = @announcementId and PersonRef = @personId)
	)
End

Insert Into AnnouncementRecipientData(AnnouncementRef, PersonRef, Complete)
		select Id, @personId, 1 from @annToMark where Complete is null
Update AnnouncementRecipientData
		Set Complete = @complete
		Where AnnouncementRef in(select Id from @annToMark where Complete is not null) and PersonRef = @personId

Commit Transaction