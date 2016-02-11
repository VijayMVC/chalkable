Create Procedure [dbo].[spGetShortSchoolSummaries]
	@filter nvarchar(max),
	@start int,
	@count int
As


select
	count(distinct s.Id) as AllCount
From
	School s join SchoolYear sy
		on s.Id = sy.SchoolRef
	left join StudentSchoolYear ssy
		on (sy.Id = ssy.SchoolYearRef) and (ssy.EnrollmentStatus is null or ssy.EnrollmentStatus = 0)
Where
	--s.IsChalkableEnabled = 1
	--AND s.IsActive = 1
	--AND sy.ArchiveDate is null
	--AND sy.AcadYear = @acadYear AND
	--AND sy.StartDate is not null
	--AND sy.EndDate is not null
	 (@filter is null or s.Name like(@filter))

Select 
	s.*,
	count(distinct ssy.StudentRef) as StudentsCount
From
	School s join SchoolYear sy
		on s.Id = sy.SchoolRef
	left join StudentSchoolYear ssy
		on (sy.Id = ssy.SchoolYearRef) and (ssy.EnrollmentStatus is null or ssy.EnrollmentStatus = 0)
Where
	--s.IsChalkableEnabled = 1
	--AND s.IsActive = 1
	--AND sy.ArchiveDate is null
	--AND sy.AcadYear = @acadYear
	--AND sy.StartDate is not null
	--AND sy.EndDate is not null
	(@filter is null or s.Name like(@filter))
Group By
	s.Id,
	s.Name,
	s.IsActive,
	s.IsPrivate,
	s.IsChalkableEnabled,
	s.IsLEEnabled,
	s.IsLESyncComplete
Order By s.Name
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY



GO


