Alter view [dbo].[vwClass]
as
select 
	Class.Id as Class_Id,
	Class.Name as Class_Name,
	Class.Description as Class_Description,
	Class.SchoolYearRef as Class_SchoolYearRef,
	Class.CourseRef as Class_CourseRef,
	Class.TeacherRef as Class_TeacherRef,
	Class.GradeLevelRef as Class_GradeLevelRef,
	Class.SisId as Class_SisId,
	Course.Id as Course_Id,
	Course.Code as Course_Code,
	Course.Title as Course_Title,
	Course.ChalkableDepartmentRef as Course_ChalkableDepartmentId,
	GradeLevel.Id as GradeLevel_Id,
	GradeLevel.Name as GradeLevel_Name,
	Person.Id as Person_Id,
	Person.FirstName as Person_FirstName,
	Person.LastName as Person_LastName,
	Person.Gender as Person_Gender,
	Person.Salutation as Person_Salutation,
	Person.Email as Person_Email 
from 
	Class	
	join Course on Class.CourseRef = Course.Id
	join GradeLevel on GradeLevel.Id = Class.GradeLevelRef
	join Person on Person.Id = Class.TeacherRef
GO

ALTER view [dbo].[vwFinalGrade]
as
select FinalGrade.Id as FinalGrade_Id,
FinalGrade.[Status] as FinalGrade_Status,
FinalGrade.GradingStyle as FinalGrade_GradingStyle,
FinalGrade.ParticipationPercent as FinalGrade_ParticipationPercent,
FinalGrade.Attendance as FinalGrade_Attendance,
FinalGrade.Discipline as FinalGrade_Discipline,
FinalGrade.DropLowestAttendance as FinalGrade_DropLowestAttendance,
FinalGrade.DropLowestDiscipline as FinalGrade_DropLowestDiscipline,
MarkingPeriodClass.MarkingPeriodRef as MarkingPeriodClass_MarkingPeriodRef,
vwClass.*
from FinalGrade
join MarkingPeriodClass on MarkingPeriodClass.Id = FinalGrade.Id
join vwClass on vwClass.Class_Id = MarkingPeriodClass.ClassRef

GO

