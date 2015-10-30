Alter Procedure [dbo].[spGetAnnouncementDetails] @id int, @callerId int, @callerRole int, @schoolId int
as

if @callerRole is null
select top 1 @callerRole = RoleRef from SchoolPerson where PersonRef = @callerId

declare @announcementTb table
(
	Id int not null,
	Created dateTime2 not null,
	Expires dateTime2 not null,
	[State] int not null,
	[Order] int not null,
	Content nvarchar(max),
	[Subject] nvarchar(max),
	[Title] nvarchar(max),
	GradingStyle int not null,
	Dropped bit not null,
	ClassAnnouncementTypeRef int null,
	SchoolRef int null,
	SisActivityId int null,
    MaxScore decimal null,
    WeightAddition decimal(9,6) null,
    WeightMultiplier decimal(9,6) null,
    MayBeDropped bit,
	VisibleForStudent bit,
	AdminRef int,
	ClassRef int,
	AdminName nvarchar(max),
	AdminGender nvarchar(max),
	PrimaryTeacherName nvarchar(max),
	PrimaryTeacherGender nvarchar(max),
	ClassName nvarchar(max),
	FullClassName nvarchar(max),
	MinGradeLevelId int,
	MaxGradeLevelId int,
	PrimaryTeacherRef int,
	DepartmentId uniqueidentifier,
	QnACount int,
	StudentsCount int,
	AttachmentsCount int,
	OwnerAttachmentsCount int,
	StudentsCountWithAttachments int,
	ApplicationCount int,
	IsOwner bit,
	Complete bit,
	RowNumber bigint,
	AllCount int
)

--DistrictAdmin = 10
--Student = 3
--Teacher = 2

-- if role = DistrictAdmin 

declare @complete bit =  cast((select top 1 Complete from AdminAnnouncementData where AnnouncementRef = @id and PersonRef = @callerId) as bit)
if @complete is null 
	set @complete = 0

if(@callerRole = 10)
begin
	insert into @announcementTb
	exec spGetAdminAnnouncements @id, @callerId, @callerRole, 0, null, null, 0, 1, null, null, null, null
end 

if(@callerRole = 3)
begin
	insert into @announcementTb
	Select 
		vwAnnouncement.*,
		0 as IsOwner,
		@complete as Complete,
		1 as RowNumber,
		0 as AllCount
	from vwAnnouncement
	where vwAnnouncement.Id = @id
		 and (
				(vwAnnouncement.ClassRef is not null and exists(select * from ClassPerson where ClassRef = vwAnnouncement.ClassRef and PersonRef = @callerId))
				or 
				(vwAnnouncement.AdminRef is not null and exists(
						select * from AdminAnnouncementRecipient 
						join StudentGroup on StudentGroup.GroupRef = AdminAnnouncementRecipient.GroupRef
						where AdminAnnouncementRecipient.AnnouncementRef = @id and StudentGroup.StudentRef = @callerId
				      )
				)
		     )					
end

if(@callerRole = 2)
begin
	insert into @announcementTb
	Select 
		vwAnnouncement.*,
		cast((case when (select count(*) from ClassTeacher where PersonRef = @callerId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef) >= 1 then 1 else 0 end) as bit)  as IsOwner,
		@complete as Complete,
		1 as RowNumber,
		0 as AllCount
	from vwAnnouncement
	where vwAnnouncement.Id = @id	
		 and (vwAnnouncement.PrimaryTeacherRef = @callerId 
				 or exists(select * from ClassTeacher where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef)
			 )
end

declare @annExists bit
if(exists(select * from @announcementTb))
set @annExists = 1
else set @annExists = 0

if(@annExists = 1)
begin
	declare @ownersIds table(id int)
	declare @primaryTeacherId int
	declare @isOwner bit
	declare @classId int
	declare @adminId int 
	declare @isGradeble bit = 0, @isGradebleType bit = 0

	select @isOwner = a.IsOwner, @primaryTeacherId = a.PrimaryTeacherRef, @classId = a.ClassRef, @adminId = a.AdminRef 
	from @announcementTb a

	if @classId is not null
		insert into @ownersIds
		select PersonRef from ClassTeacher
		where ClassRef = @classId
	
	if(@isOwner = 1 and @adminId is null) 
	begin
		set @isGradeble = 1
		set @isGradebleType = 1
	end
	
	select *, @isGradeble as IsGradeble,
	@isGradebleType as IsGradebleType
	--,
	--@finalGradeStatus as FinalGradeStatus
	from @announcementTb

	--TODO: announcementQnA stored procedure
	exec spGetAnnouncementsQnA @callerId, null, @id, null, null, @schoolId


	select * from AnnouncementAttachment
	where AnnouncementRef = @id
	and ((@callerRole = 10 and (PersonRef = @callerId))
			or(@callerRole = 2 and (@isOwner = 1 or PersonRef = @callerId
									or (PersonRef in (select id from @ownersIds))
									)	
				)
			or(@callerRole = 3 and (PersonRef = @callerId
										or (PersonRef in (select id from @ownersIds) and exists(select * from ClassPerson where ClassPerson.ClassRef = @classId and ClassPerson.PersonRef = @callerId))
										or (PersonRef  = @adminId and exists(select * from AdminAnnouncementRecipient aar join StudentGroup st on st.GroupRef = aar.GroupRef
																			 where st.StudentRef = @callerId and aar.AnnouncementRef = @id))
									)
				)
		)

	select aa.*
	from AnnouncementApplication aa
	where aa.AnnouncementRef = @id and (@annExists = 1) and aa.Active = 1

	declare @date datetime2 = (select top 1 a.Expires from @announcementTb a)
	declare @markingPeriodId int = (select top 1 Id from MarkingPeriod where @date between StartDate and EndDate)


	if @primaryTeacherId is not null
	begin 
		select top 1 * from vwPerson
		where Id = @primaryTeacherId and (@callerRole = 1 or (SchoolRef = @schoolId))
	end
	else if @adminId is not null
	begin
		 select top 1 * from vwPerson
		 where Id = @adminId 
	end

	select * from AnnouncementStandard 
	join [Standard] on [Standard].Id = AnnouncementStandard.StandardRef
	where AnnouncementStandard.AnnouncementRef = @id

	select 
		AdminAnnouncementRecipient.Id as AdminAnnouncementRecipient_Id,
		AdminAnnouncementRecipient.AnnouncementRef as AdminAnnouncementRecipient_AnnouncementRef,
		AdminAnnouncementRecipient.GroupRef as AdminAnnouncementRecipient_GroupRef,
		[Group].Id as Group_Id,
		[Group].Name as Group_Name
	from AdminAnnouncementRecipient
	join [Group] on [Group].Id = AdminAnnouncementRecipient.GroupRef
	where AnnouncementRef = @id
end





GO


