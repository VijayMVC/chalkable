Create Procedure spGetStudentsCountByAcadYear
	@acadYear int
As

select
	count(distinct ssy.StudentRef) as StudentsCount
from 
	School s join SchoolYear sy
		on s.Id = sy.SchoolRef
	left join StudentSchoolYear ssy
		on ssy.SchoolYearRef = sy.Id and ssy.EnrollmentStatus = 0
where 
	s.IsChalkableEnabled = 1
	And s.IsActive = 1
	And sy.AcadYear = @acadYear
	And sy.ArchiveDate is null
	And StartDate is not null
	And EndDate is not null

Go