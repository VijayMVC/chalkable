Create PROCEDURE [dbo].[spGetShortStaffSummary]
	@schoolYearId int,
	@filter nvarchar(max),
	@start int,
	@count int,
	@sortType int
AS

declare @TeacherAsc int = 0,
        @TeacherDesc int = 1,
        @StudentsAsc int = 2,
        @StudentsDesc int = 3

if @sortType is null or @sortType < 0 or @sortType > 3
	set @sortType = @TeacherAsc

set @sortType = @TeacherDesc

select * from
(
	Select 
		Staff.*, 
		count(distinct ClassPerson.PersonRef) as StudentsCount
	From 
		Staff 
		left join ClassTeacher
			on Staff.Id = ClassTeacher.PersonRef
		left join Class
			on ClassTeacher.ClassRef = Class.Id
		left join ClassPerson
			on ClassTeacher.ClassRef = ClassPerson.ClassRef
	Where 
		(@filter is null or FirstName like(@filter) or LastName like(@filter))
		and Class.SchoolYearRef = @schoolYearId
	Group by
		Staff.[Id],
		Staff.[FirstName],
		Staff.[LastName],
		Staff.[BirthDate],
		Staff.[Gender],
		Staff.[UserId]
) as x
Order by 
	case when @sortType = @StudentsAsc then StudentsCount end asc,
	case when @sortType = @StudentsDesc then StudentsCount end desc,
----Teachers are sorted by last name alphabetically and then by first name.
	case when @sortType = @TeacherAsc then LastName end asc,
	case when @sortType = @TeacherAsc then FirstName end asc,
	case when @sortType = @TeacherDesc then LastName end desc,
	case when @sortType = @TeacherDesc then FirstName end desc
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
