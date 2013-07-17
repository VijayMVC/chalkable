ALTER view [dbo].[vwClass]
as
select 
	Class.Id as Class_Id,
	Class.Name as Class_Name,
	Class.Description as Class_Description,
	Class.SchoolYearRef as Class_SchoolYearRef,
	Class.CourseRef as Class_CourseRef,
	Class.TeacherRef as Class_TeacherRef,
	Class.GradeLevelRef as Class_GradeLevelRef,
	Course.Id as Course_Id,
	Course.Code as Course_Code,
	Course.Title as Course_Title,
	Course.ChalkableDepartmentRef as Course_ChalkableDepartmentId,
	GradeLevel.Id as GradeLevel_Id,
	GradeLevel.Name as GradeLevel_Name,
	Person.Id as Person_Id,
	Person.FirstName as Person_FirstName,
	Person.LastName as Person_LastName,
	Person.Gender as Person_Gender,
	Person.Salutation as Person_Salutation,
	Person.Email as Person_Email 
from 
	Class	
	join Course on Class.CourseRef = Course.Id
	join GradeLevel on GradeLevel.Id = Class.GradeLevelRef
	join Person on Person.Id = Class.TeacherRef

GO




ALTER procedure [dbo].[spGetClasses] @schoolYearId uniqueidentifier, @markingPeriodId uniqueidentifier, @callerId uniqueidentifier, 
									 @personId uniqueidentifier, @start int, @count int, @classId uniqueidentifier
as

declare @callerSchoolId int,  @callerRoleId int
select @callerRoleId = RoleRef from Person where Id = @callerId
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
	Course_Id uniqueidentifier,
	Course_Code  nvarchar(255),
	Course_Title  nvarchar(255),
	Course_ChalkableDepartmentId uniqueidentifier,
	GradeLevel_Id uniqueidentifier,
	GradeLevel_Name nvarchar(255),
	Person_Id uniqueidentifier,
	Person_FirstName nvarchar(255),
	Person_LastName nvarchar(255),
	Person_Gender nvarchar(255),
	Person_Salutation nvarchar(255),
	Person_Email nvarchar(256),
	StudentsCount int
)

insert into @class
select
	vwClass.*,
	(select count(Id) from ClassPerson where ClassRef = Class_Id) as StudentsCount
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
order by vwClass.Class_Id
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY


select * from @class

select mpc.* from MarkingPeriodClass mpc
join @class c on c.Class_Id = mpc.ClassRef

GO


