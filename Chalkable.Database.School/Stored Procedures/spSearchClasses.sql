
Create Procedure spSearchClasses
@filter1 nvarchar(max), @filter2 nvarchar(max), @filter3 nvarchar(max)
as

declare @class TClassDetails

insert into
	@class
select
	vwClass.*, (select count(*) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
from
	vwClass
where
	(@filter1 is null and @filter2 is null and @filter3 is null) or
	(@filter1 is not null and Class_Name like @filter1 or
	@filter2 is not null and Class_Name like @filter2 or
	@filter3 is not null and Class_Name like @filter3) and
	SchoolYear_Id is not null and
	Class_PrimaryTeacherRef is not null

exec spSelectClassDetails @class