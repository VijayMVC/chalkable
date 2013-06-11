Create procedure [dbo].[spGetPersons]
	@personId uniqueidentifier, @callerId uniqueidentifier, @roleId int, @start int, @count int, @startFrom nvarchar(255)
	, @teacherId uniqueidentifier, @classId uniqueidentifier, @filter1 nvarchar(255), @filter2 nvarchar(255), @filter3 nvarchar(255)
	, @gradeLevelIds nvarchar(1024), @sortType int
as

declare @glIds table 
(
	id uniqueidentifier not null
) 
if @gradeLevelIds is not null
begin
	insert into @glIds
	select cast(s as uniqueidentifier) from dbo.split(',', @gradeLevelIds)
end


declare @callerRoleId int = (select RoleRef from Person where Id = @callerId);
declare @callerGradeLevelId uniqueidentifier = null;
if(@callerRoleId = 3)
begin
	set @callerGradeLevelId = (select GradeLevelRef from StudentInfo where PersonRef = @callerId)
end
	
select COUNT(*) as AllCount from vwPerson
where
	(@personId is null or  Id = @personId)
	and (@roleId is null or RoleId = @roleId)
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
	and (@callerRoleId = 1 or @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
		or(@callerRoleId = 3 and (Id = @callerId 
									   or (RoleId = 2 or RoleId = 5 or RoleId = 7 or RoleId = 8 
										   or (RoleId = 3 and GradeLevelId = @callerGradeLevelId)))
		   )
		or(@callerRoleId = 6 and (Id = @callerId or RoleId = 3))
		)	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
		 or FirstName like @filter2 or LastName like @filter2
		 or FirstName like @filter3 or LastName like @filter3)
	and (
			@gradeLevelIds is null 
			or (vwPerson.RoleId = 3 and exists(select * from @glIds as x where x.id = GradeLevelId))
			or (vwPerson.RoleId = 2 and exists
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
	and (@roleId is null or RoleId = @roleId)
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
	and (@callerRoleId = 1 or @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
		or(@callerRoleId = 3 and (Id = @callerId 
									   or (RoleId = 2 or RoleId = 5 or RoleId = 7 or RoleId = 8 
										   or (RoleId = 3 and GradeLevelId = @callerGradeLevelId)))
		   )
		or(@callerRoleId = 6 and (Id = @callerId or RoleId = 3))
		)	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
		 or FirstName like @filter2 or LastName like @filter2
		 or FirstName like @filter3 or LastName like @filter3)
	and (
			@gradeLevelIds is null 
			or (vwPerson.RoleId = 3 and exists(select * from @glIds as x where x.id = GradeLevelId))
			or (vwPerson.RoleId = 2 and exists
				(select * from Class where Class.TeacherRef = vwPerson.Id and Class.GradeLevelRef in (select id from @glIds))
			)
		)
						
) x
where
	x.RowNumber > @start
	and x.RowNumber <= @start + @count
order by  case when @sortType = 0 then x.FirstName  else x.LastName end 

GO


