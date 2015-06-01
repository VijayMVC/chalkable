create procedure spAssigAllToGroup @groupId int, @now datetime2
as
begin transaction  

	insert into StudentGroup
	select @groupId, StudentRef 
	from StudentSchoolYear
	join 
	(
		select *, ROW_NUMBER() over(partition by SchoolRef order by StartDate desc) as rowNumber 
		from SchoolYear
		where  StartDate <= @now
	) schoolYear on schoolYear.Id = StudentSchoolYear.SchoolYearRef and schoolYear.rowNumber = 1
	where StudentSchoolYear.EnrollmentStatus = 0

commit transaction 
Go