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
	[CountyID] SMALLINT NULL,
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
	[FirstLoginDate] DATETIME2(7) NULL
	[RoleRef] INT NOT NULL,
	[LastMailNotification] DATETIME2 NULL,
	[Email] NVARCHAR(256) not null,
	AddressRef INT CONSTRAINT FK_Person_Address FOREIGN KEY REFERENCES [Address](Id)
)
GO

CREATE TABLE [dbo].[AnnouncementType](
	[Id] INT PRIMARY KEY IDENTITY NOT NULL,
	[IsSystem] BIT NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(1024) NULL,
	Gradable BIT NULL,
)
GO

CREATE TABLE [dbo].[SchoolYear](
	[Id] INT NOT NULL PRIMARY KEY,
	SchoolRef INT not null CONSTRAINT FK_SchoolYear_School FOREIGN KEY REFERENCES School(Id)
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(1024) NULL,
	[StartDate] DATETIME2(7) NOT NULL,
	[EndDate] DATETIME2(7) NOT NULL,
)

GO

Alter TABLE SchoolYear
	Add CONSTRAINT QU_SchoolYear_Name UNIQUE(Name, SchoolId)
GO

CREATE TABLE [dbo].[MarkingPeriod](
	[Id] INT not null PRIMARY KEY,
	[Name] NVARCHAR(255) NOT NULL,
	[StartDate] DATETIME2(7) NOT NULL,
	[EndDate] DATETIME2(7) NOT NULL,
	[Description] NVARCHAR(1024) NULL,
	[SchoolYearRef] [UNIQUEidentifier] NOT NULL CONSTRAINT FK_MarkingPeriod_SchoolYear FOREIGN KEY REFERENCES SchoolYear(Id),
	[WeekDays] INT NOT NULL DEFAULT (126)
)
GO

CREATE TABLE [dbo].[GradeLevel](
	[Id] INT not null PRIMARY KEY,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(1024),
	[Number] INT not null
)
GO

Alter TABLE [GradeLevel]
	Add CONSTRAINT QU_GradeLevel_Name UNIQUE(Name)
GO

Alter TABLE [GradeLevel]
	Add CONSTRAINT QU_GradeLevel_Sequence UNIQUE(Sequence)
GO

CREATE TABLE [dbo].[Class](
	[Id] INT not null PRIMARY KEY,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(1024) NULL,
	[ChalkableDepartmentRef] INT NULL,
	[SchoolYearRef] INT NOT NULL CONSTRAINT FK_Class_SchoolYearRef FOREIGN KEY REFERENCES SchoolYear(Id),
	[TeacherRef] INT NULL CONSTRAINT FK_Class_Person FOREIGN KEY REFERENCES Person(Id),
	[GradeLevelRef] INT NOT NULL CONSTRAINT FK_Class_GradeLevel FOREIGN KEY REFERENCES GradeLevel(Id)
)
GO

CREATE TABLE [dbo].[MarkingPeriodClass](
	[Id] INT PRIMARY KEY NOT NULL,
	[ClassRef] INT NOT NULL CONSTRAINT [FK_MarkingPeriodClass_Class] FOREIGN KEY REFERENCES Class(Id),
	[MarkingPeriodRef] INT NOT NULL CONSTRAINT [FK_MarkingPeriodClass_MarkingPeriod] FOREIGN KEY REFERENCES MarkingPeriod(Id),
)
GO


Alter TABLE MarkingPeriodClass
	Add CONSTRAINT UQ_MarkingPeriodClass_ClassRef_MarkingPeriodRef UNIQUE(ClassRef, MarkingPeriodRef)
GO

CREATE TABLE [dbo].[Announcement](
	[Id] INT PRIMARY KEY NOT NULL,
	[PersonRef] INT NOT NULL CONSTRAINT [FK_Announcement_Person] FOREIGN KEY REFERENCES Person(Id),
	[Content] NVARCHAR(max) NULL,
	[CREATEd] DATETIME2(7) NOT NULL,
	[Expires] DATETIME2(7) NOT NULL,
	[AnnouncementTypeRef] INT NOT NULL CONSTRAINT [FK_Announcement_AnnouncementType] FOREIGN KEY REFERENCES AnnouncementType(Id),
	[State] INT NOT NULL,
	[GradingStyle] INT NOT NULL,
	[Subject] NVARCHAR(255) NULL,
	[MarkingPeriodClassRef] INT NULL CONSTRAINT [FK_Announcement_MarkingPeriodClass] FOREIGN KEY REFERENCES MarkingPeriodClass(Id),
	[Order] INT NOT NULL,
	[Dropped] BIT NOT NULL,
)
GO

CREATE TABLE [dbo].[AnnouncementApplication](
	[Id] INT PRIMARY KEY NOT NULL,
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
	[id] INT PRIMARY KEY NOT NULL,
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
	[Id] INT PRIMARY KEY NOT NULL,
	[AnnouncementRef] INT NOT NULL  CONSTRAINT [FK_AnnouncementRecipient_Announcement] FOREIGN KEY REFERENCES [dbo].[Announcement] ([Id]),
	[ToAll] BIT NOT NULL,
	[RoleRef] INT NULL,
	[GradeLevelRef] INT NULL CONSTRAINT [FK_AnnouncementRecipient_GradeLevel] FOREIGN KEY REFERENCES [dbo].[GradeLevel] ([Id]),
	[PersonRef] INT NULL CONSTRAINT [FK_AnnouncementRecipient_Person] FOREIGN KEY REFERENCES [dbo].[Person] ([Id]),
)
GO

CREATE TABLE [dbo].[AnnouncementRecipientData](
	[Id] INT PRIMARY KEY NOT NULL,
	[AnnouncementRef] INT NOT NULL CONSTRAINT [FK_AnnouncementRecipientData_Announcement] FOREIGN KEY REFERENCES [dbo].[Announcement] ([Id]),
	[PersonRef] INT NOT NULL CONSTRAINT [FK_AnnouncementRecipientData_Person] FOREIGN KEY REFERENCES [dbo].[Person] ([Id]),
	[Starred] BIT NOT NULL,
	[StarredAutomatically] INT NOT NULL,
	LastModifiedDate DATETIME2 null
)
GO


CREATE TABLE [dbo].[AnnouncementReminder](
	[Id] INT PRIMARY KEY NOT NULL,
	[RemindDate] DATETIME2(7) NULL,
	[Processed] BIT NOT NULL,
	[AnnouncementRef] INT NOT NULL CONSTRAINT [FK_AnnouncementReminder_Announcement] FOREIGN KEY REFERENCES [dbo].[Announcement] ([Id]),
	[Before] INT NULL,
	[PersonRef] INT NULL CONSTRAINT [FK_AnnouncementReminder_Person] FOREIGN KEY REFERENCES [dbo].[Person] ([Id]),
)
GO

CREATE TABLE ClassPerson
(
	Id INT not null PRIMARY KEY,
	ClassRef INT not null CONSTRAINT FK_ClassPerson_Class FOREIGN KEY REFERENCES Class(Id),
	PersonRef INT not null CONSTRAINT FK_ClassPerson_Person FOREIGN KEY REFERENCES Person(Id),
)
GO


Alter TABLE ClassPerson
	Add CONSTRAINT QU_ClassPerson_PersonRef_ClassRef UNIQUE(PersonRef, ClassRef)
GO


CREATE TABLE StudentSchoolYear
(
	[Id] INT NOT NULL PRIMARY KEY,
	SchoolYearId INT not null CONSTRAINT FK_StudentSchoolYear_SchoolYear FOREIGN KEY REFERENCES SchoolYear(Id),
	GradeLevelRef INT not null CONSTRAINT FK_StudentSchoolYear_GradeLevel FOREIGN KEY REFERENCES GradeLevel(Id),
	StudentRef INT not null CONSTRAINT FK_StudentSchoolYear_Person FOREIGN KEY REFERENCES Person(Id)
)
GO

CREATE view [vwPerson]
as
select 	
	Person.Id as Id,
	Person.RoleRef as RoleId,
	Person.FirstName as FirstName,
	Person.LastName as LastName,
	Person.BirthDate as BirthDate,
	Person.Gender as Gender,
	Person.Salutation as Salutation,
	Person.Active as Active,
	Person.FirstLoginDate as FirstLogInDate,
	Person.Email as Email,
	GradeLevel.Id as GradeLevel_Id,
	GradeLevel.Name as GradeLevel_Name,
	StudentInfo.IEP as IEP,
	StudentInfo.EnrollmentDate as EnrollmentDate,
	StudentInfo.PreviousSchool as PreviousSchool,
	StudentInfo.PreviousSchoolNote as PreviousSchoolNote,
	StudentInfo.PreviousSchoolPhone as PreviousSchoolPhone
from 
	Person
	left join StudentSchoolYear on StudentSchoolYear.StudentRef = Person.Id
	left join GradeLevel on StudentSchoolYear.GradeLevelRef = GradeLevel.Id	
GO


CREATE TABLE StudentAnnouncement
(
	Id INT not null  PRIMARY KEY,
	PersonRef INT not null CONSTRAINT FK_StudentAnnouncement_Person FOREIGN KEY REFERENCES Person(Id),
	AnnouncementRef INT not null CONSTRAINT FK_StudentAnnouncement_Announcement FOREIGN KEY REFERENCES Announcement(Id),
	Comment NVARCHAR(1024) null,
	GradeValue INT null,
	ExtraCredit NVARCHAR(255) null,
	Dropped bit not null,
	State INT not null,
	ApplicationRef UNIQUEidentifier null
)
GO

CREATE View [vwAnnouncement] 
as 
Select 
	Announcement.Id as Id,
	Announcement.CREATEd as CREATEd,
	Announcement.Expires as Expires,
	Announcement.[State] as [State],
	Announcement.[Order] as [Order],
	Announcement.Content as Content,
	Announcement.[Subject] as [Subject],
	Announcement.GradingStyle as GradingStyle,
	Announcement.Dropped as Dropped,
	Announcement.AnnouncementTypeRef as AnnouncementTypeRef,
	AnnouncementType.Name as AnnouncementTypeName,
	Announcement.PersonRef as PersonRef,
	Announcement.MarkingPeriodClassRef as MarkingPeriodClassRef,
	Person.FirstName + ' ' + Person.LastName as PersonName,
	Person.Gender as PersonGender,
	Class.Name as ClassName,
	Class.GradeLevelRef as GradeLevelId,  
	Class.CourseRef as CourseId,
	MarkingPeriodClass.ClassRef as ClassId,
	MarkingPeriodClass.MarkingPeriodRef as MarkingPeriodId,
	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = Announcement.Id) as QnACount,	
	(Select COUNT(*) from ClassPerson where ClassRef = MarkingPeriodClass.ClassRef) as StudentsCount,
	(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id) as AttachmentsCount,
	(select count(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id and PersonRef = Announcement.PersonRef) as OwnerAttachmentsCount,
	(	select COUNT(*) from
			(Select distinct PersonRef from AnnouncementAttachment 
			where AnnouncementRef = Announcement.Id
			and PersonRef <> Announcement.PersonRef) as x
	) as StudentsCountWithAttachments,
	(Select COUNT(*) from StudentAnnouncement where AnnouncementRef = Announcement.Id and GradeValue is not null) as GradingStudentsCount, 
	(Select AVG(GradeValue) from StudentAnnouncement where AnnouncementRef = Announcement.Id and GradeValue is not null) as [Avg], 
	(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = Announcement.Id and Active = 1) as ApplicationCount

from 
	Announcement
	join AnnouncementType on Announcement.AnnouncementTypeRef = AnnouncementType.Id
	left join MarkingPeriodClass on MarkingPeriodClass.Id = Announcement.MarkingPeriodClassRef
	left join Class on Class.Id = MarkingPeriodClass.ClassRef
	left join Person on Person.Id = Announcement.PersonRef

GO



CREATE TABLE Phone
(
	Id INT PRIMARY KEY not null,
	PersonRef INT not null CONSTRAINT FK_Phone_Person FOREIGN KEY REFERENCES Person(Id),
	[Value] NVARCHAR(256) NOT NULL,
	[Type] INT NOT NULL,
	[IsPRIMARY] BIT NOT NULL,
	[DigitOnlyValue] NVARCHAR(256) NOT NULL
)
GO

CREATE TABLE [dbo].[PrivateMessage](
	[Id] INT not null PRIMARY KEY,
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
	Id INT PRIMARY KEY not null,
	[RoomNumber] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(1024) NULL,
	[Size] NVARCHAR(255) NULL,
	[Capacity] INT NULL,
	[PhoneNumber] NVARCHAR(255) NULL
)
GO

CREATE TABLE DayType
(
	Id INT not null PRIMARY KEY,
	Number INT not null,
	Name NVARCHAR(1024) not null,
	SchoolYearRef INT not null CONSTRAINT FK_DayType_SchoolYear FOREIGN KEY REFERENCES SchoolYear(Id)
)
GO

CREATE TABLE Period
(
	Id INT not null PRIMARY KEY,
	[StartTime] INT NOT NULL,
	[EndTime] INT NOT NULL,
	SchoolYearRef INT not null CONSTRAINT FK_Period_SchoolYear FOREIGN KEY REFERENCES SchoolYear(Id),
	[Order] INT NOT NULL
)
GO

CREATE TABLE ClassPeriod
(
	Id INT PRIMARY KEY not null,
	PeriodRef INT not null CONSTRAINT FK_ClassPeriod_Period FOREIGN KEY REFERENCES Period(Id),
	DayTypeRef INT not null CONSTRAINT FK_ClassPeriod_DayType FOREIGN KEY REFERENCES DayType(Id),
	ClassRef INT not null CONSTRAINT FK_ClassPeriod_Class FOREIGN KEY REFERENCES Class(Id),
	RoomRef INT not null CONSTRAINT FK_ClassPeriod_Room FOREIGN KEY REFERENCES Room(Id)
)
GO

Alter TABLE ClassPeriod
	Add CONSTRAINT QU_ClassPeriod_ClassRef_PeriodRef UNIQUE(ClassRef, PeriodRef)
GO

CREATE TABLE [Date]
(
	Id INT PRIMARY KEY,
	DATETIME2 DATETIME2 not null,
	DayTypeRef INT CONSTRAINT FK_Date_DayType FOREIGN KEY REFERENCES DayType(Id),
	SchoolYearRef INT not null CONSTRAINT FK_Date_SchoolYear FOREIGN KEY REFERENCES SchoolYear(Id),
	IsSchoolDay bit not null
)
GO

Alter TABLE [Date]
	Add CONSTRAINT QU_Date_DateTime UNIQUE(DATETIME2, SchoolYearRef)
GO


CREATE TABLE [dbo].[Notification](
	[Id] INT NOT NULL PRIMARY KEY,
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
	[ReportType] INT not null,
	[DownloadDate] DATETIME2(7) NOT NULL,
	[FriendlyName] NVARCHAR(1024) NOT NULL,
)
GO

CREATE TABLE [dbo].[ReportMailDelivery]
(
	[Id] INT PRIMARY KEY NOT NULL,
	[ReportType] INT not null ,
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
	[Id] INT PRIMARY KEY NOT NULL,
	[OwnerRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallAction_Owner FOREIGN KEY REFERENCES Person(Id),
	[PersonRef] INT NULL CONSTRAINT FK_ApplicationInstallAction_Person FOREIGN KEY REFERENCES Person(Id),
	[ApplicationRef] UNIQUEidentifier NOT NULL,
	[Description] NVARCHAR(max) NULL
)
GO

CREATE TABLE [dbo].[ApplicationInstallActionRole](
	[Id] INT PRIMARY KEY NOT NULL,
	[RoleId] INT NOT NULL,
	[AppInstallActionRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallActionRole_ApplicationInstallAction FOREIGN KEY REFERENCES ApplicationInstallAction(Id)
)
GO

CREATE TABLE [dbo].[ApplicationInstallActionClasses](
	[Id] INT PRIMARY KEY NOT NULL,
	[ClassRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallActionClasses_Class FOREIGN KEY REFERENCES Class(Id),
	[AppInstallActionRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallActionClasses_ApplicationInstallAction FOREIGN KEY REFERENCES ApplicationInstallAction(Id)
)
GO

CREATE TABLE [dbo].[ApplicationInstallActionDepartment](
	[Id] INT PRIMARY KEY NOT NULL,
	[DepartmentRef] UNIQUEidentifier NOT NULL,
	[AppInstallActionRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallActionDepartment_ApplicationInstallAction FOREIGN KEY REFERENCES ApplicationInstallAction(Id),
)
GO

CREATE TABLE [dbo].[ApplicationInstallActionGradeLevel](
	[Id] INT NOT NULL PRIMARY KEY,
	[GradeLevelRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallActionGradeLevel_GradeLevel FOREIGN KEY REFERENCES GradeLevel(Id),
	[AppInstallActionRef] INT NOT NULL CONSTRAINT FK_ApplicationInstallActionGradeLevel_ApplicationInstallAction FOREIGN KEY REFERENCES ApplicationInstallAction(Id),
)
GO

CREATE TABLE [dbo].[ApplicationInstall](
	[Id] INT PRIMARY KEY NOT NULL,
	[ApplicationRef] UNIQUEidentifier NOT NULL,
	[PersonRef] INT NOT NULL CONSTRAINT FK_ApplicationInstall_Person FOREIGN KEY REFERENCES Person(Id),
	[InstallDate] DATETIME2 NOT NULL,
	[SchoolYearRef] INT NOT NULL CONSTRAINT FK_ApplicationInstall_SchoolYear FOREIGN KEY REFERENCES SchoolYear(Id),
	[OwnerRef] INT NOT NULL CONSTRAINT FK_ApplicationInstall_Owner FOREIGN KEY REFERENCES Person(Id),
	[Active] BIT NOT NULL,
	[AppInstallActionRef] INT NOT NULL CONSTRAINT FK_ApplicationInstall_ApplicationInstallAction FOREIGN KEY REFERENCES ApplicationInstallAction(Id),
)
GO

Alter view [dbo].[vwClass]
as
select 
	Class.Id as Class_Id,
	Class.Name as Class_Name,
	Class.Description as Class_Description,
	Class.SchoolYearRef as Class_SchoolYearRef,
	Class.TeacherRef as Class_TeacherRef,
	Class.GradeLevelRef as Class_GradeLevelRef,
	Class.ChalkableDepartmentRef as Class_ChalkableDepartmentId,
	GradeLevel.Id as GradeLevel_Id,
	GradeLevel.Name as GradeLevel_Name,
	GradeLevel.Number as GradeLevel_Number,
	Person.Id as Person_Id,
	Person.FirstName as Person_FirstName,
	Person.LastName as Person_LastName,
	Person.Gender as Person_Gender,
	Person.Salutation as Person_Salutation,
	Person.Email as Person_Email 
from 
	Class	
	join GradeLevel on GradeLevel.Id = Class.GradeLevelRef
	join Person on Person.Id = Class.TeacherRef
GO

CREATE TABLE GradingStyle
(
	Id INT not null PRIMARY KEY,
	GradingStyleValue INT not null, 
	MaxValue INT not null,
	StyledValue INT not null
)
GO

CREATE view [vwAnnouncementQnA]
as
	select
		ana.id as Id,
		ana.AnsweredTime as AnsweredTime,
		ana.QuestionTime as QuestionTime,
		ana.Question as Question,
		ana.Answer as Answer,
		ana.AnnouncementRef as AnnouncementRef,
		a.MarkingPeriodClassRef as MarkingPeriodClassRef,
		ana.State as [State],
		sp1.Id as AskerId,
		sp1.FirstName as AskerFirstName,
		sp1.LastName as AskerLastName,
		sp1.Gender as AskerGender,
		sp1.RoleRef as AskerRoleRef,
		sp2.Id as AnswererId,
		sp2.FirstName as AnswererFirstName,
		sp2.LastName as AnswererLastName,
		sp2.Gender as AnswererGender,
		sp2.RoleRef as AnswererRoleRef
	from AnnouncementQnA ana
		join vwPerson sp1 on sp1.Id = ana.PersonRef
		join Announcement a on a.Id = ana.AnnouncementRef
		join vwPerson sp2 on sp2.Id = a.PersonRef
GO

CREATE View vwPrivateMessage
as
	select
		 PrivateMessage.Id as PrivateMessage_Id,
		 PrivateMessage.Body as PrivateMessage_Body,
		 PrivateMessage.[Read] as PrivateMessage_Read,
		 PrivateMessage.[Sent] as PrivateMessage_Sent,
		 PrivateMessage.[Subject] as PrivateMessage_Subject,
		 PrivateMessage.DeletedBySender as PrivateMessage_DeletedBySender,
		 PrivateMessage.DeletedByRecipient as PrivateMessage_DeletedByRecipient,
		 PrivateMessage.ToPersonRef as PrivateMessage_ToPersonRef,
		 PrivateMessage.FromPersonRef as PrivateMessage_FromPersonRef,
		 p.FirstName as PrivateMessage_SenderFirstName,
		 p.LastName as PrivateMessage_SenderLastName,
		 p.Gender as PrivateMessage_SenderGender,
		 p.Salutation as PrivateMessage_SenderSalutation,
		 p.RoleRef as PrivateMessage_SenderRoleRef,
		 p2.FirstName as PrivateMessage_RecipientFirstName,
		 p2.LastName as PrivateMessage_RecipientLastName,
		 p2.Gender as PrivateMessage_RecipientGender,
		 p2.Salutation as PrivateMessage_RecipientSalutation,
		 p2.RoleRef as PrivateMessage_RecipientRoleRef
	from PrivateMessage 
		join Person p on p.Id = PrivateMessage.FromPersonRef
		join Person p2 on p2.Id = PrivateMessage.ToPersonRef 
	
GO 

