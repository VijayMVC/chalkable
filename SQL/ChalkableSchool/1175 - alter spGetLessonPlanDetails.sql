Alter Procedure [dbo].[spGetLessonPlanDetails] @lessonPlanId int, @callerId int, @callerRole int, @schoolId int
As

if @callerRole is null
	select top 1 @callerRole = RoleRef from SchoolPerson where PersonRef = @callerId

Declare @lessonPlan TLessonPlan
Declare @allCount int = 1
Declare @isOwner bit = 1
Declare @complete bit =  cast((Select Top 1 Complete from AnnouncementRecipientData where AnnouncementRef = @lessonPlanId and PersonRef = @callerId) as bit)

If @complete is null 
	Set @complete = 0

If @callerRole = 2
Begin
	insert into @lessonPlan
	Select 
		vwLessonPlan.*, @isOwner, @complete, @allCount
	From vwLessonPlan
	Where Id = @lessonPlanId and exists(select * from ClassTeacher where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef)   
End
If @callerRole = 3
Begin
	Set @isOwner = 0
	Insert into @lessonPlan
	Select vwLessonPlan.*, @isOwner, @complete, @allCount
	From vwLessonPlan
	Where Id = @lessonPlanId and VisibleForStudent = 1 and exists(select * from ClassPerson where ClassPerson.PersonRef = @callerId and ClassPerson.ClassRef = vwLessonPlan.ClassRef)   
End

Declare @primaryTeacherId int, @classId int
Select @primaryTeacherId = t.PrimaryTeacherRef, @classId = t.ClassRef, @lessonPlanId = t.Id From @lessonPlan t


exec spSelectLessonPlans @lessonPlan
exec spSelectAnnouncementAddionalData @lessonPlanId, @primaryTeacherId, @callerId, @schoolId

Declare @teacherIds TIntId
Insert Into @teacherIds
Select PersonRef From ClassTeacher Where ClassRef = @classId


select * from AnnouncementAttachment
where AnnouncementRef = @lessonPlanId
	  and (@callerRole = 2 or (@callerRole = 3 and (PersonRef = @callerId or exists(select * from @teacherIds t where t.Id = PersonRef))))
		

GO


select * from vwLessonPlan
where Title = 'my hidden lesson plan'
