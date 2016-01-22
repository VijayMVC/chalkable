CREATE PROCEDURE [dbo].[spCompleteAnnouncements]
	@personId int,
	@roleId int,
	@schoolYearId int,
	@classId int,
	@announcementType int,
	@fromDate datetime2,
	@toDate datetime2
AS

declare @annToMark table (Id int, Complete bit)

--Announcement types
declare @LESSON_PLAN int = 3,
		@ADMIN_ANNOUNCEMENT int = 2;
--Roles ids
declare @TEACHER_ROLE int = 2,
		@STUDENT_ROLE int = 3,
		@DISTRICT_ADMIN_ROLE int = 10;

if @announcementType = @LESSON_PLAN
Begin
	Insert Into @annToMark
	Select 
		Id, 
		annRecipData.Complete
	From 
		vwLessonPlan
		left join (select * from AnnouncementRecipientData where PersonRef = @personId) as annRecipData on Id = annRecipData.AnnouncementRef
	Where 
		(@toDate is null or EndDate <= @toDate)
		and (@fromDate is null or EndDate >= @fromDate)
		and ((@roleId = @TEACHER_ROLE 
			  and exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef))
			 or (@roleId = @STUDENT_ROLE
				 and exists(select * from ClassPerson where PersonRef = @personId and ClassPerson.ClassRef = vwLessonPlan.ClassRef) and VisibleForStudent = 1))
		and SchoolYearRef = @schoolYearId
		and [State] = 1
		and (@classId is null or vwLessonPlan.ClassRef = @classId)
	Group by vwLessonPlan.Id, annRecipData.Complete
End
	
If @announcementType = @ADMIN_ANNOUNCEMENT
Begin
	Insert Into @annToMark
	Select 
		Id, annRecipData.Complete
	From
		vwAdminAnnouncement
		left join(select * from AnnouncementRecipientData where PersonRef = @personId) as annRecipData on Id = annRecipData.AnnouncementRef
	Where
		(@toDate is null or Expires <= @toDate)
		And (@fromDate is null or Expires >= @fromDate)
		And ((@roleId = @STUDENT_ROLE 
			  and exists(select * from AnnouncementGroup ar
		 				join StudentGroup on StudentGroup.GroupRef = ar.GroupRef
		 				where StudentGroup.StudentRef = @personId and AnnouncementRef = vwAdminAnnouncement.Id))
			 or (@roleId = @DISTRICT_ADMIN_ROLE and vwAdminAnnouncement.AdminRef = @personId))
		and [State] = 1
	Group by vwAdminAnnouncement.Id, annRecipData.Complete
End

Insert Into AnnouncementRecipientData(AnnouncementRef, PersonRef, Complete)
		select Id, @personId, 1 from @annToMark where Complete is null
Update AnnouncementRecipientData
		Set Complete = 1
		Where AnnouncementRef in(select Id from @annToMark where Complete is not null) and PersonRef = @personId

Go
