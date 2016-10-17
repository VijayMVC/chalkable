

Create Procedure spGetStudentClasses
@schoolYearId int,
@personId int,
@markingPeriodId int
as

declare @class TClassDetails

insert into @class
select vwClass.*, (select count(*) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
from vwClass
where
Class_SchoolYearRef = @schoolYearId
and exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @personId)
and (@markingPeriodId is null or exists(select * from MarkingPeriodClass where ClassRef = Class_Id and MarkingPeriodRef = @markingPeriodId))

exec spSelectClassDetails @class