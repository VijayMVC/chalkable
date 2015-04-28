Create Procedure spGetStudentsByClass 
	@classId int, 
	@markingPeriodId int, 
	@isEnrolled bit
as begin
declare @enrollmentStatus int
if @isEnrolled is not null
begin
	if @isEnrolled = 1
		set @enrollmentStatus = 0
	else
		set @enrollmentStatus = 1	
end
select
	Student.Id,
	Student.FirstName,
	Student.LastName,
	Student.BirthDate,
	Student.Gender,
	Student.HasMedicalAlert,
	Student.IsAllowedInetAccess,
	Student.SpecialInstructions,
	Student.SpEdStatus,
	Student.PhotoModifiedDate,
	Student.UserId,
	cast(case		
		when ClassPerson.IsEnrolled = 1 and StudentSchoolYear.EnrollmentStatus = 0 then 0
		else 1
	end as bit)as IsWithdrawn
from 
	Student
    join ClassPerson on Student.Id = ClassPerson.PersonRef
    join MarkingPeriod on ClassPerson.MarkingPeriodRef = MarkingPeriod.Id
    join StudentSchoolYear on ClassPerson.PersonRef = StudentSchoolYear.StudentRef and StudentSchoolYear.SchoolYearRef = MarkingPeriod.SchoolYearRef
where
	ClassPerson.ClassRef = @classId
	and MarkingPeriod.Id = @markingPeriodId
	and (@isEnrolled is null or ClassPerson.IsEnrolled = @isEnrolled)
	and (@isEnrolled is null or StudentSchoolYear.EnrollmentStatus = @enrollmentStatus)

end
