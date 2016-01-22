
CREATE Procedure [dbo].[spSearchPersons]
@start int,
@count int,
@filter1 nvarchar(50),
@filter2 nvarchar(50),
@filter3 nvarchar(50),
@orderByFirstName bit,
@schoolId int
as

Select
count(*) as AllCount
From
vwPerson
Where
(@filter1 is not null and (FirstName like @filter1 or LastName like @filter1))
and (@filter2 is null or (FirstName like @filter2 or LastName like @filter2))
and (@filter3 is null or (FirstName like @filter3 or LastName like @filter3))
and vwPerson.SchoolRef = @schoolId

Select
*
From
vwPerson
Where
(@filter1 is not null and (FirstName like @filter1 or LastName like @filter1))
and (@filter2 is null or (FirstName like @filter2 or LastName like @filter2))
and (@filter3 is null or (FirstName like @filter3 or LastName like @filter3))
and vwPerson.SchoolRef = @schoolId
Order By
Case When
@orderByFirstName = 1
Then
FirstName
Else
LastName
End
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY