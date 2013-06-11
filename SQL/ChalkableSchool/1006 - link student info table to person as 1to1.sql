CREATE TABLE StudentInfo
(
	[Id] uniqueidentifier NOT NULL primary key ,
	[IEP] [bit] NOT NULL,
	[EnrollmentDate] [datetime2](7) NULL,
	[PreviousSchool] [nvarchar](1024) NULL,
	[PreviousSchoolPhone] [nvarchar](255) NULL,
	[PreviousSchoolNote] [nvarchar](max) NULL,
	[GradeLevelRef] uniqueidentifier NOT NULL Constraint FK_StudentInfo_GradeLevel Foreign Key References GradeLevel(Id)
)
GO

Alter Table StudentInfo
	Add Constraint FK_StudentInfo_Person Foreign Key (Id) References Person(Id)
GO


