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
	Class_ChalkableDepartmentId int,
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







