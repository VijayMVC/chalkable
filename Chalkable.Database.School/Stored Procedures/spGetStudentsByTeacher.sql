
CREATE Procedure [dbo].[spGetStudentsByTeacher]
@teacherId int,
@schoolYearId int
as begin

select
Student.*,
cast(case
when StudentSchoolYear.EnrollmentStatus = 0 then 0
else 1
end as bit) as IsWithdrawn
from
Student
join ClassPerson on Student.Id = ClassPerson.PersonRef
join MarkingPeriod on ClassPerson.MarkingPeriodRef = MarkingPeriod.Id
join StudentSchoolYear on ClassPerson.PersonRef = StudentSchoolYear.StudentRef and StudentSchoolYear.SchoolYearRef = MarkingPeriod.SchoolYearRef
where
MarkingPeriod.SchoolYearRef = @schoolYearId
and ClassPerson.ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where ClassTeacher.PersonRef = @teacherId)
end