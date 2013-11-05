create table Phone
(
	Id uniqueidentifier primary key not null,
	PersonRef uniqueidentifier not null constraint FK_Phone_Person foreign key references Person(Id),
	[Value] [nvarchar](256) NOT NULL,
	[Type] [int] NOT NULL,
	[IsPrimary] [bit] NOT NULL,
	[DigitOnlyValue] [nvarchar](256) NOT NULL,
	[SisId] [int] NULL,
)
go
CREATE TABLE [dbo].[PrivateMessage](
	[Id] uniqueidentifier primary key not null,
	[FromPersonRef] uniqueidentifier NOT NULL constraint FK_PrivateMessage_FromPerson foreign key references Person(Id),
	[ToPersonRef] uniqueidentifier NOT NULL constraint FK_PrivateMessage_ToPerson foreign key references Person(Id),
	[Sent] [datetime2](7) NULL,
	[Subject] [nvarchar](1024) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[Read] [bit] NOT NULL,
	[DeletedBySender] [bit] NOT NULL,
	[DeletedByRecipient] [bit] NOT NULL,
)
go
create table Room
(
	Id uniqueidentifier primary key not null,
	[RoomNumber] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[Size] [nvarchar](255) NULL,
	[Capacity] [int] NULL,
	[PhoneNumber] [nvarchar](255) NULL,
	[SisId] [int] NULL,
)
go
create table ScheduleSection
(
	Id uniqueidentifier primary key not null,
	Number int not null,
	Name nvarchar(1024) not null,
	MarkingPeriodRef uniqueidentifier not null constraint FK_ScheduleSection_MarkingPeriod foreign key references MarkingPeriod(Id),
	SisId int null 
)
go
create table Period
(
	Id uniqueidentifier primary key not null,
	[StartTime] [int] NOT NULL,
	[EndTime] [int] NOT NULL,
	[MarkingPeriodRef] uniqueidentifier NOT NULL constraint FK_Period_MarkingPeriod foreign key references MarkingPeriod(Id),
	[SectionRef] uniqueidentifier NOT NULL constraint FK_Period_ScheduleSection foreign key references ScheduleSection(Id),
	[SisId] [int] NULL,
	[Order] [int] NOT NULL,
	[SisId2] [int] NULL,
)
go
create table ClassPeriod
(
	Id uniqueidentifier primary key not null,
	PeriodRef uniqueidentifier not null constraint FK_ClassPeriod_Period foreign key references Period(Id),
	ClassRef uniqueidentifier not null constraint FK_ClassPeriod_Class foreign key references Class(Id),
	RoomRef uniqueidentifier not null constraint FK_ClassPeriod_Room foreign key references Room(Id),
	SisId int not null
)
go


create table [Date]
(
	Id uniqueidentifier primary key,
	[DateTime] datetime2 not null,
	ScheduleSectionRef uniqueidentifier not null constraint FK_Date_ScheduleSection foreign key references ScheduleSection(Id),
	MarkingPeriodRef uniqueidentifier not null constraint FK_Date_MarkingPeriod foreign key references MarkingPeriod(Id),
	IsSchoolDay bit not null,
	SisId int null 
)
go
