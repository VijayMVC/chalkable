Create Procedure spGetSchoolsCountByAcadYear
	@acadYear int
As

select
	count(distinct s.[Id]) as SchoolsCount
from 
	School s join SchoolYear sy
		on s.Id = sy.SchoolRef
where 
	s.IsChalkableEnabled = 1
	And s.IsActive = 1
	And sy.AcadYear = @acadYear
	And sy.ArchiveDate is null
	And StartDate is not null
	And EndDate is not null

Go