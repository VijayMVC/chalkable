

CREATE Procedure [dbo].[spSearchStaff]
@start int,
@count int,
@classId int,
@studentId int,
@schoolYearId int,
@filter nvarchar(50),
@orderByFirstName bit
as
declare @schoolId int
set @schoolId = (select top 1 SchoolRef from SchoolYear where SchoolYear.Id = @schoolYearId)

select
count(*) as AllCount
from
(
Select
	Staff.Id,
	Staff.FirstName,
	Staff.LastName,
	Staff.BirthDate,
	Staff.Gender,
	Staff.UserId
From
	Staff
	join UserSchool on Staff.UserId = UserSchool.UserRef
	join ClassTeacher on Staff.Id = ClassTeacher.PersonRef
	join Class on Class.Id = ClassTeacher.ClassRef
Where
	UserSchool.SchoolRef = @schoolId and
	Class.SchoolYearRef = @schoolYearId and
	(@classId is null or ClassTeacher.ClassRef = @classId) and
	(@studentId is null or ClassTeacher.ClassRef in (select ClassPerson.ClassRef from ClassPerson where ClassPerson.PersonRef = @studentId)) and
	(@filter is null or FirstName like @filter or LastName like @filter)
Group by
	Staff.Id,
	Staff.FirstName,
	Staff.LastName,
	Staff.BirthDate,
	Staff.Gender,
	Staff.UserId
) X


Select
	Staff.Id,
	Staff.FirstName,
	Staff.LastName,
	Staff.BirthDate,
	Staff.Gender,
	Staff.UserId
From Staff
join UserSchool on Staff.UserId = UserSchool.UserRef
join ClassTeacher on Staff.Id = ClassTeacher.PersonRef
join Class on Class.Id = ClassTeacher.ClassRef
Where
	UserSchool.SchoolRef = @schoolId and
	Class.SchoolYearRef = @schoolYearId and
	(@classId is null or ClassTeacher.ClassRef = @classId) and
	(@studentId is null or ClassTeacher.ClassRef in (select ClassPerson.ClassRef from ClassPerson where ClassPerson.PersonRef = @studentId)) and
	(@filter is null or FirstName like @filter or LastName like @filter)
Group by
	Staff.Id,
	Staff.FirstName,
	Staff.LastName,
	Staff.BirthDate,
	Staff.Gender,
	Staff.UserId
Order by IIF(@orderByFirstName = 1, FirstName, LastName) asc,
		 IIF(@orderByFirstName <> 1, FirstName, LastName) asc

OFFSET @start ROWS FETCH NEXT @count ROWS ONLY