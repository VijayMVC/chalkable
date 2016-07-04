CREATE PROCEDURE [dbo].[spGetStudentsDetailsByClass]
	@classId int,
	@isEnrolled bit
As

Declare @enrollmentStatus int

If @isEnrolled is not null
Begin
	If @isEnrolled = 1
		Set @enrollmentStatus = 0
	Else
		Set @enrollmentStatus = 1
End

Declare @students table
(
	[Id] int,
	[IsWithdrawn] bit
);

Insert Into @students
	Select Distinct
		Student.Id,
		Cast(Case When ClassPerson.IsEnrolled = 1 and StudentSchoolYear.EnrollmentStatus = 0 
				Then 0
				Else 1
				End As bit) As IsWithdrawn
	From
		Student
		join ClassPerson       On Student.Id = ClassPerson.PersonRef
		join StudentSchoolYear On ClassPerson.PersonRef = StudentSchoolYear.StudentRef
	Where
		ClassPerson.ClassRef = @classId
		and (@isEnrolled is null or ClassPerson.IsEnrolled = @isEnrolled)
		and (@isEnrolled is null or StudentSchoolYear.EnrollmentStatus = @enrollmentStatus)

Select Student.*, IsWithdrawn
From Student join @students as stud
	on Student.Id = stud.Id

Select PersonEthnicity.* From PersonEthnicity join @students as stud
	on stud.Id = PersonEthnicity.PersonRef

Select PersonLanguage.* From PersonLanguage join @students as stud
	on stud.Id = PersonLanguage.PersonRef

Select PersonNationality.* From PersonNationality join @students as stud
	on stud.Id = PersonNationality.PersonRef

Select StudentSchool.* From StudentSchool join @students as stud
	on stud.Id = StudentSchool.StudentRef

GO