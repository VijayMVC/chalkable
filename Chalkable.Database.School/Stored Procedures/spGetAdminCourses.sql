

create procedure [dbo].[spGetAdminCourses] @schoolYearId int, @gradeLevelId int
as

declare @class TClassDetails

insert into @class
select vwClass.*, (select count(*) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
from vwClass
where
Class_SchoolYearRef = @schoolYearId
and @gradeLevelId between Class_MinGradeLevelRef and Class_MaxGradeLevelRef

/* select Courses */
select course.* from Class course
join Class c on c.CourseRef = course.Id
where course.CourseRef is null
and c.SchoolYearRef = @schoolYearId
and @gradeLevelId between c.MinGradeLevelRef and c.MaxGradeLevelRef

exec spSelectClassDetails @class