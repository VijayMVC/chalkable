Create View vwAnnouncementAttachment
As
Select  
	AnnouncementAttachment.Id as AnnouncementAttachment_Id,
	AnnouncementAttachment.AnnouncementRef as AnnouncementAttachment_AnnouncementRef,
	AnnouncementAttachment.AttachedDate as AnnouncementAttachment_AttachedDate,
	AnnouncementAttachment.[Order] as AnnouncementAttachment_Order,
	AnnouncementAttachment.AttachmentRef as AnnouncementAttachment_AttachmentRef,
	Attachment.Id as Attachment_Id,
	Attachment.PersonRef as Attachment_PersonRef,
	Attachment.Name as Attachment_Name,
	Attachment.MimeType as Attachment_MimeType,
	Attachment.UploadedDate as Attachment_UploadedDate,
	Attachment.LastAttachedDate as Attachment_LastAttachedDate,
	Attachment.Uuid as Attachment_Uuid,
	Attachment.SisAttachmentId as Attachment_SisAttachmentId,
	Attachment.RelativeBlobAddress as Attachment_RelativeBlobAddress
From
	AnnouncementAttachment
Join Attachment on Attachment.Id = AnnouncementAttachment.AttachmentRef 
Go

Create View vwAnnouncementAssignedAttribute
As
Select
	 AnnouncementAssignedAttribute.Id As AnnouncementAssignedAttribute_Id,
	 AnnouncementAssignedAttribute.Name As AnnouncementAssignedAttribute_Name,
	 AnnouncementAssignedAttribute.[Text] As AnnouncementAssignedAttribute_Text,
	 AnnouncementAssignedAttribute.AnnouncementRef As AnnouncementAssignedAttribute_AnnouncementRef,
	 AnnouncementAssignedAttribute.AttributeTypeId As AnnouncementAssignedAttribute_AttributeTypeId,
	 AnnouncementAssignedAttribute.VisibleForStudents As AnnouncementAssignedAttribute_VisibleForStudent,
	 AnnouncementAssignedAttribute.SisActivityAssignedAttributeId As AnnouncementAssignedAttribute_SisActivityAssignedAttributeId,
	 AnnouncementAssignedAttribute.AttachmentRef As AnnouncementAssignedAttribute_AttachmentRef,
	 
	 Attachment.Id As Attachment_Id,
	 Attachment.PersonRef As Attachment_PersonRef,
	 Attachment.Name As Attachment_Name,
	 Attachment.MimeType As Attachment_MimeType,
	 Attachment.UploadedDate As Attachment_UploadedDate,
	 Attachment.LastAttachedDate As Attachment_LastAttachedDate,
	 Attachment.Uuid As Attachment_Uuid,
	 Attachment.SisAttachmentId As Attachment_SisAttachmentId,
	 Attachment.RelativeBlobAddress As Attachment_RelativeBlobAddress
From 
	AnnouncementAssignedAttribute
Left Join Attachment On Attachment.Id = AnnouncementAssignedAttribute.AttachmentRef
Go

Alter Procedure [dbo].[spSelectAnnouncementAddionalData] @announcementId int, @ownerId int, @callerId int, @schoolId int
As
select top 1 * from vwPerson
where Id = @ownerId and (@schoolId is null or SchoolRef = @schoolId)

exec spGetAnnouncementsQnA @callerId, null, @announcementId, null, null, @schoolId

select * from vwAnnouncementAssignedAttribute attr
where AnnouncementAssignedAttribute_AnnouncementRef = @announcementId 

select aa.*
from AnnouncementApplication aa
where aa.AnnouncementRef = @announcementId and @announcementId is not null and aa.Active = 1

select * from AnnouncementStandard 
join [Standard] on [Standard].Id = AnnouncementStandard.StandardRef
where AnnouncementStandard.AnnouncementRef = @announcementId
GO




Alter Procedure [dbo].[spGetLessonPlanDetails] @lessonPlanId int, @callerId int, @callerRole int, @schoolYearId int
As

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
	Where Id = @lessonPlanId and SchoolYearRef  = @schoolYearId
			and exists(select * from ClassTeacher where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef)   
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
	  and (@callerRole = 2 or (@callerRole = 3 and (Attachment_PersonRef = @callerId or exists(select * from @teacherIds t where t.value = Attachment_PersonRef))))
GO



Alter Procedure [dbo].[spGetAdminAnnouncementDetails] @adminAnnouncementId int, @callerId int, @callerRole int
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


GO




Alter Procedure [dbo].[spGetClassAnnouncementDetails] @classAnnouncementId int, @callerId int, @callerRole int, @schoolYearId int
As

Declare @classAnn TClassAnnouncement
Declare @allCount int = 1
Declare @isOwner bit = 1
Declare @complete bit =  cast((Select Top 1 Complete from AnnouncementRecipientData where AnnouncementRef = @classAnnouncementId and PersonRef = @callerId) as bit)

If @complete is null 
	Set @complete = 0

If @callerRole = 2
Begin
	insert into @classAnn
	Select 
		vwClassAnnouncement.*, @isOwner, @complete, @allCount
	From vwClassAnnouncement
	Where Id = @classAnnouncementId and SchoolYearRef = @schoolYearId
		  and exists(select * from ClassTeacher where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwClassAnnouncement.ClassRef)   
End
If @callerRole = 3
Begin
	Set @isOwner = 0
	Insert into @classAnn
	Select vwClassAnnouncement.*, @isOwner, @complete, @allCount
	From vwClassAnnouncement
	Where Id = @classAnnouncementId and SchoolYearRef = @schoolYearId
		  and exists(select * from ClassPerson where ClassPerson.PersonRef = @callerId and ClassPerson.ClassRef = vwClassAnnouncement.ClassRef)   
End

Declare @primaryTeacherId int, @classId int
Select @primaryTeacherId = t.PrimaryTeacherRef, @classId = t.ClassRef, @classAnnouncementId = t.Id From @classAnn t


Declare @schoolId int = cast((Select Top 1 SchoolRef From SchoolYear Where Id = @schoolYearId) as int)
Exec spSelectClassAnnouncement @classAnn
Exec spSelectAnnouncementAddionalData @classAnnouncementId, @primaryTeacherId, @callerId, @schoolId

Declare @teacherIds TInt32
Insert Into @teacherIds
Select PersonRef From ClassTeacher Where ClassRef = @classId


Select * From vwAnnouncementAttachment
Where AnnouncementAttachment_AnnouncementRef = @classAnnouncementId
		and (@callerRole = 2 or (@callerRole = 3 and (Attachment_PersonRef = @callerId or exists(select * from @teacherIds t where t.value = Attachment_PersonRef))))


GO