Create Procedure spGetClassesByTeachers
	@schoolYearId int,
	@teacherIds TInt32 readonly,
	@start int,
	@count int
As

declare @teacherIdsCount int = (select count(*) from @teacherIds)
declare @classes TClassDetails

insert into @classes
select 
	vwClass.*, 
	(select count(*) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
from 
	vwClass
where 
	Class_SchoolYearRef = @schoolYearId
	And (@teacherIdsCount = 0 or exists(select * from ClassTeacher where ClassRef = Class_Id and PersonRef in(select * from @teacherIds)))

exec spSelectClassDetails @classes

Go