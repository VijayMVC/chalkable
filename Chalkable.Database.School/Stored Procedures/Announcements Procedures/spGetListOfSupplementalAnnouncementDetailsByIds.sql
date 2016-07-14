CREATE PROCEDURE [dbo].[spGetListOfSupplementalAnnouncementDetailsByIds]
	@announcementIds TInt32 Readonly,
	@callerId int,
	@callerRole int,
	@schoolYearId int
AS

Declare @TEACHER_ROLE_ID int = 2,
		@STUDENT_ROLE_ID int = 3,
		@ADMIN_ROLE_ID int = 10

Declare @supplementalAnnouncements TSupplementalAnnouncement;
Declare @totalCount int;
Declare @isOwner int = 1;

Insert Into @supplementalAnnouncements
Select distinct
	vwSA.*,
	cast((Case When exists(Select * From ClassTeacher CT where CT.PersonRef = @callerId and CT.ClassRef = vwSA.ClassRef) then 1 else 0 End) as Bit),
	cast((Case When ard.Complete is null Then 0 Else 1 End) as Bit) as Complete, 
	0 as AllCount 
From 
	vwSupplementalAnnouncement vwSA
	left join AnnouncementRecipientData ard
		on ard.AnnouncementRef = vwSA.Id
Where
	Id in(Select * From @announcementIds)
	and (@callerRole = @ADMIN_ROLE_ID or  SchoolYearRef = @schoolYearId)
	and (
		 (@callerRole = @ADMIN_ROLE_ID or @callerRole = @TEACHER_ROLE_ID)  AND exists(Select * From ClassTeacher Where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwSA.ClassRef)
		 or @callerRole = @STUDENT_ROLE_ID And exists(Select * From SupplementalAnnouncementRecipient Where SupplementalAnnouncementRecipient.StudentRef = @callerId and SupplementalAnnouncementRef = vwSA.Id)
			And vwSA.VisibleForStudent = 1
		)


Exec spSelectSupplementalAnnouncements @supplementalAnnouncements

Declare @saSchool Table (id int, classId int, schoolId int)

Insert Into @saSchool
Select 
	sa.Id,
	sa.ClassRef,
	SchoolYear.SchoolRef
From
	@supplementalAnnouncements sa
	Join SchoolYear on SchoolYear.Id = sa.SchoolYearRef

Declare @teacherIds Table(id int, schoolId int)
Insert Into @teacherIds
Select
	ClassTeacher.PersonRef,
	sas.schoolId
From
	ClassTeacher
	Join @saSchool as sas on sas.classId = ClassTeacher.ClassRef

Select vwPerson.* 
From 
	vwPerson
	Join @teacherIds teacherIds
		On teacherIds.Id = vwPerson.Id
Where vwPerson.SchoolRef = teacherIds.schoolId

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
	Join @saSchool sas on sas.id = vwAnnouncementQnA.AnnouncementRef
Where 
	AnnouncementRef in(Select Id From @supplementalAnnouncements)
	and (@callerRole = 1 
		 or @callerId = AnswererId 
		 or @callerId = AskerId 
		 or (ClassRef is not null and AnsweredTime is not null
						and exists(Select * From ClassPerson cp Where cp.ClassRef = ClassRef and @callerId = cp.PersonRef))
		 or (ClassRef is not null and exists(Select * From ClassTeacher ct Where ct.ClassRef = ClassRef and @callerId = ct.PersonRef))
		 or (AdminRef is not null and AdminRef = @callerId))
	and (AdminRef is not null or (AskerSchoolRef = sas.schoolId and (AnswererSchoolRef is null or AnswererSchoolRef = sas.schoolId)))
Order By QuestionTime

Select * From 
	vwAnnouncementAssignedAttribute attr
Where AnnouncementAssignedAttribute_AnnouncementRef in(Select Id From @supplementalAnnouncements) 

Select aa.* From 
	AnnouncementApplication aa
Where 
	aa.AnnouncementRef in(Select Id From @supplementalAnnouncements) 
	and aa.Active = 1

Select * From 
	AnnouncementStandard join [Standard] 
		on [Standard].Id = AnnouncementStandard.StandardRef
Where AnnouncementStandard.AnnouncementRef in(Select Id From @supplementalAnnouncements) 

Select * From vwAnnouncementAttachment
Where AnnouncementAttachment_AnnouncementRef in(Select Id From @supplementalAnnouncements)
		and (@callerRole = @TEACHER_ROLE_ID or 
		(@callerRole = @STUDENT_ROLE_ID and (Attachment_PersonRef = @callerId or exists(Select * From @teacherIds t Where t.id = Attachment_PersonRef))))


