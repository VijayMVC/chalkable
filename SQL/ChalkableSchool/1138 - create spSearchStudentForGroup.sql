create procedure [dbo].[spSearchStudentsForGroup] @groupId int, @schoolYearId int, @gradeLevelId int, @classesIds nvarchar(max), @coursesIds nvarchar(max)
as

declare @classesIdsT table(value int);	
if(@coursesIds is not null)
begin
	insert into @classesIdsT(value)
	select Class.Id from Class
	join (select cast(s as int) as CourseId from dbo.split(',', @coursesIds)) course on course.CourseId = Class.CourseRef
	where SchoolYearRef = @schoolYearId
	group by Class.Id 
end

if(@classesIds is not null)
begin
	insert into @classesIdsT(value)
	select cast(s as int) from dbo.split(',', @classesIds)
end

select Student.*,
	   StudentGroup.*
from Student
join StudentSchoolYear on StudentSchoolYear.StudentRef = Student.Id
left join StudentGroup on StudentGroup.StudentRef = StudentSchoolYear.StudentRef and StudentGroup.GroupRef = @groupId
where StudentSchoolYear.SchoolYearRef = @schoolYearId and StudentSchoolYear.GradeLevelRef = @gradeLevelId
	  and StudentSchoolYear.EnrollmentStatus = 0
	  and (@classesIds is null or @coursesIds is null or exists(select * from ClassPerson 
										 join @classesIdsT c on c.value = ClassPerson.ClassRef
										 where ClassPerson.PersonRef = Student.Id))

GO


