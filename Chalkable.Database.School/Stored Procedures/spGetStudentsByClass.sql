CREATE Procedure [dbo].[spGetStudentsByClass]
	@classId int,
	@markingPeriodId int,
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

Select Distinct
	Student.*,
	Cast(Case When ClassPerson.IsEnrolled = 1 and StudentSchoolYear.EnrollmentStatus = 0
			Then 0
			Else 1
			End As bit) As IsWithdrawn
From
	Student
	join ClassPerson       On Student.Id = ClassPerson.PersonRef
	join MarkingPeriod     On ClassPerson.MarkingPeriodRef = MarkingPeriod.Id
	join StudentSchoolYear On ClassPerson.PersonRef = StudentSchoolYear.StudentRef and StudentSchoolYear.SchoolYearRef = MarkingPeriod.SchoolYearRef
Where
	ClassPerson.ClassRef = @classId
	and (@markingPeriodId is null or MarkingPeriod.Id = @markingPeriodId)
	and (@isEnrolled is null or ClassPerson.IsEnrolled = @isEnrolled)
	and (@isEnrolled is null or StudentSchoolYear.EnrollmentStatus = @enrollmentStatus)

GO