CREATE TABLE School
(
	Id INT NOT NULL PRIMARY KEY,
	Name NVARCHAR(1024),
	IsActive BIT NOT NULL,
	IsPrivate BIT NOT NULL
)

GO

CREATE TABLE [dbo].[Address]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[AddressNumber] CHAR(15) NOT NULL,
	[StreetNumber] NVARCHAR(10) NOT NULL,
	[AddressLine1] NVARCHAR(75) NOT NULL,
	[AddressLine2] NVARCHAR(75) NOT NULL,
	[City] NVARCHAR(50) NOT NULL,
	[State] NVARCHAR(5) NOT NULL,
	[PostalCode] CHAR(10) NOT NULL,
	[Country] NVARCHAR(60) NOT NULL,
	[CountyId] INT NULL,
	[Latitude] DECIMAL(10, 7) NULL,
	[Longitude] DECIMAL(10, 7) NULL,
)
GO

CREATE TABLE Person
(
	[Id] INT NOT NULL PRIMARY KEY,	
	[FirstName] NVARCHAR(255) NOT NULL,
	[LastName] NVARCHAR(255) NOT NULL,
	[BirthDate] DATETIME2(7) NULL,
	[Gender] NVARCHAR(255) NULL,
	[Salutation] NVARCHAR(255) NULL,
	[Active] BIT NOT NULL,
	[LastPasswordReset] DATETIME2(7) NULL,
	[FirstLoginDate] DATETIME2(7) NULL,
	[LastMailNotification] DATETIME2 NULL,
	[Email] NVARCHAR(256) NOT NULL,
	AddressRef INT CONSTRAINT FK_Person_Address FOREIGN KEY REFERENCES [Address](Id)
)
GO

CREATE TABLE SchoolPerson
(	
	SchoolRef INT NOT NULL CONSTRAINT FK_SchoolStaff_School FOREIGN KEY REFERENCES School(Id),
	PersonRef INT NOT NULL CONSTRAINT FK_SchoolStaff_PERSON FOREIGN KEY REFERENCES Person(Id),
	RoleRef INT NOT NULL
)

GO

ALTER TABLE SchoolPerson
	ADD CONSTRAINT PK_SchoolPerson PRIMARY KEY (SchoolRef, PersonRef)
GO

CREATE TABLE [dbo].[AnnouncementType](
	[Id] INT PRIMARY KEY NOT NULL,
	[IsSystem] BIT NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(1024) NULL,
	Gradable BIT NULL,
)
GO

CREATE TABLE [dbo].[SchoolYear](
	[Id] INT NOT NULL PRIMARY KEY,
	SchoolRef INT NOT NULL CONSTRAINT FK_SchoolYear_School FOREIGN KEY REFERENCES School(Id),
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(1024) NULL,
	[StartDate] DATETIME2(7) NOT NULL,
	[EndDate] DATETIME2(7) NOT NULL,
)

GO

Alter TABLE SchoolYear
	Add CONSTRAINT QU_SchoolYear_Name UNIQUE(Name, SchoolRef)
GO

CREATE TABLE [dbo].[MarkingPeriod](
	[Id] INT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(255) NOT NULL,
	[StartDate] DATETIME2(7) NOT NULL,
	[EndDate] DATETIME2(7) NOT NULL,
	[Description] NVARCHAR(1024) NULL,
	[SchoolYearRef] INT NOT NULL CONSTRAINT FK_MarkingPeriod_SchoolYear FOREIGN KEY REFERENCES SchoolYear(Id),
	[WeekDays] INT NOT NULL DEFAULT (126)
)
GO

CREATE TABLE [dbo].[GradeLevel](
	[Id] INT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(1024),
	[Number] INT NOT NULL
)
GO

Alter TABLE [GradeLevel]
	Add CONSTRAINT QU_GradeLevel_Name UNIQUE(Name)
GO

Alter TABLE [GradeLevel]
	Add CONSTRAINT QU_GradeLevel_Number UNIQUE(Number)
GO

CREATE TABLE [dbo].[Class](
	[Id] INT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(1024) NULL,
	[ChalkableDepartmentRef] INT NULL,
	[SchoolYearRef] INT NULL CONSTRAINT FK_Class_SchoolYearRef FOREIGN KEY REFERENCES SchoolYear(Id),
	[TeacherRef] INT NULL CONSTRAINT FK_Class_Person FOREIGN KEY REFERENCES Person(Id),
	[GradeLevelRef] INT NOT NULL CONSTRAINT FK_Class_GradeLevel FOREIGN KEY REFERENCES GradeLevel(Id)
)
GO

CREATE TABLE [dbo].[MarkingPeriodClass](
	[ClassRef] INT NOT NULL CONSTRAINT [FK_MarkingPeriodClass_Class] FOREIGN KEY REFERENCES Class(Id),
	[MarkingPeriodRef] INT NOT NULL CONSTRAINT [FK_MarkingPeriodClass_MarkingPeriod] FOREIGN KEY REFERENCES MarkingPeriod(Id),
)
GO


Alter Table MarkingPeriodClass
	Add Constraint PK_MarkingPeriodClass Primary Key (ClassRef, MarkingPeriodRef)

GO

CREATE TABLE [dbo].[Announcement](
	[Id] INT PRIMARY KEY IDENTITY(1000000000, 1) NOT NULL,
	[PersonRef] INT NOT NULL CONSTRAINT [FK_Announcement_Person] FOREIGN KEY REFERENCES Person(Id),
	[Content] NVARCHAR(max) NULL,
	[CREATEd] DATETIME2(7) NOT NULL,
	[Expires] DATETIME2(7) NOT NULL,
	[AnnouncementTypeRef] INT NOT NULL CONSTRAINT [FK_Announcement_AnnouncementType] FOREIGN KEY REFERENCES AnnouncementType(Id),
	[State] INT NOT NULL,
	[GradingStyle] INT NOT NULL,
	[Subject] NVARCHAR(255) NULL,
	[ClassRef] int null constraint FK_Announcement_Class foreign key references Class(Id),
	[Order] INT NOT NULL,
	[Dropped] BIT NOT NULL,
)
GO

CREATE TABLE [dbo].[AnnouncementApplication](
	[Id] INT PRIMARY KEY IDENTITY NOT NULL,
	[AnnouncementRef] INT NOT NULL CONSTRAINT [FK_AnnouncementApplication_Announcement] FOREIGN KEY REFERENCES [Announcement](Id),
	[ApplicationRef] [UNIQUEidentifier] NOT NULL,
	[Active] BIT NOT NULL,
	[Order] INT NOT NULL,
)
GO

CREATE TABLE [dbo].[AnnouncementAttachment](
	[Id] UNIQUEidentifier PRIMARY KEY NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[PersonRef] INT NOT NULL CONSTRAINT [FK_AnnouncementAttachment_Person] FOREIGN KEY REFERENCES Person(Id),
	[AnnouncementRef] INT NOT NULL CONSTRAINT [FK_AnnouncementAttachment_Announcement] FOREIGN KEY REFERENCES [Announcement](Id),
	[AttachedDate] DATETIME2(7) NOT NULL,
	[Uuid] NVARCHAR(255) NULL,
	[Order] INT NOT NULL,
)
GO


CREATE TABLE [dbo].[AnnouncementQnA](
	[id] INT PRIMARY KEY IDENTITY NOT NULL,
	[AnnouncementRef] INT NOT NULL CONSTRAINT [FK_AnnouncementQnA_Announcement] FOREIGN KEY REFERENCES [dbo].[Announcement] ([Id]),
	[PersonRef] INT NOT NULL CONSTRAINT [FK_AnnouncementQnA_Person] FOREIGN KEY REFERENCES [dbo].[Person] ([Id]),
	[Question] NVARCHAR(max) NOT NULL,
	[Answer] NVARCHAR(max) NULL,
	[State] INT NOT NULL,
	[AnsweredTime] DATETIME2(7) NULL,
	[QuestionTime] DATETIME2(7) NULL,
)
GO

CREATE TABLE [dbo].[AnnouncementRecipient](
	[Id] INT PRIMARY KEY IDENTITY NOT NULL,
	[AnnouncementRef] INT NOT NULL  CONSTRAINT [FK_AnnouncementRecipient_Announcement] FOREIGN KEY REFERENCES [dbo].[Announcement] ([Id]),
	[ToAll] BIT NOT NULL,
	[RoleRef] INT NULL,
	[GradeLevelRef] INT NULL CONSTRAINT [FK_AnnouncementRecipient_GradeLevel] FOREIGN KEY REFERENCES [dbo].[GradeLevel] ([Id]),
	[PersonRef] INT NULL CONSTRAINT [FK_AnnouncementRecipient_Person] FOREIGN KEY REFERENCES [dbo].[Person] ([Id]),
)
GO

CREATE TABLE [dbo].[AnnouncementRecipientData](
	[Id] INT PRIMARY KEY IDENTITY NOT NULL,
	[AnnouncementRef] INT NOT NULL CONSTRAINT [FK_AnnouncementRecipientData_Announcement] FOREIGN KEY REFERENCES [dbo].[Announcement] ([Id]),
	[PersonRef] INT NOT NULL CONSTRAINT [FK_AnnouncementRecipientData_Person] FOREIGN KEY REFERENCES [dbo].[Person] ([Id]),
	[Starred] BIT NOT NULL,
	[StarredAutomatically] INT NOT NULL,
	LastModifiedDate DATETIME2 null
)
GO

CREATE TABLE [dbo].[AnnouncementReminder](
	[Id] INT PRIMARY KEY IDENTITY NOT NULL,
	[RemindDate] DATETIME2(7) NULL,
	[Processed] BIT NOT NULL,
	[AnnouncementRef] INT NOT NULL CONSTRAINT [FK_AnnouncementReminder_Announcement] FOREIGN KEY REFERENCES [dbo].[Announcement] ([Id]),
	[Before] INT NULL,
	[PersonRef] INT NULL CONSTRAINT [FK_AnnouncementReminder_Person] FOREIGN KEY REFERENCES [dbo].[Person] ([Id]),
)
GO

CREATE TABLE ClassPerson
(	
	ClassRef INT NOT NULL CONSTRAINT FK_ClassPerson_Class FOREIGN KEY REFERENCES Class(Id),
	PersonRef INT NOT NULL CONSTRAINT FK_ClassPerson_Person FOREIGN KEY REFERENCES Person(Id)
)
GO

Alter TABLE ClassPerson
	ADD CONSTRAINT PK_ClassPerson PRIMARY KEY(PersonRef, ClassRef)
GO

CREATE TABLE StudentSchoolYear
(
	SchoolYearRef INT NOT NULL CONSTRAINT FK_StudentSchoolYear_SchoolYear FOREIGN KEY REFERENCES SchoolYear(Id),
	GradeLevelRef INT NOT NULL CONSTRAINT FK_StudentSchoolYear_GradeLevel FOREIGN KEY REFERENCES GradeLevel(Id),
	StudentRef INT NOT NULL CONSTRAINT FK_StudentSchoolYear_Person FOREIGN KEY REFERENCES Person(Id)
)
GO

Alter Table StudentSchoolYear
	Add Constraint PK_StudentSchoolYear Primary Key (SchoolYearRef, StudentRef)
GO

CREATE TABLE StudentAnnouncement
(
	Id INT NOT NULL PRIMARY KEY IDENTITY(1000000000, 1),
	PersonRef INT NOT NULL CONSTRAINT FK_StudentAnnouncement_Person FOREIGN KEY REFERENCES Person(Id),
	AnnouncementRef INT NOT NULL CONSTRAINT FK_StudentAnnouncement_Announcement FOREIGN KEY REFERENCES Announcement(Id),
	Comment NVARCHAR(1024) NULL,
	GradeValue INT NULL,
	ExtraCredit NVARCHAR(255) null,
	Dropped bit NOT NULL,
	State INT NOT NULL,
	ApplicationRef UNIQUEidentifier null
)
GO

CREATE TABLE Phone
(
	PersonRef INT NOT NULL CONSTRAINT FK_Phone_Person FOREIGN KEY REFERENCES Person(Id),
	[Value] NVARCHAR(256) NOT NULL,
	[Type] INT NOT NULL,
	[IsPRIMARY] BIT NOT NULL,
	[DigitOnlyValue] NVARCHAR(256) NOT NULL
)
GO
Alter Table Phone
	Add Constraint PK_Phone Primary Key (DigitOnlyValue, PersonRef)
GO

CREATE TABLE [dbo].[PrivateMessage](
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[FromPersonRef] INT NOT NULL CONSTRAINT FK_PrivateMessage_FromPerson FOREIGN KEY REFERENCES Person(Id),
	[ToPersonRef] INT NOT NULL CONSTRAINT FK_PrivateMessage_ToPerson FOREIGN KEY REFERENCES Person(Id),
	[Sent] DATETIME2(7) NULL,
	[Subject] NVARCHAR(1024) NOT NULL,
	[Body] NVARCHAR(max) NOT NULL,
	[Read] BIT NOT NULL,
	[DeletedBySender] BIT NOT NULL,
	[DeletedByRecipient] BIT NOT NULL,
)
GO

CREATE TABLE Room
(
	Id INT PRIMARY KEY NOT NULL,
	[RoomNumber] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(1024) NULL,
	[Size] NVARCHAR(255) NULL,
	[Capacity] INT NULL,
	[PhoneNumber] NVARCHAR(255) NULL
)
GO

CREATE TABLE DayType
(
	Id INT NOT NULL PRIMARY KEY,
	Number INT NOT NULL,
	Name NVARCHAR(1024) NOT NULL,
	SchoolYearRef INT NOT NULL CONSTRAINT FK_DayType_SchoolYear FOREIGN KEY REFERENCES SchoolYear(Id)
)
GO

CREATE TABLE Period
(
	Id INT NOT NULL PRIMARY KEY,
	[StartTime] INT NOT NULL,
	[EndTime] INT NOT NULL,
	SchoolYearRef INT NOT NULL CONSTRAINT FK_Period_SchoolYear FOREIGN KEY REFERENCES SchoolYear(Id),
	[Order] INT NOT NULL
)
GO
CREATE TABLE ClassPeriod
(
	Id INT NOT NULL PRIMARY KEY,
	PeriodRef INT NOT NULL CONSTRAINT FK_ClassPeriod_Period FOREIGN KEY REFERENCES Period(Id),
	DayTypeRef INT NOT NULL CONSTRAINT FK_ClassPeriod_DayType FOREIGN KEY REFERENCES DayType(Id),
	ClassRef INT NOT NULL CONSTRAINT FK_ClassPeriod_Class FOREIGN KEY REFERENCES Class(Id),
	RoomRef INT NOT NULL CONSTRAINT FK_ClassPeriod_Room FOREIGN KEY REFERENCES Room(Id)
)
GO

Alter TABLE ClassPeriod
	ADD CONSTRAINT UQ_Class_Period_DayType UNIQUE(ClassRef, PeriodRef, DayTypeRef)
GO

CREATE TABLE [Date]
(
	[Date] DATETIME2 NOT NULL,
	DayTypeRef INT CONSTRAINT FK_Date_DayType FOREIGN KEY REFERENCES DayType(Id),
	SchoolYearRef INT NOT NULL CONSTRAINT FK_Date_SchoolYear FOREIGN KEY REFERENCES SchoolYear(Id),
	IsSchoolDay BIT NOT NULL
)
GO

Alter TABLE [Date]
	Add CONSTRAINT PK_Date PRIMARY KEY([Date], SchoolYearRef)
GO


CREATE TABLE [dbo].[Notification](
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[Type] INT NOT NULL,
	[Message] NVARCHAR(1024) NULL,
	[Shown] BIT NOT NULL,
	[PersonRef] INT NOT NULL CONSTRAINT FK_Notification_Person FOREIGN KEY REFERENCES Person(Id),
	[AnnouncementRef] INT NULL CONSTRAINT FK_Notification_Announcement FOREIGN KEY REFERENCES Announcement(Id),
	[PrivateMessageRef] INT NULL CONSTRAINT FK_Notification_PrivateMessage FOREIGN KEY REFERENCES PrivateMessage(Id),
	[ApplicationRef] UNIQUEidentifier NULL,
	[QuestionPersonRef] INT NULL CONSTRAINT FK_Notification_QuestionPerson FOREIGN KEY REFERENCES Person(Id),
	[CREATEd] DATETIME2(7) NOT NULL,
	[MarkingPeriodRef] INT NULL CONSTRAINT FK_Notification_MarkingPeriod FOREIGN KEY REFERENCES MarkingPeriod(Id),
	[WasSend] BIT NOT NULL,
	[ClassPeriodRef] INT NULL CONSTRAINT FK_Notification_ClassPeriod FOREIGN KEY REFERENCES ClassPeriod(Id)
)
GO

CREATE TABLE [dbo].[ReportDownload]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Format] INT NOT NULL,
	[PersonRef] INT NOT NULL CONSTRAINT FK_ReportDownload_Person FOREIGN KEY REFERENCES Person(Id),
	[ReportType] INT NOT NULL,
	[DownloadDate] DATETIME2(7) NOT NULL,
	[FriendlyName] NVARCHAR(1024) NOT NULL,
)
GO

CREATE TABLE [dbo].[ReportMailDelivery]
(
	[Id] INT PRIMARY KEY NOT NULL,
	[ReportType] INT NOT NULL ,
	[Format] INT NOT NULL,
	[Frequency] INT NOT NULL,
	[PersonRef] INT NOT NULL CONSTRAINT FK_ReportMailDelivery_Person FOREIGN KEY REFERENCES Person(Id),
	[SendHour] INT NULL,
	[SendDay] INT NULL,
	[LastSentMarkingPeriodRef] INT NULL CONSTRAINT FK_ReportMailDelivery_MarkingPeriod FOREIGN KEY REFERENCES MarkingPeriod(Id),
	[LastSentTime] DATETIME2(7) NULL
)
GO

CREATE TABLE [dbo].[ApplicationInstallAction](
	[Id] INT PRIMARY KEY NOT NULL IDENTITY,
	[OwnerRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallAction_Owner FOREIGN KEY REFERENCES Person(Id),
	[PersonRef] INT NULL CONSTRAINT FK_ApplicationInstallAction_Person FOREIGN KEY REFERENCES Person(Id),
	[ApplicationRef] UNIQUEidentifier NOT NULL,
	[Description] NVARCHAR(max) NULL
)
GO

CREATE TABLE [dbo].[ApplicationInstallActionRole](
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[RoleId] INT NOT NULL,
	[AppInstallActionRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallActionRole_ApplicationInstallAction FOREIGN KEY REFERENCES ApplicationInstallAction(Id)
)
GO

CREATE TABLE [dbo].[ApplicationInstallActionClasses](
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[ClassRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallActionClasses_Class FOREIGN KEY REFERENCES Class(Id),
	[AppInstallActionRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallActionClasses_ApplicationInstallAction FOREIGN KEY REFERENCES ApplicationInstallAction(Id)
)
GO

CREATE TABLE [dbo].[ApplicationInstallActionDepartment](
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[DepartmentRef] UNIQUEidentifier NOT NULL,
	[AppInstallActionRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallActionDepartment_ApplicationInstallAction FOREIGN KEY REFERENCES ApplicationInstallAction(Id),
)
GO

CREATE TABLE [dbo].[ApplicationInstallActionGradeLevel](
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[GradeLevelRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallActionGradeLevel_GradeLevel FOREIGN KEY REFERENCES GradeLevel(Id),
	[AppInstallActionRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallActionGradeLevel_ApplicationInstallAction FOREIGN KEY REFERENCES ApplicationInstallAction(Id),
)
GO

CREATE TABLE [dbo].[ApplicationInstall](
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[ApplicationRef] UNIQUEidentifier NOT NULL,
	[PersonRef] INT NOT NULL CONSTRAINT FK_ApplicationInstall_Person FOREIGN KEY REFERENCES Person(Id),
	[InstallDate] DATETIME2 NOT NULL,
	[SchoolYearRef] INT NOT NULL CONSTRAINT FK_ApplicationInstall_SchoolYear FOREIGN KEY REFERENCES SchoolYear(Id),
	[OwnerRef] INT NOT NULL CONSTRAINT FK_ApplicationInstall_Owner FOREIGN KEY REFERENCES Person(Id),
	[Active] BIT NOT NULL,
	[AppInstallActionRef] INT NOT NULL CONSTRAINT FK_ApplicationInstall_ApplicationInstallAction FOREIGN KEY REFERENCES ApplicationInstallAction(Id),
)
GO

CREATE TABLE GradingStyle
(
	Id INT NOT NULL PRIMARY KEY,
	GradingStyleValue INT NOT NULL, 
	MaxValue INT NOT NULL,
	StyledValue INT NOT NULL
)
GO

create table SchoolGradeLevel
(
	SchoolRef int not null constraint FK_SchoolGradeLevel_School foreign key references School(Id),
	GradeLevelRef int not null constraint FK_SchoolGradeLevel_GradeLevel foreign key references GradeLevel(Id),
	constraint PK_SchoolGradeLevelId primary key (SchoolRef, GradeLevelRef)
)
go

create table ClassAnnouncementType
(
	Id int not null primary key,
	Name nvarchar(255),
	[Description] nvarchar(1024),
	Gradable bit null,
	Percentage int not null,
	AnnouncementTypeRef int not null constraint FK_ClassAnnouncementType_AnnouncementType foreign key references AnnouncementType(Id),
	ClassRef int not null constraint FK_ClassAnnouncementType_Class foreign key references Class(Id)
)
go

alter table Announcement
drop constraint FK_Announcement_AnnouncementType
go

alter table Announcement
drop column AnnouncementTypeRef
go

alter table Announcement
add ClassAnnouncementTypeRef int null constraint FK_Announcement_ClassAnnouncementType foreign key references ClassAnnouncementType(Id)
go


alter table Room 
add SchoolRef int not null constraint FK_Room_School foreign key references School(Id)
go

sp_RENAME 'Date.[Date]' , 'Day', 'COLUMN'

alter table ClassPeriod
alter column RoomRef int null
go

alter table ClassPeriod 
add SchoolRef int not null constraint FK_ClassPeriod_School foreign key references School(Id)
go

alter table Period 
add SchoolRef int not null constraint FK_Period_School foreign key references School(Id)
go


alter table MarkingPeriod 
add SchoolRef int not null constraint FK_MarkingPeriod_School foreign key references School(Id)
go

alter table Class 
add SchoolRef int not null constraint FK_Class_School foreign key references School(Id)
go

alter table MarkingPeriodClass 
add SchoolRef int not null constraint FK_MarkingPeriodClass_School foreign key references School(Id)
go

alter table [Date] 
add SchoolRef int not null constraint FK_Date_School foreign key references School(Id)
go

alter table Announcement 
add SchoolRef int not null constraint FK_Announcement_School foreign key references School(Id)
go

