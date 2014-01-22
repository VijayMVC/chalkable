ALTER view [dbo].[vwPerson]
as
select 	
	Person.Id as Id,
	Person.RoleRef as RoleRef,
	Person.FirstName as FirstName,
	Person.LastName as LastName,
	Person.BirthDate as BirthDate,
	Person.Gender as Gender,
	Person.Salutation as Salutation,
	Person.Active as Active,
	Person.FirstLoginDate as FirstLogInDate,
	Person.Email as Email,
	Person.SisId as SisId,
	GradeLevel.Id as GradeLevel_Id,
	GradeLevel.Name as GradeLevel_Name,
	StudentInfo.IEP as IEP,
	StudentInfo.EnrollmentDate as EnrollmentDate,
	StudentInfo.PreviousSchool as PreviousSchool,
	StudentInfo.PreviousSchoolNote as PreviousSchoolNote,
	StudentInfo.PreviousSchoolPhone as PreviousSchoolPhone
from 
	Person
	left join StudentInfo on StudentInfo.Id = Person.Id
	left join GradeLevel on StudentInfo.GradeLevelRef = GradeLevel.Id	

GO

ALTER view [dbo].[vwStudentFinalGrade]
as
select
StudentFinalGrade.Id as StudentFinalGrade_Id,
StudentFinalGrade.ClassPersonRef as StudentFinalGrade_ClassPersonRef,
StudentFinalGrade.FinalGradeRef as StudentFinalGrade_FinalGradeRef,
StudentFinalGrade.AdminGrade as StudentFinalGrade_AdminGrade,
StudentFinalGrade.TeacherGrade as StudentFinalGrade_TeacherGrade,
StudentFinalGrade.GradeByAnnouncement as StudentFinalGrade_GradeByAnnouncement,
StudentFinalGrade.GradeByAttendance as StudentFinalGrade_GradeByAttendance,
StudentFinalGrade.GradeByDiscipline as StudentFinalGrade_GradeByDiscipline,
StudentFinalGrade.GradeByParticipation as StudentFinalGrade_GradeByParticipation,
vwPerson.*
from StudentFinalGrade
join ClassPerson on ClassPerson.Id = StudentFinalGrade.ClassPersonRef
join vwPerson on vwPerson.Id = ClassPerson.PersonRef
GO




