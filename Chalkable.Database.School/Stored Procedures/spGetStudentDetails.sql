CREATE Procedure [dbo].[spGetStudentDetails]
	@id int, 
	@schoolYearId int
As

Select
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
	Cast(Case When StudentSchoolYear.EnrollmentStatus = 0 Then 0 Else 1 End As Bit) As IsWithdrawn
From
	Student
	Left Join StudentSchoolYear 
		On Student.Id = StudentSchoolYear.StudentRef and StudentSchoolYear.SchoolYearRef = @schoolYearId
Where
	Student.id = @id

Select
	PersonEthnicity.*
From 
	PersonEthnicity
Where 
	PersonEthnicity.PersonRef = @id

GO