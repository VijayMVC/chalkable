﻿CREATE Procedure [dbo].[spGetAdminAnnouncementDetails] @adminAnnouncementId int, @callerId int, @callerRole int
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


Exec spSelectAdminAnnoucnement @adminAnnouncement
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