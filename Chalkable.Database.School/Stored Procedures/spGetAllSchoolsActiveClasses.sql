
CREATE Procedure spGetAllSchoolsActiveClasses
as

Declare @class TClassDetails

Insert Into
@class
Select
vwAllSchoolsActiveClasses.*, (select count(*) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
From
vwAllSchoolsActiveClasses

exec spSelectClassDetails @class