Create Procedure [dbo].[spGetListOfLessonPlansDetails]
	@lessonPlanIds TInt32 readonly,
	@callerId int,
	@callerRole int,
	@schoolYearId int,
	@onlyOwner bit
As

Declare @TEACHER_ROLE_ID int = 2,
		@STUDENT_ROLE_ID int = 3,
		@ADMIN_ROLE_ID int = 10

Declare @lessonPlans TLessonPlan

Insert Into @lessonPlans
Select distinct
	vwLP.*,
	cast((Case When exists(Select * From ClassTeacher CT where CT.PersonRef = @callerId and CT.ClassRef = vwLP.ClassRef) then 1 else 0 End) as Bit),
	cast((Case When ard.Complete is null Then 0 Else 1 End) as Bit) as Complete, 
	0 as AllCount 
From 
	vwLessonPlan vwLP
	left join AnnouncementRecipientData ard
		on ard.AnnouncementRef = vwLP.Id
Where
	Id in(Select * From @lessonPlanIds)
	and (@callerRole = @ADMIN_ROLE_ID or  SchoolYearRef = @schoolYearId)
	and (
		 @onlyOwner = 0
		 or (@callerRole = @ADMIN_ROLE_ID or @callerRole = @TEACHER_ROLE_ID)  AND (exists(Select * From ClassTeacher Where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwLP.ClassRef) or InGallery = 1)
		 or @callerRole = @STUDENT_ROLE_ID And exists(Select * From ClassPerson Where ClassPerson.PersonRef = @callerId and ClassPerson.ClassRef = vwLP.ClassRef)
			And vwLP.VisibleForStudent = 1
		)

Declare @count int = (Select count(*) From @lessonPlans)

Update @lessonPlans
Set AllCount = @count

Exec spSelectLessonPlans @lessonPlans

-------------------------------------------------------------------------------------------------


Declare @lpSchool Table (id int, classId int, schoolId int)

Insert Into @lpSchool
Select 
	lp.Id, 
	lp.ClassRef, 
	SchoolYear.SchoolRef 
From 
	@lessonPlans lp
Join 
	SchoolYear on SchoolYear.Id = lp.SchoolYearRef


Declare @teacherIds Table (id int, schoolId int)
Insert Into @teacherIds
Select 
	ClassTeacher.PersonRef,
	lps.schoolId
From 
	ClassTeacher
Join  
	@lpSchool lps on lps.classId = ClassTeacher.ClassRef



Select vwPerson.* From vwPerson
Join @teacherIds t on t.id = vwPerson.Id
Where vwPerson.SchoolRef = t.schoolId

-------------------------------------------------------------------------------------------------

Select Distinct
	vwAnnouncementQnA.Id,
	vwAnnouncementQnA.AnsweredTime,
	vwAnnouncementQnA.QuestionTime,
	vwAnnouncementQnA.Question,
	vwAnnouncementQnA.Answer,
	vwAnnouncementQnA.AnnouncementRef,
	vwAnnouncementQnA.ClassRef,
	vwAnnouncementQnA.AdminRef,
	vwAnnouncementQnA.[State],
	vwAnnouncementQnA.AskerId,
	vwAnnouncementQnA.AskerFirstName,
	vwAnnouncementQnA.AskerLastName,
	vwAnnouncementQnA.AskerGender,
	vwAnnouncementQnA.AskerRoleRef,
	vwAnnouncementQnA.AnswererId,
	vwAnnouncementQnA.AnswererFirstName,
	vwAnnouncementQnA.AnswererLastName,
	vwAnnouncementQnA.AnswererGender,
	vwAnnouncementQnA.AnswererRoleRef,
	cast((Case When @callerId = vwAnnouncementQnA.AskerId Then 1 Else 0 End) as Bit) as IsOwner
From 
	vwAnnouncementQnA
Join 
	@lpSchool lps on lps.id = vwAnnouncementQnA.AnnouncementRef
Where 
	AnnouncementRef in(Select Id From @lessonPlans)
	and (@callerRole = 1 
		 or @callerId = AnswererId 
		 or @callerId = AskerId 
		 or (ClassRef is not null and AnsweredTime is not null
						and exists(Select * From ClassPerson cp Where cp.ClassRef = ClassRef and @callerId = cp.PersonRef))
		 or (ClassRef is not null and exists(Select * From ClassTeacher ct Where ct.ClassRef = ClassRef and @callerId = ct.PersonRef))
		 or (AdminRef is not null and AdminRef = @callerId))
	and (AdminRef is not null or (AskerSchoolRef = lps.schoolId and (AnswererSchoolRef is null or AnswererSchoolRef = lps.schoolId)))
Order By QuestionTime

-------------------------------------------------------------------------------------------------

Select * From 
	vwAnnouncementAssignedAttribute attr
Where AnnouncementAssignedAttribute_AnnouncementRef in(Select Id From @lessonPlans) 

Select aa.* From 
	AnnouncementApplication aa
Where 
	aa.AnnouncementRef in(Select Id From @lessonPlans) 
	and aa.Active = 1

Select * From 
	AnnouncementStandard join [Standard] 
		on [Standard].Id = AnnouncementStandard.StandardRef
Where AnnouncementStandard.AnnouncementRef in(Select Id From @lessonPlans) 

Select * From vwAnnouncementAttachment
Where AnnouncementAttachment_AnnouncementRef in(Select Id From @lessonPlans)
		and (@callerRole = @ADMIN_ROLE_ID 
		or @callerRole = @TEACHER_ROLE_ID 
		or @onlyOwner = 0
		or (@callerRole = @STUDENT_ROLE_ID 
		and (Attachment_PersonRef = @callerId 
		or exists(Select * From @teacherIds t Where t.id = Attachment_PersonRef))))
