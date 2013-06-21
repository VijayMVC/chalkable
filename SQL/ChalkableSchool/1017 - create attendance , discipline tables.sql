CREATE TABLE [dbo].[AttendanceReason](
	[Id] uniqueidentifier primary key NOT NULL,
	[AttendanceType] [int] NOT NULL,
	[Code] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
)


CREATE TABLE [dbo].[ClassAttendance](
	[Id] uniqueidentifier primary key NOT NULL,
	[ClassPersonRef] uniqueidentifier NOT NULL constraint FK_ClassAttendance_ClassPerson foreign key references ClassPerson(Id),
	[Type] [int] NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[AttendanceReasonRef] uniqueidentifier NULL constraint FK_ClassAttendance_AttendanceReason foreign key references AttendanceReason(Id),
	[SisId] [int] NULL,
	[ClassPeriodRef] uniqueidentifier NOT NULL constraint FK_ClassAttendance_ClassPeriod foreign key references ClassPeriod(Id),
	[Date] [datetime2](7) NOT NULL,
	[LastModified] [datetime2](7) NOT NULL
)

alter table ClassAttendance
add constraint UQ_ClassAttendance_ClassPeriodRef_ClassPersonRef_Date unique(ClassPersonRef, ClassPeriodRef, [Date])


CREATE TABLE [dbo].[ClassDiscipline](
	[Id] uniqueidentifier primary key NOT NULL,
	[ClassPersonRef] uniqueidentifier NOT NULL  constraint FK_ClassDiscipline_ClassPerson foreign key references ClassPerson(Id),
	[Description] [nvarchar](1024) NULL,
	[ClassPeriodRef] uniqueidentifier NOT NULL constraint FK_ClassDiscipline_ClassPeriod foreign key references ClassPeriod(Id),
	[Date] [datetime2](7) NOT NULL
)

alter table ClassDiscipline
add constraint UQ_ClassDiscipline_ClassPeriodRef_ClassPersonRef_Date unique(ClassPersonRef, ClassPeriodRef, [Date])


CREATE TABLE [dbo].[DisciplineType](
	[Id] uniqueidentifier primary key NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Score] [int] NOT NULL
)

CREATE TABLE [dbo].[ClassDisciplineType](
	[Id] uniqueidentifier primary key NOT NULL,
	[ClassDisciplineRef] uniqueidentifier NOT NULL constraint FK_ClassDisciplineType_ClassDiscipline foreign key references ClassDiscipline(Id),
	[DisciplineTypeRef] uniqueidentifier NOT NULL constraint FK_ClassDisciplineType_DisciplineType foreign key references DisciplineType(Id)
)

alter table [ClassDisciplineType]
add constraint UQ_ClassDisciplineType_ClassDisciplineRef_DisciplineTypeRef unique(ClassDisciplineRef, DisciplineTypeRef)


