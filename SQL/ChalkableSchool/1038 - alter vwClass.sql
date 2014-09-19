----------------------------
----------VIEWS-------------
----------------------------
alter VIEW [dbo].[vwClass]
AS
SELECT
	Class.Id as Class_Id,
	Class.ClassNumber as Class_ClassNumber,
	Class.Name as Class_Name,
	Class.Description as Class_Description,
	Class.SchoolYearRef as Class_SchoolYearRef,
	Class.PrimaryTeacherRef as Class_PrimaryTeacherRef,
	Class.GradeLevelRef as Class_GradeLevelRef,
	Class.ChalkableDepartmentRef as Class_ChalkableDepartmentRef,
	Class.SchoolRef as Class_SchoolRef,
	Class.CourseRef as Class_CourseRef,
	Class.RoomRef as Class_RoomRef,
	GradeLevel.Id as GradeLevel_Id,
	GradeLevel.Name as GradeLevel_Name,
	GradeLevel.Number as GradeLevel_Number,
	Person.Id as Person_Id,
	Person.FirstName as Person_FirstName,
	Person.LastName as Person_LastName,
	Person.Gender as Person_Gender,
	Person.Salutation as Person_Salutation,
	Person.Email as Person_Email,
	Person.AddressRef as Person_AddressRef,
	SchoolPerson.RoleRef as Person_RoleRef,
	SchoolYear.SchoolRef as Class_SchoolId 
FROM 
	Class	
	join GradeLevel on GradeLevel.Id = Class.GradeLevelRef
	left join Person on Person.Id = Class.PrimaryTeacherRef
	left join SchoolPerson on SchoolPerson.PersonRef = Class.PrimaryTeacherRef and SchoolPerson.SchoolRef = Class.SchoolRef
	left join SchoolYear on SchoolYear.Id = Class.SchoolYearRef
where Class.PrimaryTeacherRef is null or SchoolPerson.RoleRef is not null

GO


--Get Classes Procedure
alter procedure [dbo].[spGetClasses] @schoolId int, @schoolYearId int, @markingPeriodId int, @callerId int, @callerRoleId int,
										@personId int, @start int, @count int, @classId int, 
										@filter1 nvarchar(max), @filter2 nvarchar(max), @filter3 nvarchar(max)
as

declare @callerSchoolId int
declare @roleId int = (select top 1 RoleRef from SchoolPerson where PersonRef = @personId and SchoolRef = @schoolId)

declare @class table
(
	Class_Id int,
	Class_ClassNumber nvarchar(max),
	Class_Name nvarchar(max),
	Class_Description  nvarchar(max),
	Class_SchoolYearRef int,
	Class_PrimaryTeacherRef int,
	Class_GradeLevelRef int,
	Class_ChalkableDepartmentRef uniqueidentifier,
	Class_SchoolRef  int,
	Class_CourseRef int,
	Class_RoomRef int,
	GradeLevel_Id int,
	GradeLevel_Name nvarchar(max),
	GradeLevel_Number int,
	Person_Id int,
	Person_FirstName nvarchar(max),
	Person_LastName nvarchar(max),
	Person_Gender nvarchar(max),
	Person_Salutation nvarchar(max),
	Person_Email nvarchar(max),
	Person_AddressRef int,
	Person_RoleRef int,
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
and (@personId is null or (@roleId = 2 and exists(select * from ClassTeacher where ClassRef = Class_Id and PersonRef = @personId))
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
and (@personId is null or (@roleId = 2 and exists(select * from ClassTeacher where ClassRef = Class_Id and PersonRef = @personId))
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


