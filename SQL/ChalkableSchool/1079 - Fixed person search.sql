alter Procedure [dbo].[spSearchPersons]
	@start int,
	@count int,
	@filter nvarchar(50),
	@orderByFirstName bit,
	@schoolId int
as

Select
	count(*) as AllCount
from
	vwPerson
where
	(@filter is null or FirstName like @filter or LastName like @filter)
	and vwPerson.SchoolRef = @schoolId


Select
	*
from
	vwPerson
where
	(@filter is null or FirstName like @filter or LastName like @filter)
	and vwPerson.SchoolRef = @schoolId
order by 
	case when @orderByFirstName = 1 
		then FirstName 
		else LastName
	end
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY 
GO


