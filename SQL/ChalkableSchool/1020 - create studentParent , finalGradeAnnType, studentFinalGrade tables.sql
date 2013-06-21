create table StudentParent
(
	Id uniqueidentifier primary key not null,
	StudentRef uniqueidentifier not null constraint FK_StudentParent_Student foreign key references Person(Id),
	ParentRef uniqueidentifier not null constraint FK_StudentParent_Parent foreign key references Person(Id)
)
go

alter table StudentParent
add constraint UQ_StudentParent_StudentRef_ParentRef unique (StudentRef, ParentRef)
go 


CREATE TABLE [dbo].[FinalGradeAnnouncementType](
	[Id] uniqueidentifier primary key not null,
	[FinalGradeRef] uniqueidentifier NOT NULL constraint FK_FinalGradeAnnouncementType_FinalGrade foreign key references FinalGrade(Id),
	[AnnouncementTypeRef] int NOT NULL constraint FK_FinalGradeAnnouncementType_AnnouncementType foreign key references AnnouncementType(Id),
	[PercentValue] [int] NOT NULL,
	[GradingStyle] [int] NOT NULL,
	[DropLowest] [bit] NOT NULL
)
alter table [FinalGradeAnnouncementType]
add constraint UQ_FinalGradeAnnouncementType_FinalGradeRef_AnnouncementTypeRef unique (FinalGradeRef, AnnouncementTypeRef)
go

CREATE TABLE [StudentFinalGrade](
	[Id] uniqueidentifier primary key NOT NULL,
	[FinalGradeRef] uniqueidentifier NOT NULL constraint FK_StudentFinalGrade_FinalGrade foreign key references FinalGrade(Id),
	[ClassPersonRef] uniqueidentifier NOT NULL constraint FK_StudentFinalGrade_ClassPerson foreign key references ClassPerson(Id),
	[TeacherGrade] [int] NULL,
	[AdminGrade] [int] NULL,
	[Comment] [nvarchar](1024) NULL,
	[GradeByAnnouncement] [int] NULL,
	[GradeByParticipation] [int] NULL,
	[GradeByAttendance] [int] NULL,
	[GradeByDiscipline] [int] NULL
)
go

alter table [StudentFinalGrade]
add constraint UQ_StudentFinalGrade_FinalGradeRef_ClassPersonRef unique (FinalGradeRef, ClassPersonRef)
go

