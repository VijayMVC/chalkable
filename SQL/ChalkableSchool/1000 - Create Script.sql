Create Database ChalkableSchoolTemplate
GO

Use ChalkableSchoolTemplate
GO

CREATE TABLE Person
(
	[Id] uniqueidentifier NOT NULL primary key,	
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[BirthDate] [datetime2](7) NULL,
	[Gender] [nvarchar](255) NULL,
	[Salutation] [nvarchar](255) NULL,
	[Active] [bit] NOT NULL,
	[LastPasswordReset] [datetime2](7) NULL,
	[FirstLoginDate] [datetime2](7) NULL,
	[SisId] [int] NULL,
	[RoleRef] [int] NOT NULL,
	[LastMailNotification] [datetime] NULL,
	[Email] nvarchar(256) not null
)
GO

CREATE TABLE StudentInfo
(
	[Id] uniqueidentifier NOT NULL primary key ,
	[PersonRef] uniqueidentifier NOT NULL Constraint FK_StudentInfo_Person Foreign Key References Person(Id),
	[IEP] [bit] NOT NULL,
	[EnrollmentDate] [datetime2](7) NULL,
	[PreviousSchool] [nvarchar](1024) NULL,
	[PreviousSchoolPhone] [nvarchar](255) NULL,
	[PreviousSchoolNote] [nvarchar](max) NULL,
	[GradeLevelRef] uniqueidentifier NOT NULL Constraint FK_StudentInfo_GradeLevel Foreign Key References GradeLevel(Id)
)
GO