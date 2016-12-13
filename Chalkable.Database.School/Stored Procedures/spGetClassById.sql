

Create Procedure spGetClassById
@id int
as

declare @class TClassDetails

insert into @class
select vwClass.*, (select count(*) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
from vwClass
where
Class_Id = @id

exec spSelectClassDetails @class