CREATE PROCEDURE [dbo].[spGetSupplementalAnnouncementDetails]
	@supplementalAnnouncementPlanId int, @callerId int, @callerRole int, @schoolYearId int
AS
	Declare @supplementalAnnouncement TSupplementalAnnouncement
	Declare @allCount int = 1
	Declare @isOwner bit = 1

If @callerRole = 2
Begin
	insert into @supplementalAnnouncement
	Select 
		vwLessonPlan.*, @isOwner, @allCount
	From vwLessonPlan
	Where Id = @supplementalAnnouncementPlanId and SchoolYearRef  = @schoolYearId
			and exists(select * from ClassTeacher where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef)   
End
If @callerRole = 3
Begin
	Set @isOwner = 0
	Insert into @supplementalAnnouncement
	Select vwLessonPlan.*, @isOwner, @allCount
	From vwLessonPlan
	Where Id = @supplementalAnnouncementPlanId and VisibleForStudent = 1 and SchoolYearRef  = @schoolYearId 
			and exists(select * from ClassPerson where ClassPerson.PersonRef = @callerId and ClassPerson.ClassRef = vwLessonPlan.ClassRef)   
End

Declare @primaryTeacherId int, @classId int
Select @primaryTeacherId = t.PrimaryTeacherRef, @classId = t.ClassRef, @supplementalAnnouncement = t.Id From @supplementalAnnouncement t


Declare @schoolId int  = (Select SchoolRef from SchoolYear Where Id = @schoolYearId)
exec spSelectLessonPlans @supplementalAnnouncement
exec spSelectAnnouncementAddionalData @supplementalAnnouncement, @primaryTeacherId, @callerId, @schoolId

Declare @teacherIds TInt32
Insert Into @teacherIds
Select PersonRef From ClassTeacher Where ClassRef = @classId


select * from vwAnnouncementAttachment
where AnnouncementAttachment_AnnouncementRef = @supplementalAnnouncement
	  and (@callerRole = 2 or (@callerRole = 3 and (Attachment_PersonRef = @callerId or exists(select * from @teacherIds t where t.value = Attachment_PersonRef))))