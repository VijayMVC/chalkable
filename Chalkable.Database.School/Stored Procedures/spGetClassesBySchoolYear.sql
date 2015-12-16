﻿CREATE Procedure [dbo].[spGetClassesBySchoolYear]
	@schoolYearId int,
	@filter nvarchar(max),
	@start int,
	@count int,
	@teacherId int
As

declare @classes TClassDetails;

select count(distinct Class_Id) as AllCount
from 
	vwClass left join ClassTeacher
		on vwClass.Class_Id = ClassTeacher.ClassRef
where 
	vwClass.SchoolYear_Id = @schoolYearId
	And (@filter is null or vwClass.Class_Name like(@filter))
	And (@teacherId is null or ClassTeacher.PersonRef = @teacherId)

insert into @classes
select vwClass.*, (select count(*) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
from 
	vwClass left join ClassTeacher
		on vwClass.Class_Id = ClassTeacher.ClassRef
where 
	vwClass.SchoolYear_Id = @schoolYearId
	And (@filter is null or vwClass.Class_Name like(@filter))
	And (@teacherId is null or ClassTeacher.PersonRef = @teacherId)
Order By vwClass.Class_Name
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

exec spSelectClassDetails @classes