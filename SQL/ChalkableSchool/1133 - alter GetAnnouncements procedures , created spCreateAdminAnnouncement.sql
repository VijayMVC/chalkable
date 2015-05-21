ALTER VIEW [dbo].[vwAnnouncement] 
AS 
SELECT
	Announcement.Id as Id,
	Announcement.Created as Created,
	Announcement.Expires as Expires,
	Announcement.[State] as [State],
	Announcement.[Order] as [Order],
	Announcement.Content as Content,
	Announcement.[Subject] as [Subject],
	Announcement.Title as [Title],
	Announcement.GradingStyle as GradingStyle,
	Announcement.Dropped as Dropped,
	Announcement.ClassAnnouncementTypeRef as ClassAnnouncementTypeRef,
	Announcement.SchoolRef as SchoolRef,
	Announcement.SisActivityId as SisActivityId,
    Announcement.MaxScore as MaxScore,
    Announcement.WeightAddition as WeightAddition,
    Announcement.WeightMultiplier as WeightMultiplier,
    Announcement.MayBeDropped as MayBeDropped,
	Announcement.VisibleForStudent as VisibleForStudent,
	Announcement.AdminRef as AdminRef,
	Announcement.ClassRef as ClassRef,
	null as AdminName,
	null as AdminGender,
	Staff.FirstName + ' ' + Staff.LastName as PrimaryTeacherName,
	Staff.Gender as PrimaryTeacherGender,
	Class.Name as ClassName,
	Class.Name + ' ' + (case when Class.ClassNumber is null then '' else Class.ClassNumber end) as FullClassName,
	Class.MinGradeLevelRef as MinGradeLevelId,
	Class.MaxGradeLevelRef as MaxGradeLevelId,
	Class.PrimaryTeacherRef as PrimaryTeacherRef,  
	Class.ChalkableDepartmentRef as DepartmentId,
	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = Announcement.Id) as QnACount,	
	(Select COUNT(*) from ClassPerson where ClassRef = Announcement.ClassRef) as StudentsCount,
	(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id) as AttachmentsCount,
	(select count(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id  and PersonRef in
		(select PersonRef from ClassTeacher where ClassRef = Announcement.ClassRef)
	) as OwnerAttachmentsCount,
	(	select COUNT(*) from
			(Select distinct PersonRef from AnnouncementAttachment 
			 where AnnouncementRef = Announcement.Id and 
				   Announcement.ClassRef in (select ClassPerson.ClassRef from ClassPerson 
											 where ClassPerson.PersonRef = AnnouncementAttachment.PersonRef)) as x
	) as StudentsCountWithAttachments,
	(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = Announcement.Id and Active = 1) as ApplicationCount

FROM 
	Announcement
	join Class on Class.Id = Announcement.ClassRef
	left join Staff on Staff.Id = Class.PrimaryTeacherRef

UNION 
SELECT
	Announcement.Id as Id,
	Announcement.Created as Created,
	Announcement.Expires as Expires,
	Announcement.[State] as [State],
	Announcement.[Order] as [Order],
	Announcement.Content as Content,
	Announcement.[Subject] as [Subject],
	Announcement.Title as [Title],
	Announcement.GradingStyle as GradingStyle,
	Announcement.Dropped as Dropped,
	Announcement.ClassAnnouncementTypeRef as ClassAnnouncementTypeRef,
	Announcement.SchoolRef as SchoolRef,
	Announcement.SisActivityId as SisActivityId,
    Announcement.MaxScore as MaxScore,
    Announcement.WeightAddition as WeightAddition,
    Announcement.WeightMultiplier as WeightMultiplier,
    Announcement.MayBeDropped as MayBeDropped,
	Announcement.VisibleForStudent as VisibleForStudent,
	Announcement.AdminRef as AdminRef,
	Announcement.ClassRef as ClassRef,
	Person.FirstName + ' ' + Person.LastName as AdminName,
	Person.Gender as AdminGender,
	null as PrimaryTeacherName,
	null as PrimaryTeacherGender,
	null as ClassName,
	null as FullClassName,
	null as MinGradeLevelId,
	null as MaxGradeLevelId,
	null as PrimaryTeacherRef,  
	null as DepartmentId,
	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = Announcement.Id) as QnACount,	
	0 as StudentsCount, --todo get student from recipients table
	(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id) as AttachmentsCount,
	(select count(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id  and PersonRef = Announcement.AdminRef) as OwnerAttachmentsCount,
	0 as StudentsCountWithAttachments, --todo get student from recipients table
	(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = Announcement.Id and Active = 1) as ApplicationCount

FROM 
	Announcement
	join Person on Person.Id = Announcement.AdminRef

GO


alter procedure [dbo].[spGetAdminAnnouncements]  
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
	and (@gradeLevelsIds is null or vwAnnouncement.Id in (
														  select ar.AnnouncementRef from AdminAnnouncementRecipient ar
														  left join @gradeLevelsIdsT glT on glT.value = ar.GradeLevelRef
														  where  glT.value is not null or ar.ToAll = 1 or ar.[Role] = 3 or ar.[Role] = 2
														  )
		)
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
		and (@gradeLevelsIds is null or vwAnnouncement.Id in (
															  select ar.AnnouncementRef from AdminAnnouncementRecipient ar
															  left join @gradeLevelsIdsT glT on glT.value = ar.GradeLevelRef
															  where glT.value is not null or ar.ToAll = 1 or ar.[Role] = 3 or ar.[Role] = 2
														     )
		)
	order by Created desc				
	OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
GO


alter procedure [dbo].[spGetAnnouncementDetails] @id int, @callerId int, @callerRole int, @schoolId int
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
									or (PersonRef in (select id from @ownersIds) and exists(select * from AdminAnnouncementRecipient 
																		where [Role] = @callerRole or PersonRef = @callerId or ToAll = 1
																		 or SchoolRef = @schoolId))
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

	select * from AdminAnnouncementRecipient
	where AnnouncementRef = @id

end
GO

create procedure spCreateAdminAnnouncement @personId int, @created datetime2, @expires datetime2, @state int
as
begin transaction
--Only districtAdmn can create admin announcement 	
declare @callerRole int = 10 
declare @announcementId int
declare @isDraft bit = 0

if @state = 0 
begin
	select top 1 @announcementId = Id
	from Announcement
	where AdminRef = @personId and [State] = 0
	order by Created desc

	if @announcementId is not null
	update Announcement set [State] = -1 where Id = @announcementId
end

/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef IN (SELECT Id FROM Announcement 
where AdminRef = @personId AND [State] = 0)
/*DELETE AutoGrades*/
delete from AutoGrade
where AnnouncementApplicationRef IN (SELECT AnnouncementApplication.Id from AnnouncementApplication
		join Announcement on Announcement.Id = AnnouncementRef
		where AdminRef = @personId AND [State] = 0)
/*DELETE AnnouncementApplication*/
delete from AnnouncementApplication where AnnouncementRef IN (SELECT Id FROM Announcement
where AdminRef = @personId AND [State] = 0)
/*DELETE AdminAnnouncementRecipient*/
delete from AdminAnnouncementRecipient
where AnnouncementRef in (Select id from announcement 
where AdminRef = @personId AND [State] = 0)
/*DELETE AnnouncementStandard*/
delete from AnnouncementStandard
where AnnouncementRef in (Select id from announcement 
where AdminRef = @personId AND [State] = 0)
/*DELETE Announcement*/
delete from Announcement
where AdminRef = @personId AND [State] = 0

if @announcementId is not null
begin
	update Announcement set [State] = 0 where Id = @announcementId
	set @isDraft = 1
end
else begin
	/*INSERT TO ANNOUNCEMENT*/
	insert into Announcement (Created, Expires, ClassAnnouncementTypeRef, [State],GradingStyle,[Order], ClassRef, Dropped, SchoolRef, MayBeDropped, VisibleForStudent, AdminRef)
	values(@created, @expires, null, @state, 0, 1, null, 0, null, 0, 1, @personId);
	set @announcementId = SCOPE_IDENTITY()

	/*GET CONTENT FROM PREV ANNOUNCEMENT*/
	declare @prevContent nvarchar(1024)
	select top 1
	@prevContent = Content from Announcement
	where AdminRef = @personId and [State] = 1 and Content is not null
	order by Created desc
	
	update Announcement set Content = @prevContent where Id = @announcementId
end

exec spGetAnnouncementDetails @announcementId, @personId, @callerRole, null

commit
go 

drop table AnnouncementRecipient
go