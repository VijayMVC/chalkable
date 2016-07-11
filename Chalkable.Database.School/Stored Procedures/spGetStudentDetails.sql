CREATE Procedure [dbo].[spGetStudentDetails]
	@id int, 
	@schoolYearId int
As

Select
	Student.*,
	Cast(Case When StudentSchoolYear.EnrollmentStatus = 0 Then 0 Else 1 End As Bit) As IsWithdrawn
From
	Student
	Left Join StudentSchoolYear 
		On Student.Id = StudentSchoolYear.StudentRef and StudentSchoolYear.SchoolYearRef = @schoolYearId
Where
	Student.id = @id

Select * From PersonEthnicity Where PersonEthnicity.PersonRef = @id

Select * From PersonLanguage Where PersonLanguage.PersonRef = @id

Select * From PersonNationality Where PersonNationality.PersonRef = @id

Select * From StudentSchool Where StudentRef = @id

GO