drop procedure spApplyStarringAnnouncementForStudent
go
drop procedure spApplyStarringAnnouncementForTeacher
go
drop procedure spUpdateAnnouncemetRecipientData
go
drop procedure spGetDueDays
go
drop table AnnouncementRecipientData
go


ALTER procedure [dbo].[spGetStudentAnnouncements]  
	@id int, @schoolId int, @personId int, @classId int,  @roleId int,  @ownedOnly bit,  @gradedOnly bit
	, @fromDate DateTime2, @toDate DateTime2, @markingPeriodId int
	, @start int, @count int, @now DateTime2, @sisActivitiesIds nvarchar(max)
as 

declare @gradeLevelRef int = (select top 1 GradeLevelRef  
							  from StudentSchoolYear 
							  join SchoolYear on SchoolYear.Id = StudentSchoolYear.SchoolYearRef 
							  where StudentRef = @personId and SchoolYear.StartDate <= @now and SchoolYear.EndDate >= @now)

declare @mpStartDate datetime2, @mpEndDate datetime2
if(@markingPeriodId is not null)
	select @mpStartDate = StartDate, @mpEndDate = EndDate from MarkingPeriod where Id = @markingPeriodId

declare @sisActivitiesIdsT table(Id int)
if(@sisActivitiesIds is not null and LTRIM(@sisActivitiesIds) <> '')
begin
	insert into @sisActivitiesIdsT(Id)
	select cast(s as int) from dbo.split(',', @sisActivitiesIds)
end

declare @allCount int = (select COUNT(*) from
	vwAnnouncement
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))	
	and(@classId is null or ClassRef = @classId)
	and (exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))					
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
)
declare @notExpiredCount int = (select count(*) 
    from 
		vwAnnouncement	
	where
		(@id is not null or Expires >= @now) and
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and(@classId is null or ClassRef = @classId)
		and (exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))				
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
		)

select *
from
	(
	Select 
		vwAnnouncement.*,
		0 as IsOwner,
		ROW_NUMBER() OVER(ORDER BY  vwAnnouncement.Expires) as RowNumber,
		@allCount as AllCount
	from 
		vwAnnouncement	
	where
		(@id is not null or Expires >= @now) and
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and(@classId is null or ClassRef = @classId)
		and (exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))					
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))

	union 
	Select 
		vwAnnouncement.*,
		0 as IsOwner,
		(ROW_NUMBER() OVER(ORDER BY  vwAnnouncement.Expires desc)) + @notExpiredCount as RowNumber,
		@allCount as AllCount
	from 
		vwAnnouncement	
	where
		(@id is not null or Expires < @now) and
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and(@classId is null or ClassRef = @classId)
		and (exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))					
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))

	) x
where RowNumber > @start and RowNumber <= @start + @count
order by RowNumber


GO

ALTER procedure [dbo].[spGetTeacherAnnouncements]  
	@id int, @schoolId int, @personId int, @classId int, @roleId int, @ownedOnly bit, @gradedOnly bit
	,@fromDate DateTime2, @toDate DateTime2, @markingPeriodId int, @start int, @count int, @now DateTime2, @allSchoolItems bit
	, @sisActivitiesIds nvarchar(max)
as 

declare @sisActivitiesIdsT table(Id int)
if(@sisActivitiesIds is not null and LTRIM(@sisActivitiesIds) <> '')
begin
	insert into @sisActivitiesIdsT(Id)
	select cast(s as int) from dbo.split(',', @sisActivitiesIds)
end


declare @gradeLevelsT table(Id int)
insert into @gradeLevelsT(Id)
select GradeLevelRef from Class
where PrimaryTeacherRef = @personId
group by GradeLevelRef 

declare @mpStartDate datetime2, @mpEndDate datetime2
if(@markingPeriodId is not null)
	select @mpStartDate = StartDate, @mpEndDate = EndDate from MarkingPeriod where Id = @markingPeriodId


declare @allCount int;
set @allCount = (select COUNT(*) from
	vwAnnouncement	
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@classId is null or ClassRef = @classId)
	and ((@allSchoolItems = 1 and @roleId = 2) or vwAnnouncement.PrimaryTeacherRef = @personId 
			 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef))
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
	and (@ownedOnly = 0 or vwAnnouncement.PrimaryTeacherRef = @personId 
		 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef))
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
)


Select 
	vwAnnouncement.*,
	--cast((case vwAnnouncement.PrimaryTeacherRef when @personId then 1 else 0 end) as bit) as IsOwner,
	cast((case when (select count(*) from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef) >= 1 then 1 else 0 end) as bit)  as IsOwner,
	ROW_NUMBER() OVER(ORDER BY vwAnnouncement.Created desc) as RowNumber,
 	@allCount as AllCount
from 
	vwAnnouncement	
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@classId is null or ClassRef = @classId)and ((@allSchoolItems = 1 and @roleId = 2) or vwAnnouncement.PrimaryTeacherRef = @personId 
		 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef))			
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
	and (@ownedOnly = 0 or vwAnnouncement.PrimaryTeacherRef = @personId 
		 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef))
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
	
order by Created desc				
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

GO
----------------------------
---GET ANNOUNCEMENT DETAILS 
----------------------------
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
	SchoolRef int not null,
	SisActivityId int null,
    MaxScore decimal null,
    WeightAddition decimal null,
    WeightMultiplier decimal null,
    MayBeDropped bit,
	VisibleForStudent bit,
	ClassAnnouncementTypeName nvarchar(max),
	AnnouncementType int,
	ClassRef int,
	PrimaryTeacherName nvarchar(max),
	PrimaryTeacherGender nvarchar(max),
	ClassName nvarchar(max),
	GradeLevelId int,
	PrimaryTeacherRef int,
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

--if(@callerRole = 5 or @callerRole = 8  or @callerRole = 7 or @callerRole = 1)
--begin
--insert into @announcementTb
--exec spGetAdminAnnouncements @id, @schoolId, @callerId, null,  @callerRole, 0, 0, null, null, null, 0, 1, null, null
--end
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
--declare @annTypeId int
declare @isGradeble bit = 0, @isGradebleType bit = 0

select @isOwner = a.IsOwner, @primaryTeacherId = a.PrimaryTeacherRef, @classId = a.ClassRef, @isGradebleType = cat.Gradable 
from @announcementTb a
join ClassAnnouncementType cat on cat.Id = a.ClassAnnouncementTypeRef 

insert into @ownersIds
select PersonRef from ClassTeacher
where ClassRef = @classId

--if(@annTypeId = 2 or @annTypeId = 3 or @annTypeId = 4 or @annTypeId = 5
--	or @annTypeId =6 or @annTypeId =7 or @annTypeId =8 or @annTypeId =9 or @annTypeId = 10)
--set @isGradebleType = 1

if(@isOwner = 1 and @isGradebleType = 1) set @isGradeble = 1

--declare @markingPeriodClassId uniqueidentifier  = (select MarkingPeriodClassRef from @announcementTb)
declare @wasSubmittedToAdmin bit = 0;
--declare @finalGradeStatus int =  (select [Status] from FinalGrade where Id = @markingPeriodClassId)

select *, @isGradeble as IsGradeble,
	@isGradebleType as IsGradebleType
--,
--@finalGradeStatus as FinalGradeStatus
from @announcementTb

--TODO: announcementQnA stored procedure
exec spGetAnnouncementsQnA @callerId, null, @id, null, null


select * from AnnouncementAttachment
where AnnouncementRef = @id
	and (((@callerRole = 5 or @callerRole = 8  or @callerRole = 7) and (PersonRef = @callerId))
			or(@callerRole = 2 and (@isOwner = 1 or PersonRef = @callerId
									or (PersonRef in (select id from @ownersIds) and exists(select * from AnnouncementRecipient 
																		where RoleRef = @callerRole or PersonRef = @callerId or ToAll = 1))
								   )	
			  )
			or(@callerRole = 3 and (PersonRef = @callerId
									 or (PersonRef in (select id from @ownersIds) and exists(select * from ClassPerson where ClassRef = @classId and PersonRef = @callerId))
									)
			  )
		)

select aa.*
--as AnnouncementApplicationId,
--aa.AnnouncementRef as AnnouncementId,
--aa.Active as Active,
--aa.[Order] as [Order]
--,
--cast(case when exists(select * from ApplicationInstall where PersonRef = @callerId and Active = 1 and ApplicationRef = a.Id)
--	then 1 else 0
--end as bit)  as IsInstalledForMe
from AnnouncementApplication aa
where aa.AnnouncementRef = @id and (@annExists = 1) and aa.Active = 1

declare @date datetime2 = (select top 1 a.Expires from @announcementTb a)
declare @markingPeriodId int = (select top 1 Id from MarkingPeriod where @date between StartDate and EndDate)

exec spGetPersons @schoolId, @primaryTeacherId, @callerId, null, 0, 1, null,null,null,null,null,null,null, 1, @callerRole, @markingPeriodId

select * from AnnouncementStandard 
join [Standard] on [Standard].Id = AnnouncementStandard.StandardRef
where AnnouncementStandard.AnnouncementRef = @id
end


GO
ALTER PROCEDURE [dbo].[spDeleteAnnouncement] @id int, @personId int, @classId int, @state int, @classAnnouncementTypeId int
AS

declare @ann table
(
	Id int,
	SchoolYearId int,
	AnnouncementTypeId int,
	PersonId int,
	[State] int,
	ClassId int
)

insert into @ann(Id, SchoolYearId, AnnouncementTypeId, PersonId, [State], ClassId)
select a.Id, c.SchoolYearRef, a.ClassAnnouncementTypeRef, c.PrimaryTeacherRef, a.State, a.ClassRef
from Announcement a
join Class c on c.Id = a.ClassRef
where (@id is null or a.Id = @id)
	and (@personId is null or c.PrimaryTeacherRef = @personId or exists(select * from ClassTeacher ct where ct.PersonRef = @personId and ct.ClassRef = c.Id))
	and (@classId is null or a.ClassRef = @classId)
	and (@classAnnouncementTypeId is null or ClassAnnouncementTypeRef = @classAnnouncementTypeId)
	and (a.ClassRef is not null or a.ClassAnnouncementTypeRef is null  or a.[State] = 0)
	and (@state is null or a.[State] = @state)

/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef in (select Id from @ann)
/*DELETE AnnouncementApplication*/

/*DELETE AnnouncementRecipient*/
delete from AnnouncementRecipient
where AnnouncementRef in (select Id from @ann)

delete from [Notification]
where AnnouncementRef in (select Id from @ann)

/*DELETE ANOUNCEMENTQNA*/
delete from AnnouncementQnA
where AnnouncementRef in (select Id from @ann)

/*DELETE AnnouncementApplication*/
delete from AnnouncementApplication
where AnnouncementRef in (select Id from @ann)

/*DELETE AnnouncementStandard*/
delete from AnnouncementStandard
where AnnouncementRef in (select Id from @ann)

/*DELETE Announcement*/
delete from Announcement where Id in (select Id from @ann)


declare @schoolYearId int

declare  AnnCursor cursor for
select SchoolYearId, AnnouncementTypeId, PersonId,  ClassId
from @ann

open AnnCursor
fetch next from AnnCursor
into @schoolYearId, @classAnnouncementTypeId, @personId, @classId

declare @oldSchoolYear int, @oldannouncementTypeId int,
@oldPersonId int, @oldClassId int

while @@FETCH_STATUS = 0
begin

if(@schoolYearId <> @oldSchoolYear or @classAnnouncementTypeId <> @oldannouncementTypeId
	or @personId <> @oldPersonId or @classId <> @oldClassId
	or (@oldSchoolYear is null and @oldannouncementTypeId is null
	and @oldPersonId is null and @oldClassId is null))
begin
/*Reordering Process*/
exec spReorderAnnouncements @schoolYearId, @classAnnouncementTypeId, @classId

set @oldannouncementTypeId = @classAnnouncementTypeId
set @oldClassId = @classId
set @oldPersonId = @personId
set @oldSchoolYear = @schoolYearId

end
fetch next from AnnCursor
into @schoolYearId, @classAnnouncementTypeId, @personId, @classId
end
CLOSE AnnCursor;
DEALLOCATE AnnCursor;


GO

--------------------------
-- CREATE ANNOUNCEMENT 
-----------------------------
ALTER procedure [dbo].[spCreateAnnouncement] @schoolId int, @classAnnouncementTypeId int, @personId int, @created datetime2,
											  @expires datetime2, @state int, @gradingStyle int, @classId int
as
begin transaction
declare @callerRole int
select top 1 @callerRole = RoleRef from SchoolPerson where PersonRef = @personId

declare @announcementId int
--declare @markingPeriodClassId int;
--if(@classId is not null)
--	set @markingPeriodClassId = (select Id from MarkingPeriodClass where ClassRef = @classId and MarkingPeriodRef = @markingPeriodId)

declare @isDraft bit = 0

if @state = 0
begin
	select top 1 @announcementId = Id 
	from Announcement
	where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId) and [State] = 0
		  and (@classAnnouncementTypeId is null or ClassAnnouncementTypeRef = @classAnnouncementTypeId)
	order by Created desc

	if @announcementId is null
		select top 1 @announcementId = Id from Announcement
		where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId) and [State] = 0
		order by Created desc

	if @announcementId is not null
	update Announcement set [State] = -1 where Id = @announcementId
end


/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef IN (SELECT Id FROM Announcement 
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0)

/*DELETE AnnouncementApplication*/
delete from AnnouncementApplication where AnnouncementRef IN (SELECT Id FROM Announcement
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0)


/*DELETE AnnouncementRecipient*/
delete from AnnouncementRecipient
where AnnouncementRef in (Select id from announcement 
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [state] = 0)

/*DELETE AnnouncementStandard*/
delete from AnnouncementStandard
where AnnouncementRef in (Select id from announcement 
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [state] = 0)

/*DELETE Announcement*/
delete from Announcement
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0

/*RESTORE STATE FOR DRAFT*/
if @announcementId is not null
begin
	update Announcement set [State] = 0 where Id = @announcementId
	set @isDraft = 1
end
else begin
	/*INSERT TO ANNOUNCEMENT*/
	insert into Announcement (Created, Expires, ClassAnnouncementTypeRef, [State],GradingStyle,[Order], ClassRef, Dropped, SchoolRef, MayBeDropped, VisibleForStudent)
	values(@created, @expires, @classAnnouncementTypeId, @state, @gradingStyle, 1, @classId, 0, @schoolId, 0, 1);
	set @announcementId = SCOPE_IDENTITY()
end


/*GET CONTENT FROM PREV ANNOUNCEMENT*/
declare @prevContent nvarchar(1024)
select top 1
@prevContent = Content from Announcement
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
	  and ClassAnnouncementTypeRef = @classAnnouncementTypeId
	  and [State] = 1
  	  and Content is not null
order by Created desc

update Announcement set Content = @prevContent where Id = @announcementId

commit 

if(@classAnnouncementTypeId is not null and @classId is not null)
begin 
	declare @schoolYearId int
	select @schoolYearId = Id from SchoolYear where @created between StartDate and EndDate
	declare @resT table (res int)
	insert into @resT
	exec [spReorderAnnouncements] @schoolYearId, @classAnnouncementTypeId,  @classId
end
exec spGetAnnouncementDetails @announcementId, @personId, @callerRole, @schoolId
GO


