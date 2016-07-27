Create Procedure [dbo].[spGetListOfClassAnnouncementDetails]
	@classAnnouncementIds TInt32 readonly,
	@callerId int,
	@callerRole int,
	@schoolYearId int,
	@onlyOwner bit
As

Declare @TEACHER_ROLE_ID int = 2,
		@STUDENT_ROLE_ID int = 3,
		@ADMIN_ROLE_ID int = 10

Declare @classAnns TClassAnnouncementComplex

Insert Into @classAnns
Select 
	vwCA.*,
	cast((Case When exists(Select * From ClassTeacher CT where CT.PersonRef = @callerId and CT.ClassRef = vwCA.ClassRef) then 1 else 0 End) as Bit),
	0, 
	0 as AllCount 
From 
	vwClassAnnouncement vwCA
Where
	Id in (Select * From @classAnnouncementIds)
	and (@callerRole = @ADMIN_ROLE_ID or SchoolYearRef = @schoolYearId)
	and (
			 @onlyOwner = 0 
			 or (@callerRole = @ADMIN_ROLE_ID or @callerRole = @TEACHER_ROLE_ID) AND exists(Select * From ClassTeacher Where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwCA.ClassRef)
			 or @callerRole= @STUDENT_ROLE_ID AND exists(Select * From ClassPerson Where ClassPerson.PersonRef = @callerId and ClassPerson.ClassRef = vwCA.ClassRef)
		 )

Declare @count int = (Select count(*) From @classAnns)

Update @classAnns
Set AllCount = @count

Exec spSelectClassAnnouncement @classAnns

-------------------------------------------------------------------------------------------------

Declare @classAnnsSchool Table (id int, classId int, schoolId int)

Insert Into @classAnnsSchool
Select 
	ca.Id, 
	ca.ClassRef, 
	SchoolYear.SchoolRef 
From 
	@classAnns ca
Join 
	SchoolYear on SchoolYear.Id = ca.SchoolYearRef



Declare @teacherIds Table (id int, schoolId int)
Insert Into @teacherIds
Select 
	ClassTeacher.PersonRef,
	cas.schoolId
From 
	ClassTeacher
Join  
	@classAnnsSchool cas on cas.classId = ClassTeacher.ClassRef


Select vwPerson.* From vwPerson
Join @teacherIds t on t.id = vwPerson.Id
Where vwPerson.SchoolRef = t.schoolId

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
Join @classAnnsSchool cas on cas.id = vwAnnouncementQnA.AnnouncementRef
Where 
	 (@callerRole = 1 or @callerRole = @ADMIN_ROLE_ID
		 or @callerId = AnswererId 
		 or @callerId = AskerId 
		 or (ClassRef is not null and AnsweredTime is not null
						and exists(Select * From ClassPerson cp Where cp.ClassRef = ClassRef and @callerId = cp.PersonRef))
		 or (ClassRef is not null and exists(Select * From ClassTeacher ct Where ct.ClassRef = ClassRef and @callerId = ct.PersonRef))
		 or (AdminRef is not null and AdminRef = @callerId))
	and (AdminRef is not null or (AskerSchoolRef = cas.schoolId and (AnswererSchoolRef is null or AnswererSchoolRef = cas.schoolId)))
Order By QuestionTime

-------------------------------------------------------------------------------------------------

Select * From 
	vwAnnouncementAssignedAttribute attr
Where AnnouncementAssignedAttribute_AnnouncementRef in(Select * From @classAnnouncementIds) 

Select aa.* From 
	AnnouncementApplication aa
Where 
	aa.AnnouncementRef in(Select * From @classAnnouncementIds) 
	and aa.Active = 1

Select * From 
	AnnouncementStandard join [Standard] 
		on [Standard].Id = AnnouncementStandard.StandardRef
Where AnnouncementStandard.AnnouncementRef in(Select * From @classAnnouncementIds) 

Select * From vwAnnouncementAttachment
Where AnnouncementAttachment_AnnouncementRef in(Select * From @classAnnouncementIds)
		and (@callerRole = @TEACHER_ROLE_ID 
			 or @callerRole = @ADMIN_ROLE_ID 
			 or @onlyOwner = 0
			 or (@callerRole = @STUDENT_ROLE_ID 
				 and (Attachment_PersonRef = @callerId or exists(Select * From @teacherIds t Where t.id = Attachment_PersonRef))
			 ))


