ALTER procedure [dbo].[spGetClasses] @schoolYearId uniqueidentifier, @markingPeriodId uniqueidentifier, @callerId uniqueidentifier, @callerRoleId int,
@personId uniqueidentifier, @start int, @count int, @classId uniqueidentifier,
@filter1 nvarchar(max), @filter2 nvarchar(max), @filter3 nvarchar(max)
as

declare @callerSchoolId int
declare @roleId int = (select top 1 RoleRef from Person where Id = @personId)

declare @class table
(
Class_Id uniqueidentifier,
Class_Name nvarchar(255),
Class_Description  nvarchar(1024),
Class_SchoolYearRef uniqueidentifier,
Class_CourseRef uniqueidentifier,
Class_TeacherRef uniqueidentifier,
Class_GradeLevelRef uniqueidentifier,
Class_SisId int,
Course_Id uniqueidentifier,
Course_Code  nvarchar(255),
Course_Title  nvarchar(255),
Course_ChalkableDepartmentRef uniqueidentifier,
GradeLevel_Id uniqueidentifier,
GradeLevel_Name nvarchar(255),
Person_Id uniqueidentifier,
Person_FirstName nvarchar(255),
Person_LastName nvarchar(255),
Person_Gender nvarchar(255),
Person_Salutation nvarchar(255),
Person_Email nvarchar(256),
Class_StudentsCount int
)

select Count(*) as SourceCount
from
vwClass
where
(@classId is null or Class_Id = @classId)
and (@schoolYearId is null or Class_SchoolYearRef = @schoolYearId)
and (@markingPeriodId is null or exists(select * from MarkingPeriodClass where ClassRef = Class_Id and MarkingPeriodRef = @markingPeriodId))
and (@callerRoleId = 1 or @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2
or (@callerRoleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @callerId))
)
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
select
vwClass.*,
(select count(Id) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
from
vwClass
where
(@classId is null or Class_Id = @classId)
and (@schoolYearId is null or Class_SchoolYearRef = @schoolYearId)
and (@markingPeriodId is null or exists(select * from MarkingPeriodClass where ClassRef = Class_Id and MarkingPeriodRef = @markingPeriodId))
and (@callerRoleId = 1 or @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2
or (@callerRoleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @callerId))
)
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


