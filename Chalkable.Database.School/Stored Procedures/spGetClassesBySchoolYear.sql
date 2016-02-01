Create Procedure [dbo].[spGetClassesBySchoolYear]
	@schoolYearId int,
	@filter nvarchar(max),
	@start int,
	@count int,
	@teacherId int,
	@sortType int
As

declare @classes TClassDetails;
declare @ClassAsc int = 0,
        @ClassDesc int = 1,
        @TeacherAsc int = 2,
        @TeacherDesc int = 3,
        @StudentsAsc int = 4,
        @StudentsDesc int = 5

if @sortType is null or @sortType > 5 or @sortType < 0
	set @sortType = @ClassAsc

insert into @classes
select * from 
(
	select 
		vwClass.*, 
		(select count(*) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
	from 
		vwClass 
		left join ClassTeacher
			on vwClass.Class_Id = ClassTeacher.ClassRef
	where 
		vwClass.SchoolYear_Id = @schoolYearId
		And (@filter is null 
			 or vwClass.Class_Name like(@filter) 
			 or vwClass.Person_FirstName like(@filter) 
			 or vwClass.Person_LastName like(@filter))
		And (@teacherId is null or ClassTeacher.PersonRef = @teacherId)
) as x
Order By
	case when @sortType = @StudentsAsc then Class_StudentsCount end,
	case when @sortType = @StudentsDesc then Class_StudentsCount end desc,
	case when @sortType = @ClassAsc then Class_Name end asc,
	case when @sortType = @ClassDesc then Class_Name end desc,
----Teachers are sorted by last name alphabetically and then by first name.
	case when @sortType = @TeacherAsc then Person_LastName end asc,
	case when @sortType = @TeacherAsc then Person_FirstName end asc,
	case when @sortType = @TeacherDesc then Person_LastName end desc,
	case when @sortType = @TeacherDesc then Person_FirstName end desc
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

exec spSelectClassDetails @classes