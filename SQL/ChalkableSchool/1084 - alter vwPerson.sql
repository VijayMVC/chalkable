alter view [dbo].[vwPerson]
as
SELECT
	Student.Id as Id,
	cast(3  as int) as RoleRef,
	StudentSchool.SchoolRef as SchoolRef,
	Student.FirstName as FirstName,
	Student.LastName as LastName,
	Student.BirthDate as BirthDate,
	Student.Gender as Gender,
	Person.Salutation as Salutation,
	Person.Active as Active,
	Person.FirstLoginDate as FirstLoginDate,
	Person.AddressRef as AddressRef,
	Student.HasMedicalAlert as HasMedicalAlert,
	Student.IsAllowedInetAccess as IsAllowedInetAccess,
	Student.SpecialInstructions as SpecialInstructions,
	Student.SpEdStatus as SpEdStatus,
	Student.UserId as UserId
FROM Student
JOIN Person on Person.Id = Student.Id
JOIN StudentSchool on StudentSchool.StudentRef = Student.Id
UNION
SELECT 
	Staff.Id as Id,
	cast(2 as int) as RoleRef, 
	StaffSchool.SchoolRef as SchoolRef,
	Staff.FirstName as FirstName,
	Staff.LastName as LastName,
	Staff.BirthDate as BirthDate,
	Staff.Gender as Gender,
	Person.Salutation as Salutation,
	Person.Active as Active,
	Person.FirstLoginDate as FirstLoginDate,
	Person.AddressRef as AddressRef,
	cast(0 as bit) as HasMedicalAlert,
	cast(0 as bit) as IsAllowedInetAccess,
	cast(null as nvarchar(max)) as SpecialInstructions,
	cast(null as nvarchar(max)) as SpEdStatus,
	Staff.UserId as UserId
FROM Staff
JOIN Person on Person.Id = Staff.Id
JOIN StaffSchool on StaffSchool.StaffRef = Staff.UserId


GO
