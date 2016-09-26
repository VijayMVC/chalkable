CREATE Procedure [dbo].[spGetClassesByTeachers]
	@schoolYearId int,
	@teacherIds TInt32 readonly
As

declare @teacherIdsCount int = (select count(*) from @teacherIds)
declare @classes TClassDetails

insert into @classes
select 
	vwClass.*,
	(select count(*) from ClassPerson Where ClassRef = vwClass.Class_Id) as Class_StudentsCount 
from 
	vwClass join ClassTeacher
		on vwClass.Class_Id = ClassTeacher.ClassRef
where 
	Class_SchoolYearRef = @schoolYearId
	And (@teacherIdsCount = 0 or  ClassTeacher.PersonRef in(select * from @teacherIds))

exec spSelectClassDetails @classes