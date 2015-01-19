ALTER VIEW [dbo].[vwPerson]
AS
SELECT
	Person.Id as Id,
	SchoolPerson.RoleRef as RoleRef,
	SchoolPerson.SchoolRef as SchoolRef,
	Person.FirstName as FirstName,
	Person.LastName as LastName,
	Person.BirthDate as BirthDate,
	Person.Gender as Gender,
	Person.Salutation as Salutation,
	Person.Active as Active,
	Person.FirstLoginDate as FirstLogInDate,
	Person.Email as Email,
	Person.AddressRef as AddressRef,
	Person.HasMedicalAlert as HasMedicalAlert,
	Person.IsAllowedInetAccess as IsAllowedInetAccess,
	Person.SpecialInstructions as SpecialInstructions,
	Person.SpEdStatus as SpEdStatus
	--,
	--GradeLevel.Id as GradeLevel_Id,
	--GradeLevel.Name as GradeLevel_Name,
	--StudentSchoolYear.SchoolYearRef as StudentSchoolYear_SchoolYearRef	
FROM 
	Person
	Join SchoolPerson on Person.Id = SchoolPerson.PersonRef
	--left join StudentSchoolYear on StudentSchoolYear.StudentRef = Person.Id
	--left join GradeLevel on StudentSchoolYear.GradeLevelRef = GradeLevel.Id	

GO


