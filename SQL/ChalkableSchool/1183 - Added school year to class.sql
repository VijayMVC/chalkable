Drop Procedure spSelectClassDetails
GO

Drop Type TClassDetails
GO

Create Type TClassDetails As Table(
	[Class_Id] [int] NULL,
	[Class_ClassNumber] [nvarchar](max) NULL,
	[Class_Name] [nvarchar](max) NULL,
	[Class_Description] [nvarchar](max) NULL,
	[Class_SchoolYearRef] [int] NULL,
	[Class_PrimaryTeacherRef] [int] NULL,
	[Class_MinGradeLevelRef] [int] NULL,
	[Class_MaxGradeLevelRef] [int] NULL,
	[Class_ChalkableDepartmentRef] [uniqueidentifier] NULL,
	[Class_CourseRef] [int] NULL,
	[Class_CourseTypeRef] [int] NOT NULL,
	[Class_RoomRef] [int] NULL,
	[Class_GradingScaleRef] [int] NULL,
	[Person_Id] [int] NULL,
	[Person_FirstName] [nvarchar](max) NULL,
	[Person_LastName] [nvarchar](max) NULL,
	[Person_Gender] [nvarchar](max) NULL,
	[Person_RoleRef] [int] NULL,
	[Class_StudentsCount] [int] NULL
)
GO


Create Procedure spSelectClassDetails
	@classes TClassDetails readonly
as
	Select 
		* 
	From
		@classes
	Order By 
		Class_Id

	Select 
		mpc.*
	From 
		MarkingPeriodClass mpc
		join @classes c on c.Class_Id = mpc.ClassRef

	Select 
		ct.*
	From 
		ClassTeacher ct
		join @classes c on c.Class_Id = ct.ClassRef

GO

