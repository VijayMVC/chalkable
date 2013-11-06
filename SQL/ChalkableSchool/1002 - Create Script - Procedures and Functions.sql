--Get Classes Procedure
CREATE procedure [dbo].[spGetClasses] @schoolId int, @schoolYearId int, @markingPeriodId int, @callerId int, @callerRoleId int,
										@personId int, @start int, @count int, @classId int, 
										@filter1 nvarchar(max), @filter2 nvarchar(max), @filter3 nvarchar(max)
as

declare @callerSchoolId int
declare @roleId int = (select top 1 RoleRef from SchoolPerson where PersonRef = @personId and SchoolRef = @schoolId)

declare @class table
(
	Class_Id int,
	Class_Name nvarchar(255),
	Class_Description  nvarchar(1024),
	Class_SchoolYearRef int,
	Class_TeacherRef int,
	Class_GradeLevelRef int,
	Class_ChalkableDepartmentRef uniqueidentifier,
	Class_SchoolRef  int,
	GradeLevel_Id int,
	GradeLevel_Name nvarchar(255),
	GradeLevel_Number int,
	Person_Id int,
	Person_FirstName nvarchar(255),
	Person_LastName nvarchar(255),
	Person_Gender nvarchar(255),
	Person_Salutation nvarchar(255),
	Person_Email nvarchar(256),
	Class_SchoolId int,
	Class_StudentsCount int
)

select Count(*) as SourceCount
from vwClass
where (@classId is null or Class_Id = @classId)
and (@schoolYearId is null or Class_SchoolYearRef = @schoolYearId)
and (@markingPeriodId is null or exists(select * from MarkingPeriodClass where ClassRef = Class_Id and MarkingPeriodRef = @markingPeriodId))

and (@callerRoleId = 1 or ((Class_SchoolId = @schoolId) and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2
	or (@callerRoleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @callerId))
)))
and (@personId is null or (@roleId = 2 and Class_TeacherRef = @personId)
		or @roleId = 5 or  @roleId = 7 or  @roleId = 8 or @roleId = 1
		or (@roleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @personId))
	)
and
(
	(@filter1 is null and @filter2 is null and @filter3 is null) or
	(@filter1 is not null and Class_Name like @filter1 or
		@filter2 is not null and Class_Name like @filter2 or
		@filter3 is not null and Class_Name like @filter3)
)


insert into @class
select vwClass.*, (select count(*) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
from vwClass
where
(@classId is null or Class_Id = @classId)
and (@schoolYearId is null or Class_SchoolYearRef = @schoolYearId)
and (@markingPeriodId is null or exists(select * from MarkingPeriodClass where ClassRef = Class_Id and MarkingPeriodRef = @markingPeriodId))

and (@callerRoleId = 1 or ((Class_SchoolId = @schoolId) and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2
	or (@callerRoleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @callerId))
)))
and (@personId is null or (@roleId = 2 and Class_TeacherRef = @personId)
or @roleId = 5 or  @roleId = 7 or  @roleId = 8 or @roleId = 1
or (@roleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @personId))

)
and
(
(@filter1 is null and @filter2 is null and @filter3 is null) or
(@filter1 is not null and Class_Name like @filter1 or
@filter2 is not null and Class_Name like @filter2 or
@filter3 is not null and Class_Name like @filter3)
)
order by vwClass.Class_Id
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY


select * from @class

select mpc.* from MarkingPeriodClass mpc
join @class c on c.Class_Id = mpc.ClassRef
GO


----------------------
-- GET PERSONS

CREATE procedure [dbo].[spGetPersons] @schoolId int,
	@personId int, @callerId int, @roleId int, @start int, @count int, @startFrom nvarchar(255)
	, @teacherId int, @classId int, @filter1 nvarchar(255), @filter2 nvarchar(255), @filter3 nvarchar(255)
	, @gradeLevelIds nvarchar(1024), @sortType int, @callerRoleId int
as

declare @glIds table 
(
	id int not null
) 
if @gradeLevelIds is not null
begin
	insert into @glIds
	select cast(s as int) from dbo.split(',', @gradeLevelIds)
end

declare @callerGradeLevelId int = null;
if(@callerRoleId = 3)
begin
	-- todo : needs currentSchoolYearId for getting right grade level 
	set @callerGradeLevelId = (select  top 1 GradeLevelRef from StudentSchoolYear where StudentRef = @callerId)
end


select COUNT(*) as AllCount from vwPerson
where
	(@personId is null or Id = @personId)
	and (@roleId is null or RoleRef = @roleId)
	and (@startFrom is null or LastName >= @startFrom)
	and (@teacherId is null or 
		Id in (select ClassPerson.PersonRef from 
							ClassPerson 
							join Class on ClassPerson.ClassRef = Class.Id 
							where Class.TeacherRef = @teacherId
							))
	and (@classId is null or ((@roleId is null or @roleId = 3) and Id in (select PersonRef from ClassPerson where ClassPerson.ClassRef = @classId))
						  or ((@roleId is null or @roleId = 2) and Id in (select TeacherRef from Class where Id = @classId))
		)
	and (@callerRoleId = 1 or ((exists(select * from SchoolPerson where PersonRef = @personId and SchoolRef = @schoolId)) and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
			or(@callerRoleId = 3 and (Id = @callerId 
										   or (RoleRef = 2 or RoleRef = 5 or RoleRef = 7 or RoleRef = 8 
											   or (RoleRef = 3 and GradeLevel_Id = @callerGradeLevelId)))
			   )
			or(@callerRoleId = 6 and (Id = @callerId or RoleRef = 3))
		)))	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
		 or FirstName like @filter2 or LastName like @filter2
		 or FirstName like @filter3 or LastName like @filter3)
	and (
			@gradeLevelIds is null 
			or (vwPerson.RoleRef = 3 and exists(select * from @glIds as x where x.id = GradeLevel_Id))
			or (vwPerson.RoleRef = 2 and exists
				(select * from Class where Class.TeacherRef = vwPerson.Id and Class.GradeLevelRef in (select id from @glIds))
			)
		)

-- Sort Type
-- 0 : by FisrtName
-- 1 : by LastName
--------------------			
select * from
(						
	select 
		vwPerson.*,
		ROW_NUMBER() OVER(ORDER BY case when @sortType = 0 then vwPerson.FirstName else vwPerson.LastName end) as RowNumber
	from vwPerson
	where
	(@personId is null or  Id = @personId)
	and (@roleId is null or RoleRef = @roleId)
	and (@startFrom is null or LastName >= @startFrom)
	and (@teacherId is null or 
		Id in (select ClassPerson.PersonRef from 
							ClassPerson 
							join Class on ClassPerson.ClassRef = Class.Id 
							where Class.TeacherRef = @teacherId
							))
	and (@classId is null or ((@roleId is null or @roleId = 3) and Id in (select PersonRef from ClassPerson where ClassPerson.ClassRef = @classId))
						  or ((@roleId is null or @roleId = 2) and Id in (select TeacherRef from Class where Id = @classId))
		)
	and (@callerRoleId = 1 or ((exists(select * from SchoolPerson where PersonRef = @personId and SchoolRef = @schoolId)) and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
			or(@callerRoleId = 3 and (Id = @callerId 
										   or (RoleRef = 2 or RoleRef = 5 or RoleRef = 7 or RoleRef = 8 
											   or (RoleRef = 3 and GradeLevel_Id = @callerGradeLevelId)))
			   )
			or(@callerRoleId = 6 and (Id = @callerId or RoleRef = 3))
		)))	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
		 or FirstName like @filter2 or LastName like @filter2
		 or FirstName like @filter3 or LastName like @filter3)
	and (
			@gradeLevelIds is null 
			or (vwPerson.RoleRef = 3 and exists(select * from @glIds as x where x.id = GradeLevel_Id))
			or (vwPerson.RoleRef = 2 and exists
				(select * from Class where Class.TeacherRef = vwPerson.Id and Class.GradeLevelRef in (select id from @glIds))
			)
		)
						
) x
where
	x.RowNumber > @start
	and x.RowNumber <= @start + @count
order by  case when @sortType = 0 then x.FirstName  else x.LastName end
GO


-- Get Person Details

create procedure [dbo].[spGetPersonDetails] @schoolId int, @personId int, @callerId int, @callerRoleId int
as

declare @personT table
(
	Id int,
	RoleRef int,
	FirstName nvarchar(255),
	LastName nvarchar(255),
	BirthDate datetime2,
	Gender nvarchar(255),
	Salutation nvarchar(255),
	Active bit,
	FirstLogInDate datetime2,
	Email nvarchar(256),
	AddressRef int,
	GradeLevel_Id  int,
	GradeLevel_Name varchar(max)
)
insert into @personT
exec spGetPersons @schoolId, @personId, @callerId, null, 0, 1, null, null, null, null,null,null,null,0, @callerRoleId

select p.*,
	   [Address].AddressNumber as Address_AddressNumber,
	   [Address].StreetNumber as Address_StreetNumber,
	   [Address].AddressLine1 as Address_AddressLine1,
	   [Address].AddressLine2 as Address_AddressLine2,
	   [Address].City as Address_City,
	   [Address].[State] as Address_State,
	   [Address].PostalCode as Address_PostalCode,
	   [Address].Country as Address_Country,
	   [Address].CountyID as Address_CountyID,
	   [Address].Latitude as Address_Latitude,
	   [Address].Longitude as Address_Longitude
from @personT p
join [Address] on [Address].Id = p.AddressRef

select * from Phone
where PersonRef = @personId
GO


CREATE procedure [dbo].[spUpdateAnnouncemetRecipientData] @personId int, @announcementId int,
@starred bit, @starredAutomatically int, @currentDate date
as

declare @id int, @oldStarredAutomatically bit
select @id = anr.id, @oldStarredAutomatically = anr.StarredAutomatically
from AnnouncementRecipientData anr
where anr.AnnouncementRef = @announcementId and anr.PersonRef = @personId

if @id is not null
begin
	if(@starredAutomatically is null)
		set @starredAutomatically = @oldStarredAutomatically
	update AnnouncementRecipientData
	set Starred = @starred , StarredAutomatically = @starredAutomatically, LastModifiedDate = @currentDate
	where @id = AnnouncementRecipientData.Id
end
else 
begin
	if(@starredAutomatically is null)
		set @starredAutomatically = 0
	insert into AnnouncementRecipientData (AnnouncementRef, PersonRef, Starred, StarredAutomatically, LastModifiedDate)
	values (@announcementId, @personId, @starred, @starredAutomatically, @currentDate)
end
GO

create procedure [dbo].[spGetDueDays] @AnnTypeId int, @dueDays1 int output, @dueDays2 int output
as

--Id	Name
--1		Announcement
--2		HW
--3		Essay
--4		Quiz
--5		Test
--6		Project
--7		Final
--8		Midterm
--9		Book report
--10	Term paper
--11	Admin


  if @AnnTypeId != 11 and @AnnTypeId != 1 
  begin
	   
       
	   if @AnnTypeId = 2
		  set @dueDays1 = 2
	   else 
		  set @dueDays1 = 3

       if @AnnTypeId = 6 or @AnnTypeId = 10 
          set @dueDays2 = 14
       else
       begin
            if @AnnTypeId != 5 and @AnnTypeId != 2 and @AnnTypeId != 4
               set @dueDays2 = 7
       end
  end
GO


----------------------------------------
--APPLY STARRING ANNOUNCEMENT PROCEDURES
----------------------------------------

create procedure [dbo].[spApplyStarringAnnouncementForStudent] @personId int, @currentDate date
as

DECLARE @id int, @classId int,
		@ExpDay date, @AnnTypeId int, @saterredAnnRecipientDate int
DECLARE @currentDay date = @currentDate
DECLARE @tomorrowDate date = dateAdd(day, 1, @currentDay)
DECLARE @dueDays1 int = 0
DECLARE @dueDays2 int = 0
DECLARE @starredA int = 0
DECLARE AnnouncementStCursor CURSOR FOR
select an.Id,
	   an.ClassRef,
	   an.Expires,
	   ClassAnnouncementType.AnnouncementTypeRef,
	   ard.StarredAutomatically
from Announcement an
left join ClassAnnouncementType on ClassAnnouncementType.Id = an.ClassAnnouncementTypeRef
left join ClassPerson c on c.ClassRef = an.ClassRef and c.PersonRef = @personId
left join AnnouncementRecipientData ard on ard.AnnouncementRef = an.Id and ard.PersonRef = @personId

where ((an.ClassRef is not null and c.PersonRef = @personId)
		   or(an.ClassAnnouncementTypeRef is null and exists(select * from AnnouncementRecipient ar
															 where ar.AnnouncementRef = an.Id 
																   and (ar.ToAll = 1 or ar.PersonRef = @personId or ar.RoleRef = 3)
															)
			  )
	  )
	  and (ard.LastModifiedDate is null or ard.LastModifiedDate < @currentDate )
	  and an.[State] <> 0 and (ard.Id is null or (ard.Starred <> 0 and an.Expires < @currentDate) 
											  or (ard.Starred <> 1 and an.Expires = @tomorrowDate))
OPEN AnnouncementStCursor
FETCH NEXT FROM AnnouncementStCursor
INTO @id, @classId, @ExpDay, @AnnTypeId, @saterredAnnRecipientDate


declare @studentAvgT table(classId int, [avg] int)

WHILE @@FETCH_STATUS = 0
BEGIN 
	if @ExpDay < @currentDay
	BEGIN
		exec spUpdateAnnouncemetRecipientData
		@personId,
		@id, 0, @starredA, @currentDate
	END
	else BEGIN

		set @starredA = 0
		if(@saterredAnnRecipientDate is not null)
		begin
			set @starredA = @saterredAnnRecipientDate
		end

		if @ExpDay = DATEADD(Day, 1, @currentDay) and @starredA < 3
		BEGIN
			set @starredA = 3
			exec spUpdateAnnouncemetRecipientData
				 @personId, @id, 1, @starredA, @currentDate
		END
		else BEGIN
	
			exec dbo.spGetDueDays @AnnTypeId, @dueDays1 output, @dueDays2 output
			DECLARE @avgStudent int = (select top 1 [avg] from @studentAvgT where classId = @classId)
	
			if(@avgStudent is null)
			begin
				--TODO : think about this ... include type percents
				set @avgStudent = (select Avg(sa.GradeValue)
								   from Announcement an
								   join StudentAnnouncement sa on sa.AnnouncementRef = an.Id
								   where sa.PersonRef = @personId and an.ClassRef = @classId
								   )
				if(@avgStudent is null) set @avgStudent = 0
		
				insert into @studentAvgT(classId, [avg])
				values(@classId, @avgStudent)
			end
			if (@dueDays1 != 0)
			and
			((((@ExpDay <= DATEADD(Day, @dueDays1, @currentDay) and @starredA < 2)  or (@dueDays2!=0 and @ExpDay <= DATEADD(Day, @dueDays2, @currentDay) and @starredA < 1))
			and  @avgStudent >= 80)
			or
			(((@ExpDay <= DATEADD(Day, @dueDays1 + 1, @currentDay) and @starredA < 2) or (@dueDays2!=0 and @ExpDay <= DATEADD(Day, @dueDays2 + 1, @currentDay)and @starredA < 1))
			and @avgStudent between 65 and 80)
			or
			(((@ExpDay <= DATEADD(Day, @dueDays1 + 2, @currentDay) and @starredA < 2) or (@dueDays2!=0 and @ExpDay <= DATEADD(Day, @dueDays2 + 2, @currentDay)and @starredA < 1))
			and @avgStudent <= 65))

			BEGIN
				set @starredA = @starredA + 1
				exec spUpdateAnnouncemetRecipientData
				@personId, 	@id, 1, @starredA, @currentDate
			END
			set @dueDays1 = 0
			set @dueDays2 = 0
			END
		END
	FETCH NEXT FROM AnnouncementStCursor
	INTO @id, @classId, @ExpDay, @AnnTypeId, @saterredAnnRecipientDate
END
CLOSE AnnouncementStCursor;
DEALLOCATE AnnouncementStCursor;
GO

CREATE procedure [dbo].[spApplyStarringAnnouncementForTeacher] @personId int, @currentDate date
as
	DECLARE @id int, @ExpDay date, @starredA int
	DECLARE @currentDay date = @currentDate
	DECLARE @nextDay date = DATEADD(Day, 1, @currentDay)
	DECLARE AnnouncementCursorToUnstar CURSOR FOR

	select x.AnnouncmentId, x.AnExpires, ard.StarredAutomatically
	from AnnouncementRecipientData ard
	right join (select an.Id as AnnouncmentId, an.Expires as AnExpires , @personId as AnSchoolPerson
					from Announcement an
					left join AnnouncementRecipient ar on ar.AnnouncementRef = an.Id
					where  an.PersonRef = @personId or (ar.Id is not null and
					(ar.PersonRef = @personId  or ar.ToAll = 1 or ar.RoleRef = 2))
				)x
	on x.AnnouncmentId = ard.AnnouncementRef and x.AnSchoolPerson = ard.PersonRef
	where x.AnExpires < @currentDay and ard.StarredAutomatically = 1
			and (ard.Id is null or ard.Starred <> 0)
			and ard.LastModifiedDate < @currentDay

	OPEN AnnouncementCursorToUnstar
	FETCH NEXT FROM AnnouncementCursorToUnstar
	INTO @id, @ExpDay, @starredA
	WHILE @@FETCH_STATUS = 0
	BEGIN
		exec spUpdateAnnouncemetRecipientData @personId, @id, 0, 0, @currentDay
		FETCH NEXT FROM AnnouncementCursorToUnstar
		INTO @id, @ExpDay, @starredA
	END
	CLOSE AnnouncementCursorToUnstar;
	DEALLOCATE AnnouncementCursorToUnstar;


	DECLARE AnnouncementCursorToStar CURSOR FOR
	select x.AnnouncmentId, x.AnExpires, ard.StarredAutomatically
	from AnnouncementRecipientData ard
	right join (select an.Id as AnnouncmentId, an.Expires as AnExpires , @personId as AnSchoolPerson
					from Announcement an
					left join AnnouncementRecipient ar on ar.AnnouncementRef = an.Id
					where an.PersonRef = @personId or (ar.Id is not null and
					(ar.PersonRef = @personId  or ar.ToAll = 1 or ar.RoleRef = 2))
				)x
	on x.AnnouncmentId = ard.AnnouncementRef and x.AnSchoolPerson = ard.PersonRef
	where x.AnExpires = @nextDay
   		  and (ard.StarredAutomatically < 1 or ard.StarredAutomatically is null)
		  and (ard.Id is null or ard.Starred <> 1)
		  and (ard.LastModifiedDate is null or ard.LastModifiedDate < @currentDay)

	OPEN AnnouncementCursorToStar
	FETCH NEXT FROM AnnouncementCursorToStar
	INTO @id, @ExpDay, @starredA

	WHILE @@FETCH_STATUS = 0
	BEGIN
		exec spUpdateAnnouncemetRecipientData @personId, @id, 1, 1, @currentDay

		FETCH NEXT FROM AnnouncementCursorToStar
		INTO @id, @ExpDay, @starredA
	END
	CLOSE AnnouncementCursorToStar;
	DEALLOCATE AnnouncementCursorToStar;
GO


----------------------------------------
-- GET ANNOUNCEMENTS PROCEDURES
----------------------------------------


create procedure [dbo].[spGetAdminAnnouncements]  
	@id int, @schoolId int, @personId int, @classId int, @roleId int, @staredOnly bit, @ownedOnly bit
	,@fromDate DateTime2, @toDate DateTime2, @markingPeriodId int, @start int, @count int, @now DateTime2
	,@gradeLevelsIds nvarchar(256) 
as 

declare @allCount int;
declare @gradeLevelsIdsT table(value int);
if(@gradeLevelsIds is not null)
begin
	insert into @gradeLevelsIdsT(value)
	select cast(s as int) from dbo.split(',', @gradeLevelsIds)
end

declare @mpStartDate datetime2, @mpEndDate datetime2
if(@markingPeriodId is not null)
	select @mpStartDate = StartDate, @mpEndDate = EndDate from MarkingPeriod where Id = @markingPeriodId

set @allCount = (select COUNT(*) from
	vwAnnouncement	
	left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
		 on vwAnnouncement.Id = ard.AnnouncementRef
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
	and (@classId is null or ClassRef = @classId)
	and (@staredOnly = 0 or Starred = 1)
	and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	and (@gradeLevelsIds is null or vwAnnouncement.Id in (
														  select ar.AnnouncementRef from AnnouncementRecipient ar
														  left join @gradeLevelsIdsT glT on glT.value = ar.GradeLevelRef
														  where  glT.value is not null or ar.ToAll = 1 or ar.RoleRef = 3 or ar.RoleRef = 2
														  )
		)
)

	Select 
		vwAnnouncement.*,
		cast((case vwAnnouncement.PersonRef when @personId then 1 else 0 end) as bit) as IsOwner,
		ard.PersonRef as RecipientDataPersonId,
		ard.Starred as Starred,
		ROW_NUMBER() OVER(ORDER BY vwAnnouncement.Created desc) as RowNumber,
		--0 as StarredCount,
		@allCount as AllCount
	from 
		vwAnnouncement	
		left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
			 on vwAnnouncement.Id = ard.AnnouncementRef
	where
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and (@classId is null or ClassRef = @classId)
		and (@staredOnly = 0 or Starred = 1)
		and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@gradeLevelsIds is null or vwAnnouncement.Id in (
															  select ar.AnnouncementRef from AnnouncementRecipient ar
															  left join @gradeLevelsIdsT glT on glT.value = ar.GradeLevelRef
															  where glT.value is not null or ar.ToAll = 1 or ar.RoleRef = 3 or ar.RoleRef = 2
														     )
		)
	order by Created desc				
	OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

GO

create procedure [dbo].[spGetTeacherAnnouncements]  
	@id int, @schoolId int, @personId int, @classId int, @roleId int, @staredOnly bit, @ownedOnly bit, @gradedOnly bit
	,@fromDate DateTime2, @toDate DateTime2, @markingPeriodId int, @start int, @count int, @now DateTime2, @allSchoolItems bit
as 

exec spApplyStarringAnnouncementForTeacher @personId, @now;

declare @gradeLevelsT table(Id int)
insert into @gradeLevelsT(Id)
select GradeLevelRef from Class
where TeacherRef = @personId
group by GradeLevelRef 

declare @mpStartDate datetime2, @mpEndDate datetime2
if(@markingPeriodId is not null)
	select @mpStartDate = StartDate, @mpEndDate = EndDate from MarkingPeriod where Id = @markingPeriodId


declare @allCount int;
set @allCount = (select COUNT(*) from
	vwAnnouncement	
	left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
		 on vwAnnouncement.Id = ard.AnnouncementRef
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@classId is null or ClassRef = @classId)
	and ((@allSchoolItems = 1 and @roleId = 2) or vwAnnouncement.PersonRef = @personId 
		or (ClassAnnouncementTypeRef is null 
				and exists(select AnnouncementRecipient.Id from AnnouncementRecipient
						   where AnnouncementRef = vwAnnouncement.Id  and (ToAll = 1 or PersonRef = @personId 
								or (RoleRef = @roleId and (@roleId <> 2 or GradeLevelRef is null or GradeLevelRef in (select Id from @gradeLevelsT))))
						   )
		    )
		)
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
	and (@staredOnly = 0 or Starred = 1)
	and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
)


Select 
	vwAnnouncement.*,
	cast((case vwAnnouncement.PersonRef when @personId then 1 else 0 end) as bit) as IsOwner,
	ard.PersonRef as RecipientDataPersonId,
	ard.Starred as Starred,
	ROW_NUMBER() OVER(ORDER BY vwAnnouncement.Created desc) as RowNumber,
	--@starredCount as StarredCount,
	 	@allCount as AllCount
from 
	vwAnnouncement	
	left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
			on vwAnnouncement.Id = ard.AnnouncementRef
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@classId is null or ClassRef = @classId)
	and ((@allSchoolItems = 1 and @roleId = 2) or vwAnnouncement.PersonRef = @personId 
		or (ClassAnnouncementTypeRef is null 
			and exists(select AnnouncementRecipient.Id from AnnouncementRecipient
						where AnnouncementRef = vwAnnouncement.Id  and (ToAll = 1 or PersonRef = @personId 
							or (RoleRef = @roleId and (@roleId <> 2 or GradeLevelRef is null or GradeLevelRef in (select Id from @gradeLevelsT))))
						)
			)
		)			
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
	and (@staredOnly = 0 or Starred = 1)
	and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	and (@gradedOnly = 0 or GradingStudentsCount > 0)		
order by Created desc				
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

GO




create procedure [dbo].[spGetStudentAnnouncements]  
	@id int, @schoolId int, @personId int, @classId int,  @roleId int, @staredOnly bit, @ownedOnly bit,  @gradedOnly bit
	, @fromDate DateTime2, @toDate DateTime2, @markingPeriodId int
	, @start int, @count int, @now DateTime2
as 

exec spApplyStarringAnnouncementForStudent @personId, @now;

declare @gradeLevelRef int = (select top 1 GradeLevelRef  
							  from StudentSchoolYear 
							  join SchoolYear on SchoolYear.Id = StudentSchoolYear.SchoolYearRef 
							  where StudentRef = @personId and SchoolYear.StartDate <= @now and SchoolYear.EndDate >= @now)


declare @mpStartDate datetime2, @mpEndDate datetime2
if(@markingPeriodId is not null)
	select @mpStartDate = StartDate, @mpEndDate = EndDate from MarkingPeriod where Id = @markingPeriodId


declare @allCount int = (select COUNT(*) from
	vwAnnouncement	
	left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
		 on vwAnnouncement.Id = ard.AnnouncementRef
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))	
	and(@classId is null or ClassRef = @classId)
	and (vwAnnouncement.PersonRef = @personId 
		or (ClassAnnouncementTypeRef is null 
			and exists
				(
						select * from AnnouncementRecipient 
						where AnnouncementRef = vwAnnouncement.Id 
						and (ToAll = 1 or PersonRef = @personId or (RoleRef = @roleId and (@roleId <> 3 or GradeLevelRef is null or GradeLevelRef = @gradeLevelRef)))
				)
			)
		or exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId)
	)			
	and (@staredOnly = 0 or Starred = 1)
	and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	and (@gradedOnly = 0 or (GradingStudentsCount > 0 and vwAnnouncement.Id in (select sa.AnnouncementRef from StudentAnnouncement sa 
																				where  sa.PersonRef = @personId and sa.GradeValue is not null)))
)
declare @notExpiredCount int = (select count(*) 
    from 
		vwAnnouncement	
		left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
			 on vwAnnouncement.Id = ard.AnnouncementRef
	where
		(@id is not null or Expires >= @now) and
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and(@classId is null or ClassRef = @classId)
		and (vwAnnouncement.PersonRef = @personId 
			or (ClassAnnouncementTypeRef is null 
				and exists
					(
							select * from AnnouncementRecipient 
							where AnnouncementRef = vwAnnouncement.Id 
							and (ToAll = 1 or PersonRef = @personId or (RoleRef = @roleId and (@roleId <> 3 or GradeLevelRef is null or GradeLevelRef = @gradeLevelRef)))
					)
				)
			or exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId)
		)			
		and (@staredOnly = 0 or Starred = 1)
		and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@gradedOnly = 0 or (GradingStudentsCount > 0 and vwAnnouncement.Id in (select sa.AnnouncementRef from StudentAnnouncement sa 
																					where  sa.PersonRef = @personId and sa.GradeValue is not null)))
		)


select *
from
	(
	Select 
		vwAnnouncement.*,
		cast((case vwAnnouncement.PersonRef when @personId then 1 else 0 end) as bit) as IsOwner,
		ard.PersonRef as RecipientDataPersonId,
		ard.Starred as Starred,
		ROW_NUMBER() OVER(ORDER BY  vwAnnouncement.Expires) as RowNumber,
		@allCount as AllCount
	from 
		vwAnnouncement	
		left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
			 on vwAnnouncement.Id = ard.AnnouncementRef
	where
		(@id is not null or Expires >= @now) and
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and(@classId is null or ClassRef = @classId)
		and (vwAnnouncement.PersonRef = @personId 
			or (ClassAnnouncementTypeRef is null 
				and exists
					(
							select * from AnnouncementRecipient 
							where AnnouncementRef = vwAnnouncement.Id 
							and (ToAll = 1 or PersonRef = @personId or (RoleRef = @roleId and (@roleId <> 3 or GradeLevelRef is null or GradeLevelRef = @gradeLevelRef)))
					)
				)
			or exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId)
		)			
		and (@staredOnly = 0 or Starred = 1)
		and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@gradedOnly = 0 or (GradingStudentsCount > 0 and vwAnnouncement.Id in (select sa.AnnouncementRef from StudentAnnouncement sa 
																					 where  sa.PersonRef = @personId and sa.GradeValue is not null)))
	union 
	Select 
		vwAnnouncement.*,
		cast((case vwAnnouncement.PersonRef when @personId then 1 else 0 end) as bit) as IsOwner,
		ard.PersonRef as RecipientDataPersonId,
		ard.Starred as Starred,
		(ROW_NUMBER() OVER(ORDER BY  vwAnnouncement.Expires desc)) + @notExpiredCount as RowNumber,
		@allCount as AllCount
	from 
		vwAnnouncement	
		left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
			 on vwAnnouncement.Id = ard.AnnouncementRef
	where
		(@id is not null or Expires < @now) and
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and(@classId is null or ClassRef = @classId)
		and (vwAnnouncement.PersonRef = @personId 
			or (ClassAnnouncementTypeRef is null 
				and exists
					(
							select * from AnnouncementRecipient 
							where AnnouncementRef = vwAnnouncement.Id 
							and (ToAll = 1 or PersonRef = @personId or (RoleRef = @roleId and (@roleId <> 3 or GradeLevelRef is null or GradeLevelRef = @gradeLevelRef)))
					)
				)
			or exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId)
		)			
		and (@staredOnly = 0 or Starred = 1)
		and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@gradedOnly = 0 or (GradingStudentsCount > 0 and vwAnnouncement.Id in (select sa.AnnouncementRef from StudentAnnouncement sa 
																					where  sa.PersonRef = @personId and sa.GradeValue is not null)))
	) x
where RowNumber > @start and RowNumber <= @start + @count
order by RowNumber
GO


CREATE procedure [dbo].[spGetAnnouncementsQnA] @callerId int, @announcementQnAId int
												, @announcementId int, @askerId int, @answererId int
as
	declare @callerRolerId int = (select top 1 RoleRef from SchoolPerson where PersonRef = @callerId)

	select vwAnnouncementQnA.*,
		cast((case when @callerId = vwAnnouncementQnA.AskerId then 1 else 0 end) as bit) as IsOwner
	from vwAnnouncementQnA
	where (@announcementId is null or AnnouncementRef = @announcementId)
		and (@askerId is null or @askerId = AskerId)
		and (@answererId is  null or @answererId = AnswererId)
		and (@callerRolerId = 1 or @callerId = AnswererId  or @callerId = AskerId 
				or 	(ClassRef is not null and AnsweredTime is not null
						and exists(select * from ClassPerson cp 
    								where cp.ClassRef = ClassRef and @callerId = cp.PersonRef
								  )
					)
			)
		and (@announcementQnAId is null or @announcementQnAId = Id)
	order by QuestionTime
GO


CREATE Procedure [dbo].[spGetStudentAnnouncementsForAnnouncement]
	@announcementId int, @personId int
as

		Insert Into StudentAnnouncement
		(AnnouncementRef, PersonRef, Dropped, [State])
		select x.AnnouncementRef, x.PersonRef, 0, 1 from
		StudentAnnouncement
		right join 	(select Announcement.Id as AnnouncementRef, ClassPerson.ClassRef as ClassRef, ClassPerson.PersonRef as PersonRef  
					 from Announcement
					 join ClassPerson on ClassPerson.ClassRef = Announcement.ClassRef
					 where Announcement.[State] = 1)x
		on StudentAnnouncement.PersonRef = x.PersonRef and x.AnnouncementRef = StudentAnnouncement.AnnouncementRef
		where x.AnnouncementRef = @announcementId
			and StudentAnnouncement.Id is null
	
	declare @roleId int
	select top 1 @roleId = RoleRef from SchoolPerson where PersonRef = @personId

	select vwPerson.*,
		   StudentAnnouncement.Id as StudentAnnouncement_Id,
		   StudentAnnouncement.PersonRef as StudentAnnouncement_PersonRef,
		   StudentAnnouncement.AnnouncementRef as StudentAnnouncement_AnnouncementRef,
		   StudentAnnouncement.ApplicationRef as StudentAnnouncement_ApplicationRef,
		   StudentAnnouncement.Comment as StudentAnnouncement_Comment,
		   StudentAnnouncement.Dropped as StudentAnnouncement_Dropped,
		   StudentAnnouncement.ExtraCredit as StudentAnnouncement_ExtraCredit,
		   StudentAnnouncement.GradeValue as StudentAnnouncement_GradeValue,
		   StudentAnnouncement.State as StudentAnnouncement_State,
		   Announcement.ClassRef as StudentAnnouncement_ClassId
	from StudentAnnouncement 
	join Announcement on StudentAnnouncement.AnnouncementRef = Announcement.Id
	join vwPerson  on vwPerson.Id = StudentAnnouncement.PersonRef
	where  Announcement.[State] = 1 
			and AnnouncementRef = @announcementId
			and (StudentAnnouncement.PersonRef = @personId or @roleId = 2 or @roleId = 5 or @roleId = 8 or @roleId = 7)
GO





create procedure [dbo].[spGetAnnouncementDetails] @id int, @callerId int, @callerRole int, @schoolId int
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
	GradingStyle int not null,
	Dropped bit not null,
	ClassAnnouncementTypeRef int not null,
	SchoolRef int not null,
	ClassAnnouncementTypeName nvarchar(max),
	AnnouncementType int,
	PersonRef int not null,
	ClassRef int,
	PersonName nvarchar(max),
	PersonGender nvarchar(max),
	ClassName nvarchar(max),
	GradeLevelId int,
	QnACount int,
	StudentsCount int,
	AttachmentsCount int,
	OwnerAttachmentsCount int,
	StudentsCountWithAttachments int,
	GradingStudentsCount int,
	[Avg] int,
	ApplicationCount int,
	IsOwner bit,
	RecipientDataSchoolPersonId int,
	Starred bit,
	RowNumber bigint,

	--StarredCount int,
	AllCount int
)

if(@callerRole = 5 or @callerRole = 8  or @callerRole = 7 or @callerRole = 1)
begin
insert into @announcementTb
exec spGetAdminAnnouncements @id, @schoolId, @callerId, null,  @callerRole, 0, 0, null, null, null, 0, 1, null, null
end
if(@callerRole = 3)
begin
insert into @announcementTb
exec spGetStudentAnnouncements @id, @schoolId, @callerId, null, @callerRole, 0, 0, 0, null, null, null, 0, 1, null
end
if(@callerRole = 2)
begin
insert into @announcementTb
exec spGetTeacherAnnouncements @id, @schoolId, @callerId, null,  @callerRole, 0, 0, 0, null, null, null, 0, 1, null, 1
end

declare @annExists bit
if(exists(select * from @announcementTb))
set @annExists = 1
else set @annExists = 0

if(@annExists = 1)
begin
declare @ownerId int
declare @classId int
declare @annTypeId int
select @ownerId = PersonRef , @classId = a.ClassRef, @annTypeId = cat.AnnouncementTypeRef 
from @announcementTb a
join ClassAnnouncementType cat on cat.Id = a.ClassAnnouncementTypeRef 

declare @isGradeble bit = 0, @isGradebleType bit = 0
if(@annTypeId = 2 or @annTypeId = 3 or @annTypeId = 4 or @annTypeId = 5
	or @annTypeId =6 or @annTypeId =7 or @annTypeId =8 or @annTypeId =9 or @annTypeId = 10)
set @isGradebleType = 1

if(@ownerId = @callerId and @isGradebleType = 1) set @isGradeble = 1

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

exec spGetStudentAnnouncementsForAnnouncement @id, @callerId

select * from AnnouncementAttachment
where AnnouncementRef = @id
	and (((@callerRole = 5 or @callerRole = 8  or @callerRole = 7) and (PersonRef = @callerId))
			or(@callerRole = 2 and (@ownerId = @callerId or PersonRef = @callerId
									or (@ownerId = PersonRef and exists(select * from AnnouncementRecipient 
																		where RoleRef = @callerRole or PersonRef = @callerId or ToAll = 1))
								   )	
			  )
			or(@callerRole = 3 and (PersonRef = @callerId
									 or (PersonRef = @ownerId and exists(select * from ClassPerson where ClassRef = @classId and PersonRef = @callerId))
									)
			  )
		)

select * from AnnouncementReminder
where AnnouncementRef = @id and (@annExists = 1)
	and (@callerRole = 1 or (@ownerId = @callerId and PersonRef is null)
	or (PersonRef is not null and @callerId = PersonRef))


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

exec spGetPersons @schoolId, @ownerId, @callerId, null, 0, 1, null,null,null,null,null,null,null, 1, @callerRole
end
GO


-----------------------------
-- CREATE ANNOUNCEMENT 
-----------------------------

CREATE procedure [dbo].[spCreateAnnouncement] @schoolId int, @classAnnouncementTypeId int, @personId int, @created datetime2,
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
	where PersonRef = @personId and [State] = 0
		  and (@classAnnouncementTypeId is null or ClassAnnouncementTypeRef = @classAnnouncementTypeId)
	order by Created desc

	if @announcementId is null
		select top 1 @announcementId = Id from Announcement
		where PersonRef = @personId and [State] = 0
		order by Created desc

	if @announcementId is not null
	update Announcement set [State] = -1 where Id = @announcementId

end

/*DELETE REMINDER*/
delete from AnnouncementReminder where AnnouncementRef IN (SELECT Id FROM Announcement WHERE PersonRef = @personId
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0)

/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef IN (SELECT Id FROM Announcement WHERE PersonRef = @personId
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0)

/*DELETE AnnouncementApplication*/
delete from AnnouncementApplication where AnnouncementRef IN (SELECT Id FROM Announcement WHERE PersonRef = @personId
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0)

/*DELETE StudentAnnouncement*/
delete from StudentAnnouncement
where AnnouncementRef in (Select id from announcement where PersonRef = @personId
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [state] = 0)

/*DELETE AnnouncementRecipientData*/
delete from AnnouncementRecipientData
where AnnouncementRef in (Select id from announcement where PersonRef = @personId
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [state] = 0)

/*DELETE AnnouncementRecipient*/
delete from AnnouncementRecipient
where AnnouncementRef in (Select id from announcement where PersonRef = @personId
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [state] = 0)


/*DELETE Announcement*/
delete from Announcement where PersonRef = @personId
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
	insert into Announcement (PersonRef, Created, Expires, ClassAnnouncementTypeRef, [State],GradingStyle,[Order], ClassRef, Dropped)
	values(@personId, @created, @expires, @classAnnouncementTypeId, @state, @gradingStyle, 1, @classId, 0);
	set @announcementId = SCOPE_IDENTITY()
end

/*GET CONTENT FROM PREV ANNOUNCEMENT*/
declare @prevContent nvarchar(1024)
select top 1
@prevContent = Content from Announcement
where PersonRef = @personId
	  and ClassAnnouncementTypeRef = @classAnnouncementTypeId
	  and [State] = 1
  	  and Content is not null
order by Created desc

update Announcement set Content = @prevContent where Id = @announcementId

commit
--Select @announcementId;

--declare @roleId int
--select @roleId = RoleRef from Person where Id = @personId
--if @roleId = 2
--begin
--insert into @tmp
--	exec spGetTeacherAnnouncements
--	@announcementId, @personId, null, null, null, 0, 0, 0, null, null, null, 0, 1, null, 0
--end
--else begin
--insert into @tmp
--	exec spGetAdminAnnouncements
--	@announcementId, @personId, null, null, null, 0, 0, null, null, null, 0, 1, null, null
--end

--update @tmp set IsDraft = @isDraft

--select * from @tmp

exec spGetAnnouncementDetails @announcementId, @personId, @callerRole, @schoolId

GO



create procedure [dbo].[spReorderAnnouncements] @schoolYearId int, @classAnnType int, 
												@ownerId int, @classId int
as
with AnnView as
               (
                select a.Id, Row_Number() over(order by a.Expires, a.[Created]) as [Order]  
                from Announcement a
                join Class c on c.Id = a.ClassRef
				where c.SchoolYearRef = @schoolYearId and a.ClassAnnouncementTypeRef = @classAnnType 
                      and a.PersonRef = @ownerId and [State] = 1 and a.ClassRef = @classId
               )
update Announcement
set [Order] = AnnView.[Order]
from AnnView 
where AnnView.Id = Announcement.Id
select  1
GO

CREATE PROCEDURE [dbo].[spDeleteAnnouncement] @id int, @personId int, @classId int,
												@state int, @classAnnouncementTypeId int
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
select a.Id, c.SchoolYearRef, a.ClassAnnouncementTypeRef, a.PersonRef, a.State, a.ClassRef
from Announcement a
join Class c on c.Id = a.ClassRef
where (@id is null or a.Id = @id)
	and (@personId is null or PersonRef = @personId)
	and (@classId is null or a.ClassRef = @classId)
	and (@classAnnouncementTypeId is null or ClassAnnouncementTypeRef = @classAnnouncementTypeId)
	and (a.ClassRef is not null or a.ClassAnnouncementTypeRef is null  or a.[State] = 0)
	and (@state is null or a.[State] = @state)

delete from AnnouncementReminder where AnnouncementRef in (select Id from @ann)
/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef in (select Id from @ann)
/*DELETE AnnouncementApplication*/
delete from StudentAnnouncement
where AnnouncementRef in (select Id from @ann)
/*DELETE AnnouncementRecipient*/
delete from AnnouncementRecipient
where AnnouncementRef in (select Id from @ann)
/*DELETE AnnouncementRecipientData*/
delete from AnnouncementRecipientData
where AnnouncementRef in (select Id from @ann)

delete from [Notification]
where AnnouncementRef in (select Id from @ann)

/*DELETE ANOUNCEMENTQNA*/
delete from AnnouncementQnA
where AnnouncementRef in (select Id from @ann)

/*DELETE AnnouncementApplication*/
delete from AnnouncementApplication
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
exec spReorderAnnouncements @schoolYearId, @classAnnouncementTypeId, @personId, @classId

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




CREATE procedure [dbo].[spGetPersonsForApplicationInstall]
	@applicationId uniqueidentifier, @callerId int, @personId int, @roleIds nvarchar(2048), @departmentIds nvarchar(2048)
	, @gradeLevelIds nvarchar(2048), @classIds nvarchar(2048), @callerRoleId int
	, @hasAdminMyApps bit, @hasTeacherMyApps bit, @hasStudentMyApps bit, @canAttach bit, @schoolYearId int
as
declare @roleIdsT table(value int);
declare @departmentIdsT table(value uniqueidentifier);
declare @gradeLevelIdsT table(value int);
declare @classIdsT table(value int);


PRINT('application id');
PRINT(@applicationId);
PRINT('caller id');
PRINT(@callerId);
PRINT('person id');
PRINT(@personId );
PRINT('caller role id');
PRINT(@callerRoleId);

if(@roleIds is not null)
begin
	insert into @roleIdsT(value)
	select cast(s as int) from dbo.split(',', @roleIds) 
end
if(@departmentIds is not null)
begin
	insert into @departmentIdsT(value)
	select cast(s as int) from dbo.split(',', @departmentIds)
end
if(@gradeLevelIds is not null)
begin
	insert into @gradeLevelIdsT(value)
	select cast(s as int) from dbo.split(',', @gradeLevelIds)
end
if(@classIds is not null)
begin
	insert into @classIdsT(value)
	select cast(s as int) from dbo.split(',', @classIds)
end

declare @canInstallForTeahcer bit = @hasTeacherMyApps | @canAttach
declare @canInstallForStudent bit = @hasStudentMyApps | @canAttach

declare @canInstall bit = 0
if (
	(@callerRoleId = 3 and @hasStudentMyApps = 1) 
	or (@callerRoleId = 2 and (@canInstallForStudent = 1 or @canInstallForTeahcer = 1))
	or ((@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8) 
		 and (@hasAdminMyApps = 1 or @canInstallForStudent = 1 or @canInstallForTeahcer = 1)
	   )
   )
set @canInstall = 1


declare @schoolId int = (select SchoolRef from SchoolYear where Id = @schoolYearId)

declare @preResult table
(
	[Type] int not null,
	GroupId nvarchar(256) not null,
	PersonId int not null
);
declare @localPersons table
(
	Id int not null,
	RoleRef int not null
);
--select sp due to security
if @canInstall = 1
begin
	if @callerRoleId = 3
	begin
		insert into @localPersons (Id, RoleRef)
		select PersonRef, RoleRef 
		from SchoolPerson
		where PersonRef = @callerId and SchoolRef = @schoolId and @hasStudentMyApps = 1
	end
	if  @callerRoleId = 2
	begin
		insert into @localPersons (Id, RoleRef)
		select PersonRef, RoleRef 
		from SchoolPerson
		where ((@canInstallForStudent = 1 and PersonRef in (select ClassPerson.PersonRef
															from ClassPerson
															join Class on ClassPerson.ClassRef = Class.Id
															where Class.TeacherRef = @callerId
									  					   )
			   ) 
			   or (PersonRef = @callerId and @canInstallForTeahcer = 1))
			   and SchoolRef = @schoolId
	end
	if @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8
	begin
		insert into @localPersons (Id, RoleRef)
		select PersonRef, RoleRef 
		from SchoolPerson
		where ((RoleRef = 2 and @canInstallForTeahcer = 1)
				or ((RoleRef = 5 or RoleRef = 7 or RoleRef = 8) and @hasAdminMyApps = 1)
				or (RoleRef = 3 and @canInstallForStudent = 1))
			  and SchoolRef = @schoolId
	end
end

delete from @localPersons where Id in (select PersonRef from ApplicationInstall where ApplicationRef = @applicationId and Active = 1)
/*
TYPES:
0 - role
1 - department
2 - grade level
3 - class
4 - person
*/
--insert by roles
if @roleIds is not null
Insert into @preResult
([type], groupId, PersonId)
select 0, cast(RoleRef as nvarchar(256)), Id
from @localPersons
where RoleRef in (select Value from @roleIdsT)
--insert by departments
if @departmentIds is not null
begin
	Insert into @preResult ([type], groupId, PersonId)
	select distinct 1, cast(d.Value as nvarchar(256)), sp.Id
	from @localPersons as sp
	join ClassPerson as csp on sp.Id = csp.PersonRef
	join Class c on csp.ClassRef = c.Id
	join @departmentIdsT d on c.ChalkableDepartmentRef = d.value


	Insert into @preResult ([type], groupId, PersonId)
	select distinct 1, cast(d.Value as nvarchar(256)), sp.Id
	from @localPersons as sp
	join Class c on c.TeacherRef = sp.Id
	join @departmentIdsT d on c.ChalkableDepartmentRef = d.value
end
--insert by grade level
if @gradeLevelIds is not null
begin
	Insert into @preResult
	([type], groupId, PersonId)
	select distinct 2, cast(gl.Value as nvarchar(256)), sp.Id
	from @localPersons as sp
	join StudentSchoolYear as ssy on sp.Id = ssy.StudentRef and ssy.SchoolYearRef = @schoolYearId
	join @gradeLevelIdsT gl on ssy.GradeLevelRef = gl.value 
	
	Insert into @preResult
	([type], groupId, PersonId)
	select distinct 2, cast(gl.Value as nvarchar(256)), sp.Id
	from @localPersons as sp
	join Class c on sp.Id = c.TeacherRef
	join @gradeLevelIdsT gl on c.GradeLevelRef = gl.value
end
--insert by class
if @classIds is not null
begin
	Insert into @preResult
	([type], groupId, PersonId)
	select distinct 3, cast(cc.Value as nvarchar(256)), sp.Id
	from @localPersons as sp
	join ClassPerson csp on csp.PersonRef = sp.Id
	join @classIdsT cc on csp.ClassRef = cc.value

	if @callerRoleId <> 2
	begin
		Insert into @preResult ([type], groupId, PersonId)
		select distinct 3, cast(cc.Value as nvarchar(256)), sp.Id
		from @localPersons as sp
		join Class c on sp.Id = c.TeacherRef
		join @classIdsT cc on c.Id = cc.value
	end
end

declare @isSinglePerson bit = 0
if @personId is null and @callerRoleId = 2
begin
	set @personId = @callerId
	set @isSinglePerson = 1
end

--insert by sp
if @personId is not null
	Insert into @preResult([type], groupId, PersonId)
	select 4, cast(Id as nvarchar(256)), Id
	from @localPersons
	where id = @personId



if @roleIds is null and @departmentIds is null and @gradeLevelIds is null 
	and @classIds is null and (@isSinglePerson = 1 or @personId is null)
	
	Insert into @preResult
	([type], groupId, PersonId)
	select 4, cast(Id as nvarchar(256)), Id
	from @localPersons

select * 
from @preResult

GO


CREATE procedure [dbo].[spGetPersonsForApplicationInstallCount]
	@applicationId uniqueidentifier, @callerId int, @personId int,
	 @roleIds nvarchar(2048), @departmentIds nvarchar(2048)
	, @gradeLevelIds nvarchar(2048), @classIds nvarchar(2048), @callerRoleId int
	, @hasAdminMyApps bit, @hasTeacherMyApps bit, @hasStudentMyApps bit, @canAttach bit, @schoolId int
as
PRINT('start');
PRINT(getDate());
declare @preResult table
(
	[Type] int not null,
	GroupId nvarchar(256) not null,
	PersonId int not null
);
insert into @preResult
exec dbo.spGetPersonsForApplicationInstall @applicationId, @callerId, @personId, @roleIds, @departmentIds, @gradeLevelIds, @classIds
, @callerRoleId , @hasAdminMyApps , @hasTeacherMyApps , @hasStudentMyApps , @canAttach, @schoolId

select [Type], [GroupId], Count(*) as [Count]
from @preResult
group by [Type], [GroupId]
union
select 5 as [Type], null as [GroupId], COUNT(*) as [Count] from (select distinct PersonId from @preResult) z

PRINT('end');
PRINT(getDate());

GO

CREATE procedure [dbo].[spGetStudentCountToAppInstallByClass]
@applicationId uniqueidentifier, @schoolYearId int, @personId int, @roleId int
as

declare @emptyResult bit = 0;

select
	Class.Id as ClassId,
	Class.Name as ClassName,
	Count(*) as NotInstalledStudentCount
from Class
join ClassPerson on ClassPerson.ClassRef = Class.Id
left join (select * from ApplicationInstall where ApplicationRef = @applicationId and Active = 1) x
on x.PersonRef = ClassPerson.PersonRef and x.SchoolYearRef = Class.SchoolYearRef
where
	@emptyResult = 0
	and Class.SchoolYearRef = @schoolYearId
	and (@roleId = 8 or @roleId = 7 or @roleId = 5 or @roleId = 2 and Class.TeacherRef = @personId)
	and x.Id is null
group by
Class.Id, Class.Name

GO


