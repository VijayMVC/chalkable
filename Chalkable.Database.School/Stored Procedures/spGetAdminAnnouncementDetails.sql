CREATE Procedure [dbo].[spGetAdminAnnouncementDetails] @adminAnnouncementId int, @callerId int, @callerRole int
as
	
if @callerRole is null
select top 1 @callerRole = RoleRef from SchoolPerson where PersonRef = @callerId

Declare @adminAnnouncement TAdminAnnouncement
Declare @allCount int = 1
Declare @isOwner bit = 1
Declare @complete bit =  cast((Select Top 1 Complete from AnnouncementRecipientData where AnnouncementRef = @adminAnnouncementId and PersonRef = @callerId) as bit)

If @complete is null 
	Set @complete = 0

If @callerRole = 10
Begin
	insert into @adminAnnouncement
	Select 
		vwAdminAnnouncement.*, @isOwner, @complete, @allCount
	From vwAdminAnnouncement
	Where Id = @adminAnnouncementId and  vwAdminAnnouncement.AdminRef = @callerId
End
If @callerRole = 3
Begin
	Set @isOwner = 0
	Insert into @adminAnnouncement
	Select vwAdminAnnouncement.*, @isOwner, @complete, @allCount
	From vwAdminAnnouncement
	Where Id = @adminAnnouncementId 
		and exists(
				select * from AnnouncementGroup 
				join StudentGroup on StudentGroup.GroupRef = AnnouncementGroup.GroupRef
				where AnnouncementGroup.AnnouncementRef = @adminAnnouncementId and StudentGroup.StudentRef = @callerId
			)
				
End

Declare @adminId int
Select @adminId = t.AdminRef, @adminAnnouncementId = t.Id From @adminAnnouncement t

--This procedure was changed to make ordered selects
--Here we don't need to order, so we create fake table
Declare @emptyFake TAnnouncementOrder
Exec spSelectAdminAnnoucnement @adminAnnouncement, @emptyFake, 0, 0, 0
Exec spSelectAnnouncementAddionalData @adminAnnouncementId, @adminId, @callerId, null


Select * From vwAnnouncementAttachment
Where AnnouncementAttachment_AnnouncementRef = @adminAnnouncementId
		and (@callerRole = 10 or (@callerRole = 3 and (Attachment_PersonRef = @callerId or Attachment_PersonRef = @adminId)))

Select  
	AnnouncementGroup.AnnouncementRef as AnnouncementGroup_AnnouncementRef,
	AnnouncementGroup.GroupRef as AnnouncementGroup_GroupRef,
	[Group].Id as Group_Id,
	[Group].Name as Group_Name
From AnnouncementGroup
join [Group] on [Group].Id = AnnouncementGroup.GroupRef
Where AnnouncementRef = @adminAnnouncementId

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
Where AdminAnnouncementStudent.AdminAnnouncementRef = @adminAnnouncementId