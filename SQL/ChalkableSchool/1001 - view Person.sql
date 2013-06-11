CREATE view [dbo].[vwPerson]
as
select 	
	Person.Id as PersonId,
	Person.RoleRef as RoleId,
	Person.FirstName as FirstName,
	Person.LastName as LastName,
	Person.BirthDate as BirthDate,
	Person.Gender as Gender,
	Person.Salutation as Salutation,
	Person.Active as Active,
	Person.FirstLoginDate as FirstLogInDate,
	Person.Email as Email,
	GradeLevel.Id as GradeLevelId,
	GradeLevel.Name as GradeLevelName,
	StudentInfo.IEP as IEP,
	StudentInfo.EnrollmentDate as EnrollmentDate,
	StudentInfo.PreviousSchool as PreviousSchool,
	StudentInfo.PreviousSchoolNote as PreviousSchoolNote,
	StudentInfo.PreviousSchoolPhone as PreviousSchoolPhone
from 
	Person
	left join StudentInfo on StudentInfo.PersonRef = Person.Id
	left join GradeLevel on StudentInfo.GradeLevelRef = GradeLevel.Id	
GO


