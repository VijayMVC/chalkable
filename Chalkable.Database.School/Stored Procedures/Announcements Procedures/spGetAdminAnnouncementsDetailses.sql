Create Procedure [dbo].[spGetAdminAnnouncementsDetailses] 
	@adminAnnouncementIds TInt32 ReadOnly, 
	@callerId int, 
	@callerRole int
as


if @callerRole is null
	select top 1 @callerRole = RoleRef from SchoolPerson where PersonRef = @callerId

Declare @adminAnnouncements TAdminAnnouncement
Declare @allCount int
Declare @isOwner bit = 1

If @callerRole = 10
Begin
	insert into @adminAnnouncements
	Select 
		vwAA.*, 
		@isOwner, 
		cast((case when adr.Complete is null then 0 else adr.Complete end) as bit), 
		@allCount as AllCount
	From 
		vwAdminAnnouncement vwAA left join AnnouncementRecipientData adr
			on vwAA.Id = adr.AnnouncementRef and adr.PersonRef = @callerId
	Where 
		Id in(select * from @adminAnnouncementIds) 
		and vwAA.AdminRef = @callerId
End
If @callerRole = 3
Begin
	Set @isOwner = 0
	Insert into @adminAnnouncements
	Select 
		vwAA.*, 
		@isOwner, 
		cast((case when adr.Complete is null then 0 else adr.Complete end) as bit), 
		@allCount as AllCount
	From 
		vwAdminAnnouncement vwAA left join AnnouncementRecipientData adr
			on vwAA.Id = adr.AnnouncementRef and adr.PersonRef = @callerId
	Where Id in(select * from @adminAnnouncementIds)
		and exists(
				select * from 
					AnnouncementGroup join StudentGroup 
						on StudentGroup.GroupRef = AnnouncementGroup.GroupRef
				where 
					AnnouncementGroup.AnnouncementRef in(select * from @adminAnnouncementIds)
					and StudentGroup.StudentRef = @callerId
			)			
End


--This procedure was changed to make ordered selects
--Here we don't need to order, so we create fake table
Declare @emptyFake TAnnouncementOrder
Exec spSelectAdminAnnoucnement @adminAnnouncements, @emptyFake, 0, 0, 0

select vwPerson.* from vwPerson
join @adminAnnouncements ann on ann.AdminRef = vwPerson.Id

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
Where 
	AnnouncementRef in(Select Id From @adminAnnouncements)
	and (@callerRole = 1 
		 or @callerId = AnswererId 
		 or @callerId = AskerId 
		 or (ClassRef is not null and AnsweredTime is not null
						and exists(Select * From ClassPerson cp Where cp.ClassRef = ClassRef and @callerId = cp.PersonRef))
		 or (ClassRef is not null and exists(Select * From ClassTeacher ct Where ct.ClassRef = ClassRef and @callerId = ct.PersonRef))
		 or (AdminRef is not null and AdminRef = @callerId))
Order By QuestionTime

-------------------------------------------------------------------------------------------------

Select * From 
	vwAnnouncementAssignedAttribute attr
Where AnnouncementAssignedAttribute_AnnouncementRef in(Select Id From @adminAnnouncements) 

Select aa.* From 
	AnnouncementApplication aa
Where 
	aa.AnnouncementRef in(Select Id From @adminAnnouncements) 
	and aa.Active = 1

Select * From 
	AnnouncementStandard join [Standard] 
		on [Standard].Id = AnnouncementStandard.StandardRef
Where AnnouncementStandard.AnnouncementRef in(Select Id From @adminAnnouncements) 

Select * From vwAnnouncementAttachment
Where AnnouncementAttachment_AnnouncementRef in(select Id from @adminAnnouncements)
		and (@callerRole = 10 or (@callerRole = 3 and (Attachment_PersonRef = @callerId or Attachment_PersonRef in(select AdminRef from @adminAnnouncements))))

Select  
	AnnouncementGroup.AnnouncementRef as AnnouncementGroup_AnnouncementRef,
	AnnouncementGroup.GroupRef as AnnouncementGroup_GroupRef,
	[Group].Id as Group_Id,
	[Group].Name as Group_Name,
	[Student].Id as Student_Id,
	[Student].FirstName as Student_FirstName,
	[Student].LastName as Student_LastName,
	[Student].Gender as Student_Gender
From AnnouncementGroup
join [Group] 
	on [Group].Id = AnnouncementGroup.GroupRef
join StudentGroup
		on StudentGroup.GroupRef = [Group].Id
	join Student
		on Student.Id = StudentGroup.StudentRef
Where AnnouncementRef in(select Id from @adminAnnouncements)

Select
	AdminAnnouncementStudent.AdminAnnouncementRef as AdminAnnouncementStudent_AdminAnnouncementRef,
	AdminAnnouncementStudent.StudentRef as AdminAnnouncementStudent_StudentRef,
	Student.Id as Student_Id,
	Student.FirstName as Student_FirstName,
	Student.LastName as Student_LastName,
	Student.BirthDate as Student_BirthDate,
	Student.Gender as Student_Gender,
	Student.HasMedicalAlert as Student_HasMedicalAlert,
	Student.IsAllowedInetAccess as Student_IsAllowedInetAccess,
	Student.SpecialInstructions as Student_SpecialInstructions,
	Student.SpEdStatus as Student_SpEdStatus,
	Student.UserId as Student_UserId
From AdminAnnouncementStudent
join Student
	on AdminAnnouncementStudent.StudentRef = Student.Id
Where AdminAnnouncementStudent.AdminAnnouncementRef in (select Id from @adminAnnouncements)