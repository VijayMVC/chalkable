
CREATE View [dbo].[vwClass]
As
Select
Class.Id as Class_Id,
Class.ClassNumber as Class_ClassNumber,
Class.Name as Class_Name,
Class.Description as Class_Description,
Class.SchoolYearRef as Class_SchoolYearRef,
Class.PrimaryTeacherRef as Class_PrimaryTeacherRef,
Class.MinGradeLevelRef as Class_MinGradeLevelRef,
Class.MaxGradeLevelRef as Class_MaxGradeLevelRef,
Class.ChalkableDepartmentRef as Class_ChalkableDepartmentRef,
Class.CourseRef as Class_CourseRef,
Class.CourseTypeRef as Class_CourseTypeRef,
Class.RoomRef as Class_RoomRef,
Class.GradingScaleRef as Class_GradingScaleRef,
Staff.Id as Person_Id,
Staff.FirstName as Person_FirstName,
Staff.LastName as Person_LastName,
Staff.Gender as Person_Gender,
2 as Person_RoleRef,
SchoolYear.Id SchoolYear_Id,
SchoolYear.SchoolRef as SchoolYear_SchoolRef,
SchoolYear.Name as SchoolYear_Name,
SchoolYear.Description as SchoolYear_Description,
SchoolYear.StartDate as SchoolYear_StartDate,
SchoolYear.EndDate as SchoolYear_EndDate,
SchoolYear.ArchiveDate as SchoolYear_ArchiveDate
From
Class
left join Staff on Staff.Id = Class.PrimaryTeacherRef
left join SchoolYear on SchoolYear.Id = Class.SchoolYearRef
Where
Class.PrimaryTeacherRef is null or Staff.Id is not null