create view [dbo].[vwClass]
as
select 
	Class.Id as Class_Id,
	Class.Name as Class_Name,
	Class.Description as Class_Description,
	Class.SchoolYearRef as Class_SchoolYearRef,
	Class.CourseRef as Class_CourseRef,
	Class.TeacherRef as Class_TeacherRef,
	Class.GradeLevelRef as Class_GradeLevelRef,
	MarkingPeriodClass.Id as MarkingPeriodClass_Id,
	MarkingPeriodClass.MarkingPeriodRef as MarkingPeriodClass_MarkingPeriodRef,
	MarkingPeriodClass.ClassRef as MarkingPeriodClass_ClassRef,
	Course.Id as Course_Id,
	Course.Code as Course_Code,
	Course.Title as Course_Title,
	Course.ChalkableDepartmentRef as Course_ChalkableDepartmentId,
	GradeLevel.Id as GradeLevel_Id,
	GradeLevel.Name as GradeLevel_Name,
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
	left join MarkingPeriodClass on Class.Id = MarkingPeriodClass.Classref
GO

create procedure [dbo].[spGetClasses] @schoolYearId uniqueidentifier, @markingPeriodId uniqueidentifier, @callerId uniqueidentifier, 
									 @personId uniqueidentifier, @start int, @count int, @classId uniqueidentifier
as

declare @callerSchoolId int,  @callerRoleId int
select @callerRoleId = RoleRef from Person where Id = @callerId
declare @roleId int = (select top 1 RoleRef from Person where Id = @personId)

select * from
(
select 
	vwClass.*,
	ROW_NUMBER() OVER(ORDER BY Class_Id) as RowNumber,
	(select count(Id) from ClassPerson where ClassRef = Class_Id) as StudentsCount
from 
	vwClass
where
	(@classId is null or Class_Id = @classId)
	and (@schoolYearId is null or Class_SchoolYearRef = @schoolYearId)
	and (@markingPeriodId is null or MarkingPeriodClass_MarkingPeriodRef = @markingPeriodId)	
	and (@callerRoleId = 1 or @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2
		or (@callerRoleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @callerId))
		)
	and (@personId is null or (@roleId = 2 and Class_TeacherRef = @personId)
		  or @roleId = 5 or  @roleId = 7 or  @roleId = 8 or @roleId = 1
		  or (@roleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @personId))
		  
		)
)x
where 
	x.RowNumber > @start
	and x.RowNumber <= @start + @count
order by x.RowNumber

GO
