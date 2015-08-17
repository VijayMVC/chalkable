Create Type TClassDetails as Table
(
	Class_Id int,
	Class_ClassNumber nvarchar(max),
	Class_Name nvarchar(max),
	Class_Description  nvarchar(max),
	Class_SchoolYearRef int,
	Class_PrimaryTeacherRef int,
	Class_MinGradeLevelRef int,
	Class_MaxGradeLevelRef int,
	Class_ChalkableDepartmentRef uniqueidentifier,	
	Class_CourseRef int,
	Class_RoomRef int,
	Class_GradingScaleRef int,
	Person_Id int,
	Person_FirstName nvarchar(max),
	Person_LastName nvarchar(max),
	Person_Gender nvarchar(max),
	Person_RoleRef int,
	Class_StudentsCount int
)