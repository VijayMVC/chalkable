



CREATE Procedure [dbo].[spGetLessonPlanDetails] @lessonPlanId int, @callerId int, @callerRole int, @schoolYearId int
As

Declare @lessonPlan TLessonPlan
Declare @allCount int = 1
Declare @isOwner bit = 1
Declare @complete bit =  cast((Select Top 1 Complete from AnnouncementRecipientData where AnnouncementRef = @lessonPlanId and PersonRef = @callerId) as bit)

If @complete is null 
	Set @complete = 0

If @callerRole = 2 or @callerRole = 10
Begin
	insert into @lessonPlan
	Select 
		vwLessonPlan.*, @isOwner, @complete, @allCount
	From vwLessonPlan
	Where Id = @lessonPlanId and SchoolYearRef  = @schoolYearId
			and exists(select * from ClassTeacher where ClassTeacher.PersonRef = @callerId and (vwLessonPlan.ClassRef is null or ClassTeacher.ClassRef = vwLessonPlan.ClassRef))   
End
If @callerRole = 3
Begin
	Set @isOwner = 0
	Insert into @lessonPlan
	Select vwLessonPlan.*, @isOwner, @complete, @allCount
	From vwLessonPlan
	Where Id = @lessonPlanId and VisibleForStudent = 1 and SchoolYearRef  = @schoolYearId 
			and exists(select * from ClassPerson where ClassPerson.PersonRef = @callerId and ClassPerson.ClassRef = vwLessonPlan.ClassRef)   
End

Declare @primaryTeacherId int, @classId int
Select @primaryTeacherId = t.PrimaryTeacherRef, @classId = t.ClassRef, @lessonPlanId = t.Id From @lessonPlan t


Declare @schoolId int  = (Select SchoolRef from SchoolYear Where Id = @schoolYearId)
exec spSelectLessonPlans @lessonPlan
exec spSelectAnnouncementAddionalData @lessonPlanId, @primaryTeacherId, @callerId, @schoolId

Declare @teacherIds TInt32
Insert Into @teacherIds
Select PersonRef From ClassTeacher Where ClassRef = @classId


select * from vwAnnouncementAttachment
where AnnouncementAttachment_AnnouncementRef = @lessonPlanId
	  and (@callerRole = 2 or @callerRole = 10 or (@callerRole = 3 and (Attachment_PersonRef = @callerId or exists(select * from @teacherIds t where t.value = Attachment_PersonRef))))