Create Procedure spSearchPersons
	@start int,
	@count int,
	@filter nvarchar(50),
	@orderByFirstName bit
as

Select
	count(*) as AllCount
from
	vwPerson
where
	(@filter is null or FirstName like @filter or LastName like @filter)



Select
	*
from
	vwPerson
where
	(@filter is null or FirstName like @filter or LastName like @filter)
order by 
	case when @orderByFirstName = 1 
		then FirstName 
		else LastName
	end
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY 