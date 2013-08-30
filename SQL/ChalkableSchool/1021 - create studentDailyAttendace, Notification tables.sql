alter table [Date]
alter column ScheduleSectionRef uniqueidentifier null

alter table [Date]
alter column MarkingPeriodRef uniqueidentifier null


CREATE TABLE [dbo].[StudentDailyAttendance](
	[Id] uniqueidentifier primary key NOT NULL,
	[PersonRef] uniqueidentifier NOT NULL constraint FK_StudentDailyAttendance_Person foreign key references Person(Id),
	[Date] [datetime2](7) NOT NULL,
	[Arrival] [int] NULL,
	[TimeIn] [int] NULL,
	[TimeOut] [int] NULL
)
go

CREATE TABLE [dbo].[Notification](
	[Id] uniqueidentifier primary key NOT NULL,
	[Type] [int] NOT NULL,
	[Message] [nvarchar](1024) NULL,
	[Shown] [bit] NOT NULL,
	[PersonRef] uniqueidentifier NOT NULL constraint  FK_Notification_Person foreign key references Person(Id),
	[AnnouncementRef] uniqueidentifier NULL  constraint  FK_Notification_Announcement foreign key references Announcement(Id),
	[PrivateMessageRef] uniqueidentifier NULL  constraint  FK_Notification_PrivateMessage foreign key references PrivateMessage(Id),
	[ApplicationRef] uniqueidentifier NULL,
	[QuestionPersonRef] uniqueidentifier NULL constraint  FK_Notification_QuestionPerson foreign key references Person(Id),
	[Created] [datetime2](7) NOT NULL,
	[MarkingPeriodRef] uniqueidentifier NULL constraint  FK_Notification_MarkingPeriod foreign key references MarkingPeriod(Id),
	[WasSend] [bit] NOT NULL,
	[ClassPeriodRef] uniqueidentifier NULL constraint  FK_Notification_ClassPeriod foreign key references ClassPeriod(Id),
)
GO