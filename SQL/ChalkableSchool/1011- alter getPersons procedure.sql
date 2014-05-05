ALTER PROCEDURE [dbo].[spGetPersons] @schoolId int, @personId int, @callerId int, @roleId int, @start int, @count int, @startFrom nvarchar(255)
	, @teacherId int, @classId int, @filter1 nvarchar(255), @filter2 nvarchar(255), @filter3 nvarchar(255)
	, @gradeLevelIds nvarchar(1024), @sortType int, @callerRoleId int, @markingPeriodId int
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
		Id in (select ClassPerson.PersonRef from ClassPerson 
				join Class on ClassPerson.ClassRef = Class.Id 
				where Class.TeacherRef = @teacherId and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)
							))
	and (@classId is null or ((@roleId is null or @roleId = 3) and Id in (select PersonRef from ClassPerson 
																	      where ClassPerson.ClassRef = @classId
																			    and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)
																		 )
							  )
						  or ((@roleId is null or @roleId = 2) and Id in (select TeacherRef from Class where Id = @classId))
		)
	and (@callerRoleId = 1 or (vwPerson.SchoolRef = @schoolId and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
			or(@callerRoleId = 3 and (Id = @callerId   or (RoleRef = 2 or RoleRef = 5 or RoleRef = 7 or RoleRef = 8 
											   or (RoleRef = 3 and exists(select * from StudentSchoolYear where StudentRef = vwPerson.Id and GradeLevelRef = @callerGradeLevelId))))
			   )
			or(@callerRoleId = 6 and (Id = @callerId or RoleRef = 3))
		)))	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
		 or FirstName like @filter2 or LastName like @filter2
		 or FirstName like @filter3 or LastName like @filter3)
	and (
			@gradeLevelIds is null 
			or (vwPerson.RoleRef = 3 and exists(select * from StudentSchoolYear ssy
												join @glIds gl on gl.id = ssy.GradeLevelRef
												where ssy.StudentRef = vwPerson.Id))
			or (vwPerson.RoleRef = 2 and exists
				(select * from Class where Class.TeacherRef = vwPerson.Id and Class.GradeLevelRef in (select id from @glIds))
			)
		)

-- Sort Type
-- 0 : by FisrtName
-- 1 : by LastName
--------------------			
					
select vwPerson.*
from vwPerson
where
	(@personId is null or  Id = @personId)
	and (@roleId is null or RoleRef = @roleId)
	and (@startFrom is null or LastName >= @startFrom)
	and (@teacherId is null or 
		Id in (select ClassPerson.PersonRef from 
							ClassPerson 
							join Class on ClassPerson.ClassRef = Class.Id 
							where Class.TeacherRef = @teacherId and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)
							))
	and (@classId is null or ((@roleId is null or @roleId = 3) and Id in (select PersonRef from ClassPerson 
																		 where ClassPerson.ClassRef = @classId 
																		 and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)))
							or ((@roleId is null or @roleId = 2) and Id in (select TeacherRef from Class where Id = @classId))
		)
	and (@callerRoleId = 1 or (vwPerson.SchoolRef = @schoolId and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
			or(@callerRoleId = 3 and (Id = @callerId 
											or (RoleRef = 2 or RoleRef = 5 or RoleRef = 7 or RoleRef = 8 
												or (RoleRef = 3 and exists(select * from StudentSchoolYear where StudentRef = vwPerson.Id and GradeLevelRef = @callerGradeLevelId))))
				)
			or(@callerRoleId = 6 and (Id = @callerId or RoleRef = 3))
		)))	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
			or FirstName like @filter2 or LastName like @filter2
			or FirstName like @filter3 or LastName like @filter3)
	and (
			@gradeLevelIds is null 
			or (vwPerson.RoleRef = 3 and exists(select * from StudentSchoolYear ssy
												join @glIds gl on gl.id = ssy.GradeLevelRef
												where ssy.StudentRef = vwPerson.Id))
			or (vwPerson.RoleRef = 2 and exists
				(select * from Class where Class.TeacherRef = vwPerson.Id and Class.GradeLevelRef in (select id from @glIds))
			)
		)
						
order by  case when @sortType = 0 then FirstName  else LastName end
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
GO






