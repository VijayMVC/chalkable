Create Procedure [dbo].[spGetStudentDetails] 
	@id int, @schoolYearId int
as begin
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
		when StudentSchoolYear.EnrollmentStatus = 0 then 0
		else 1
	end as bit) as IsWithdrawn
from 
	Student
    join StudentSchoolYear on Student.Id = StudentSchoolYear.StudentRef
where
	Student.id = @id
	and StudentSchoolYear.SchoolYearRef = @schoolYearId
	
end