
CREATE Procedure [dbo].[spSearchStudentsForGroup]
@groupId Int,
@schoolYearId Int,
@gradeLevelId Int,
@classesIds TInt32 ReadOnly,
@coursesIds TInt32 ReadOnly
As

Declare @classesIdsT Table(value Int);
If Exists(Select * From @coursesIds)
Begin
Insert Into
@classesIdsT(value)
select
Class.Id
from
Class join (Select value as CourseId from @coursesIds) course
on
course.CourseId = Class.CourseRef
Where
SchoolYearRef = @schoolYearId
Group By
Class.Id
End

If Exists(Select * From @classesIds)
Begin
Insert Into
@classesIdsT(value)
Select
value
From
@classesIds
End

Select Student.*,
StudentGroup.*
From
Student	Join StudentSchoolYear
On
StudentSchoolYear.StudentRef = Student.Id left join StudentGroup
On
StudentGroup.StudentRef = StudentSchoolYear.StudentRef and StudentGroup.GroupRef = @groupId
Where
StudentSchoolYear.SchoolYearRef = @schoolYearId
and StudentSchoolYear.GradeLevelRef = @gradeLevelId
and StudentSchoolYear.EnrollmentStatus = 0
and ( (not exists(select * from @classesIds) and not exists(select * from @coursesIds))
or exists(Select
*
From
ClassPerson Join @classesIdsT c
On
c.value = ClassPerson.ClassRef
Where
ClassPerson.PersonRef = Student.Id))