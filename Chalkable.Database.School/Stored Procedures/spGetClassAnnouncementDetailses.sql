


Create Procedure [dbo].[spGetClassAnnouncementDetailses]
	@classAnnouncementIds TInt32 readonly,
	@callerId int,
	@callerRole int,
	@schoolYearId int
As

Declare @teacherRoleId int = 2,
		@isOwner bit,
		@schoolId int

Set @isOwner = cast((Case When @callerRole = @teacherRoleId Then 1 Else 0 End) as Bit)

Select @schoolId = SchoolRef From SchoolYear Where Id = @schoolYearId

Declare @classAnns TClassAnnouncement

Insert Into @classAnns
Select 
	vwCA.*,
	@isOwner,
	0, 
	0 as AllCount 
From 
	vwClassAnnouncement vwCA
Where
	Id in(Select * From @classAnnouncementIds)
	and SchoolYearRef = @schoolYearId
	and (@callerRole=2 AND exists(Select * From ClassTeacher Where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwCA.ClassRef)
		 or @callerRole=3 And exists(Select * From ClassPerson Where ClassPerson.PersonRef = @callerId and ClassPerson.ClassRef = vwCA.ClassRef))

Declare @count int = (Select count(*) From @classAnns)

Update @classAnns
Set AllCount = @count

Exec spSelectClassAnnouncement @classAnns

-------------------------------------------------------------------------------------------------

Declare @teacherIds TInt32
Insert Into @teacherIds
Select PersonRef From ClassTeacher 
Where ClassRef in (Select ca.ClassRef From @classAnns ca)


select vwPerson.* from vwPerson
join @teacherIds tId on tId.value = vwPerson.Id
where (@schoolId is null or vwPerson.SchoolRef = @schoolId)

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
Where 
	AnnouncementRef in(Select Id From @classAnns)
	and (@callerRole = 1 
		 or @callerId = AnswererId 
		 or @callerId = AskerId 
		 or (ClassRef is not null and AnsweredTime is not null
						and exists(Select * From ClassPerson cp Where cp.ClassRef = ClassRef and @callerId = cp.PersonRef))
		 or (ClassRef is not null and exists(Select * From ClassTeacher ct Where ct.ClassRef = ClassRef and @callerId = ct.PersonRef))
		 or (AdminRef is not null and AdminRef = @callerId))
	and (AdminRef is not null or (AskerSchoolRef = @schoolId and (AnswererSchoolRef is null or AnswererSchoolRef = @schoolId)))
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
		and (@callerRole = 2 or (@callerRole = 3 and (Attachment_PersonRef = @callerId or exists(Select * From @teacherIds t Where t.value = Attachment_PersonRef))))