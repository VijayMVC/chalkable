ALTER VIEW [dbo].[vwClass]
AS
SELECT
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
	2 as Person_RoleRef
FROM
	Class
	left join Staff on Staff.Id = Class.PrimaryTeacherRef
where 
	Class.PrimaryTeacherRef is null or Staff.Id is not null
GO

drop type [TClass]

CREATE TYPE [dbo].[TClass] AS TABLE(
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[ChalkableDepartmentRef] [uniqueidentifier] NULL,
	[SchoolYearRef] [int] NULL,
	[PrimaryTeacherRef] [int] NULL,
	[MinGradeLevelRef] [int] NULL,
	[MaxGradeLevelRef] [int] NULL,
	[RoomRef] [int] NULL,
	[CourseRef] [int] NULL,
	[CourseTypeRef] int not null,
	[GradingScaleRef] [int] NULL,
	[ClassNumber] [nvarchar](41) NULL
)
GO

ALTER Procedure [dbo].[spSelectClassDetails] 
@classes nvarchar(max)
as
--select * from
--@classes
--order by Class_Id
--select mpc.*
--from MarkingPeriodClass mpc
--join @classes c on c.Class_Id = mpc.ClassRef

--select ct.*
--from ClassTeacher ct
--join @classes c on c.Class_Id = ct.ClassRef
GO

drop type [TClassDetails] 
go

CREATE TYPE [dbo].[TClassDetails] AS TABLE(
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
	[Class_CourseTypeRef] [int] not null,
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




ALTER Procedure [dbo].[spSelectClassDetails]
@classes TClassDetails readonly
as
select * from
@classes
order by Class_Id
select mpc.*
from MarkingPeriodClass mpc
join @classes c on c.Class_Id = mpc.ClassRef

select ct.*
from ClassTeacher ct
join @classes c on c.Class_Id = ct.ClassRef
GO


