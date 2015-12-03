Create Procedure spSelectAnnouncementAddionalData @announcementId int, @ownerId int, @callerId int, @schoolId int
As
select top 1 * from vwPerson
where Id = @ownerId and (@schoolId is null or SchoolRef = @schoolId)

exec spGetAnnouncementsQnA @callerId, null, @announcementId, null, null, @schoolId

select aa.*
from AnnouncementApplication aa
where aa.AnnouncementRef = @announcementId and @announcementId is not null and aa.Active = 1

select * from AnnouncementStandard 
join [Standard] on [Standard].Id = AnnouncementStandard.StandardRef
where AnnouncementStandard.AnnouncementRef = @announcementId

Go

Create Procedure spGetLessonPlanDetails @lessonPlanId int, @callerId int, @callerRole int, @schoolId int
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
	Where Id = @lessonPlanId and exists(select * from ClassPerson where ClassPerson.PersonRef = @callerId and ClassPerson.ClassRef = vwLessonPlan.ClassRef)   
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
		
Go

Create Procedure spGetClassAnnouncementDetails @classAnnouncementId int, @callerId int, @callerRole int, @schoolId int
As

if @callerRole is null
select top 1 @callerRole = RoleRef from SchoolPerson where PersonRef = @callerId

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
	Where Id = @classAnnouncementId and exists(select * from ClassTeacher where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwClassAnnouncement.ClassRef)   
End
If @callerRole = 3
Begin
	Set @isOwner = 0
	Insert into @classAnn
	Select vwClassAnnouncement.*, @isOwner, @complete, @allCount
	From vwClassAnnouncement
	Where Id = @classAnnouncementId and exists(select * from ClassPerson where ClassPerson.PersonRef = @callerId and ClassPerson.ClassRef = vwClassAnnouncement.ClassRef)   
End

Declare @primaryTeacherId int, @classId int
Select @primaryTeacherId = t.PrimaryTeacherRef, @classId = t.ClassRef, @classAnnouncementId = t.Id From @classAnn t


Exec spSelectClassAnnouncement @classAnn
Exec spSelectAnnouncementAddionalData @classAnnouncementId, @primaryTeacherId, @callerId, @schoolId

Declare @teacherIds TIntId
Insert Into @teacherIds
Select PersonRef From ClassTeacher Where ClassRef = @classId


Select * From AnnouncementAttachment
Where AnnouncementRef = @classAnnouncementId
		and (@callerRole = 2 or (@callerRole = 3 and (PersonRef = @callerId or exists(select * from @teacherIds t where t.Id = PersonRef))))
Go

Create Procedure spGetAdminAnnouncementDetails @adminAnnouncementId int, @callerId int, @callerRole int
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


Select * From AnnouncementAttachment
Where AnnouncementRef = @adminAnnouncementId
		and (@callerRole = 10 or (@callerRole = 3 and (PersonRef = @callerId or PersonRef = AnnouncementAttachment.PersonRef)))


Select  
	AnnouncementGroup.AnnouncementRef as AdminAnnouncementRecipient_AnnouncementRef,
	AnnouncementGroup.GroupRef as AdminAnnouncementRecipient_GroupRef,
	[Group].Id as Group_Id,
	[Group].Name as Group_Name
From AnnouncementGroup
join [Group] on [Group].Id = AnnouncementGroup.GroupRef
Where AnnouncementRef = @adminAnnouncementId
Go


Drop Procedure spGetAnnouncementDetails
Go