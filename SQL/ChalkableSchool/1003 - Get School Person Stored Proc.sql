CREATE procedure [dbo].[spGetPersons]
	@personId int, @callerId int, @roleId int, @start int, @count int, @startFrom nvarchar(255)
	, @teacherId int, @classId int, @filter1 nvarchar(255), @filter2 nvarchar(255), @filter3 nvarchar(255)
	, @gradeLevelIds nvarchar(1024), @sortType int
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


declare @callerRoleId int = (select RoleRef from SchoolPerson where Id = @callerId);
declare @callerGradeLevelId int = null;
if(@callerRoleId = 3)
begin
	set @callerGradeLevelId = (select GradeLevelRef from StudentInfo where SchoolPersonRef = @callerId)
end
	
declare @allCount int;
set @allCount = 
(select COUNT(*) from vwSchoolPerson
where
	(@personId is null or  PersonId = @personId)
	and (@roleId is null or RoleId = @roleId)
	and (@startFrom is null or LastName >= @startFrom)
	and (@teacherId is null or 
		PersonId in (select ClassSchoolPerson.SchoolPersonRef from 
							ClassSchoolPerson 
							join Class on ClassSchoolPerson.ClassRef = Class.Id 
							join StaffCourse on StaffCourse.Id = Class.StaffCourseRef
							where StaffCourse.SchoolPersonRef = @teacherId
							))
	and (@classId is null or ((@roleId is null or @roleId = 3) and SchoolPersonId in (select SchoolPersonRef from ClassSchoolPerson where ClassSchoolPerson.ClassRef = @classId))
						  or ((@roleId is null or @roleId = 2) and SchoolPersonId in (select sc.SchoolPersonRef from StaffCourse sc 
																				      join Class c on c.StaffCourseRef = sc.Id
																					  where c.Id = @classId))
		)
	and (@callerRoleId = 1 or @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
		or(@callerRoleId = 3 and (SchoolPersonId = @callerId 
									   or (RoleId = 2 or RoleId = 5 or RoleId = 7 or RoleId = 8 
										   or (RoleId = 3 and GradeLevelId = @callerGradeLevelId)))
		   )
		or(@callerRoleId = 6 and (SchoolPersonId = @callerId or RoleId = 3))
		)	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
		 or FirstName like @filter2 or LastName like @filter2
		 or FirstName like @filter3 or LastName like @filter3)
	and (
			@gradeLevelIds is null 
			or (vwSchoolPerson.RoleId = 3 and exists(select * from @glIds as x where x.id = GradeLevelId))
			or (vwSchoolPerson.RoleId = 2 and exists
				(select * from Class join StaffCourse on Class.StaffCourseRef = StaffCourse.Id 
				where StaffCourse.SchoolPersonRef = vwSchoolPerson.SchoolPersonId and Class.GradeLevelRef in (select id from @glIds))
			)
		)
)						

-- Sort Type
-- 0 : by FisrtName
-- 1 : by LastName
--------------------			
select * from
(						
	select 
		vwSchoolPerson.*,
		@allCount as AllCount,
		ROW_NUMBER() OVER(ORDER BY case when @sortType = 0 then vwSchoolPerson.FirstName else vwSchoolPerson.LastName end) as RowNumber
	from vwSchoolPerson
	where
		(@personId is null or  SchoolPersonId = @personId)
		and (@roleId is null or Roleid = @roleId)
		and (@startFrom is null or LastName >= @startFrom)
		and (@teacherId is null or 
			SchoolPersonId in (select ClassSchoolPerson.SchoolPersonRef from 
								ClassSchoolPerson 
								join Class on ClassSchoolPerson.ClassRef = Class.Id 
								join StaffCourse on StaffCourse.Id = Class.StaffCourseRef
								where StaffCourse.SchoolPersonRef = @teacherId
								))
		and (@classId is null or ((@roleId is null or @roleId = 3) and SchoolPersonId in (select SchoolPersonRef from ClassSchoolPerson where ClassSchoolPerson.ClassRef = @classId))
								or ((@roleId is null or @roleId = 2) and SchoolPersonId in (select sc.SchoolPersonRef from StaffCourse sc 
																							join Class c on c.StaffCourseRef = sc.Id
																							where c.Id = @classId))
			)
		
		and (@callerRoleId = 1 or @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
			 or( @callerRoleId = 3 and (SchoolPersonId = @callerId 
									   or (RoleId = 2 or RoleId = 5 or RoleId = 7 or RoleId = 8 
										   or (RoleId = 3 and GradeLevelId = @callerGradeLevelId)))
				)
	  		 or(@callerRoleId = 6 and (SchoolPersonId = @callerId or RoleId = 3))
	
			)
		and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
		 or FirstName like @filter2 or LastName like @filter2
		 or FirstName like @filter3 or LastName like @filter3)
		and (
			@gradeLevelIds is null 
			or (vwSchoolPerson.RoleId = 3 and exists(select * from @glIds as x where x.id = GradeLevelId))
			or (vwSchoolPerson.RoleId = 2 and exists
				(select * from Class join StaffCourse on Class.StaffCourseRef = StaffCourse.Id 
				where StaffCourse.SchoolPersonRef = vwSchoolPerson.SchoolPersonId and Class.GradeLevelRef in (select id from @glIds))
			)
		)
) x
where
	x.RowNumber > @start
	and x.RowNumber <= @start + @count
order by  case when @sortType = 0 then x.FirstName  else x.LastName end 

GO


