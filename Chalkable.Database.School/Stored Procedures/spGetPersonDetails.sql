
CREATE PROCEDURE spGetPersonDetails 
	@schoolId int, 
	@personId int
As

Select
	*
From
	vwPerson
Where
	id = @personId
	and Schoolref = @schoolId

select top 1 a.* from [Address] a
	join Person p on p.AddressRef = a.Id
where p.Id = @personId

select * from Phone
where PersonRef = @personId

Select *
from StudentSchoolYear
join GradeLevel on GradeLevel.Id = StudentSchoolYear.GradeLevelRef
where StudentSchoolYear.StudentRef = @personId

select * from PersonEmail
where PersonRef = @personId