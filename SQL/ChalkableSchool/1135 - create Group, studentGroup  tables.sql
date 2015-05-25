create table [Group]
(
	Id int not null primary key identity(1,1),
	Name nvarchar(255) not null,
)
go

create table StudentGroup
(
	GroupRef int not null constraint FK_StudentGroup_Group foreign key references [Group](Id),
	StudentRef int not null constraint FK_StudentGroup_Student foreign key references Student(Id)
)
go

delete from AdminAnnouncementRecipient
go
drop table AdminAnnouncementRecipient
go

create table AdminAnnouncementRecipient
(
	Id int not null primary key identity(1,1),
	AnnouncementRef int not null constraint FK_AdminAnnouncementRecipient_Announcement foreign key references Announcement(Id),
	GroupRef int not null constraint FK_AdminAnnouncementRecipient_Group foreign key references [Group](Id)
)
alter table AdminAnnouncementRecipient
add constraint UQ_Announcement_Group unique (AnnouncementRef, GroupRef) 
go



ALTER procedure [dbo].[spGetAdminAnnouncements]  
	@id int, @personId int, @roleId int,  @ownedOnly bit
	,@fromDate DateTime2, @toDate DateTime2, @start int, @count int, @now DateTime2
	,@gradeLevelsIds nvarchar(256) 
as 

declare @allCount int;
declare @gradeLevelsIdsT table(value int);
if(@gradeLevelsIds is not null)
begin
	insert into @gradeLevelsIdsT(value)
	select cast(s as int) from dbo.split(',', @gradeLevelsIds)
end

set @allCount = (select COUNT(*) from
	vwAnnouncement	
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@ownedOnly = 0 or vwAnnouncement.AdminRef = @personId)
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	--and (@gradeLevelsIds is null or vwAnnouncement.Id in (
	--													  select ar.AnnouncementRef from AdminAnnouncementRecipient ar
	--													  left join @gradeLevelsIdsT glT on glT.value = ar.GradeLevelRef
	--													  where  glT.value is not null or ar.ToAll = 1 or ar.[Role] = 3 or ar.[Role] = 2
	--													  )
		--)
)

	Select 
		vwAnnouncement.*,
		cast((case vwAnnouncement.AdminRef when @personId then 1 else 0 end) as bit) as IsOwner,
		ROW_NUMBER() OVER(ORDER BY vwAnnouncement.Created desc) as RowNumber,
		@allCount as AllCount
	from 
		vwAnnouncement	
	where
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@ownedOnly = 0 or vwAnnouncement.AdminRef = @personId)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		--and (@gradeLevelsIds is null or vwAnnouncement.Id in (
		--													  select ar.AnnouncementRef from AdminAnnouncementRecipient ar
		--													  left join @gradeLevelsIdsT glT on glT.value = ar.GradeLevelRef
		--													  where glT.value is not null or ar.ToAll = 1 or ar.[Role] = 3 or ar.[Role] = 2
		--												     )
		--)
	order by Created desc				
	OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

GO


ALTER procedure [dbo].[spGetAnnouncementDetails] @id int, @callerId int, @callerRole int, @schoolId int
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
	RowNumber bigint,
	AllCount int
)

--DistrictAdmin = 10
--Student = 3
--Teacher = 2

-- if role = DistrictAdmin 
if(@callerRole = 10)
begin
	insert into @announcementTb
	exec spGetAdminAnnouncements @id, @callerId, @callerRole, 0, null, null, 0, 1, null, null
end 

if(@callerRole = 3)
begin
insert into @announcementTb
exec spGetStudentAnnouncements @id, @schoolId, @callerId, null, @callerRole, 0, 0, null, null, null, 0, 1, null, null
end
if(@callerRole = 2)
begin
insert into @announcementTb
exec spGetTeacherAnnouncements @id, @schoolId, @callerId, null,  @callerRole, 0, 0, null, null, null, 0, 1, null, 1, null
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

	if @classId is null
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
										or (PersonRef in (select id from @ownersIds) and exists(select * from ClassPerson where ClassRef = @classId and PersonRef = @callerId))
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


