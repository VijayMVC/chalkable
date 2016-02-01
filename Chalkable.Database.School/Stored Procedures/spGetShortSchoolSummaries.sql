Create Procedure [dbo].[spGetShortSchoolSummaries]
	@filter nvarchar(max),
	@start int,
	@count int,
	@sortType int
As

declare @SchoolAsc int = 0,
        @SchoolDesc int = 1,
        @StudentsAsc int = 2,
        @StudentsDesc int = 3

if @sortType is null or @sortType < 0 or @sortType > 3
	set @sortType = 0
	
set @sortType = 2 

select * from
(
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
		s.IsActive = 1
		--AND sy.ArchiveDate is null
		--AND sy.AcadYear = @acadYear
		--AND sy.StartDate is not null
		--AND sy.EndDate is not null
		AND (@filter is null or s.Name like(@filter))
	Group By
		s.Id,
		s.Name,
		s.IsActive,
		s.IsPrivate,
		s.IsChalkableEnabled,
		s.IsLEEnabled,
		s.IsLESyncComplete
) as x
Order By 
	case when @sortType = @SchoolAsc then Name end asc,
	case when @sortType = @SchoolDesc then Name end desc,
	case when @sortType = @StudentsAsc then StudentsCount end asc,
	case when @sortType = @StudentsDesc then StudentsCount end desc
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY