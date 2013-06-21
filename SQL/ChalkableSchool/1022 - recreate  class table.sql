alter table ClassPeriod
drop constraint FK_ClassPeriod_Class
go
alter table ClassPerson
drop constraint FK_ClassPerson_Class
go
alter table MarkingPeriodClass
drop constraint FK_MarkingPeriodClass_Class
go
drop table Class 
go
CREATE TABLE [dbo].[Class](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[SchoolYearRef] [uniqueidentifier] NOT NULL constraint FK_Class_SchoolYearRef foreign key references SchoolYear(Id),
	[CourseRef] [uniqueidentifier] NOT NULL constraint FK_Class_CourseRef foreign key references Course(Id),
	[TeacherRef] [uniqueidentifier] NULL constraint FK_Class_Person foreign key references Person(Id),
	[GradeLevelRef] [uniqueidentifier] NOT NULL constraint FK_Class_GradeLevel foreign key references GradeLevel(Id),
	[SisId] [int] NULL,
)
GO
alter table ClassPeriod
add constraint FK_ClassPeriod_Class FOREIGN KEY([ClassRef]) references Class(Id)
go

alter table ClassPerson
add constraint FK_ClassPerson_Class FOREIGN KEY([ClassRef]) references Class(Id)
go
alter table MarkingPeriodClass
add constraint FK_MarkingPeriodClass_Class FOREIGN KEY([ClassRef]) references Class(Id)
go
