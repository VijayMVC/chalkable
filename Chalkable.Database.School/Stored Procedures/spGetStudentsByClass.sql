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
	Cast(Case When ClassPerson.IsEnrolled = 1 and StudentSchoolYear.EnrollmentStatus = 0 
		 Then 0
		 Else 1
		 End As bit) As IsWithdrawn,
	Ethnicity.Id As Ethnicity_Id,
	Ethnicity.Code As Ethnicity_Code,
	Ethnicity.Name As Ethnicity_Name,
	Ethnicity.[Description] As Ethnicity_Description,
	Ethnicity.StateCode As Ethnicity_StateCode,
	Ethnicity.SIFCode As Ethnicity_SIFCode,
	Ethnicity.NCESCode As Ethnicity_NCESCode,
	Ethnicity.IsActive As Ethnicity_IsActive,
	Ethnicity.IsSystem As Ethnicity_IsSystem
From
	Student
	left join PersonEthnicity   On Student.Id = PersonEthnicity.PersonRef and PersonEthnicity.IsPrimary = 1
	left join Ethnicity		    On Ethnicity.Id = PersonEthnicity.EthnicityRef
	     join ClassPerson       On Student.Id = ClassPerson.PersonRef
	     join MarkingPeriod     On ClassPerson.MarkingPeriodRef = MarkingPeriod.Id
	     join StudentSchoolYear On ClassPerson.PersonRef = StudentSchoolYear.StudentRef and StudentSchoolYear.SchoolYearRef = MarkingPeriod.SchoolYearRef
Where
	ClassPerson.ClassRef = @classId
	and (@markingPeriodId is null or MarkingPeriod.Id = @markingPeriodId)
	and (@isEnrolled is null or ClassPerson.IsEnrolled = @isEnrolled)
	and (@isEnrolled is null or StudentSchoolYear.EnrollmentStatus = @enrollmentStatus)
GO