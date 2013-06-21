CREATE TABLE [dbo].[ReportDownload](
	[Id] uniqueidentifier primary key NOT NULL,
	[Format] [int] NOT NULL,
	[ImportSystemType] [int] NOT NULL,
	[PersonRef] uniqueidentifier NOT NULL constraint FK_ReportDownload_Person foreign key references Person(Id),
	[ReportType] int not null ,
	[DownloadDate] [datetime2](7) NOT NULL,
	[FriendlyName] [nvarchar](1024) NOT NULL,
)
go


CREATE TABLE [dbo].[ReportMailDelivery](
	[Id] uniqueidentifier primary key NOT NULL,
	[ReportType] int not null ,
	[Format] [int] NOT NULL,
	[Frequency] [int] NOT NULL,
	[PersonRef] uniqueidentifier NOT NULL constraint FK_ReportMailDelivery_Person foreign key references Person(Id),
	[SendHour] [int] NULL,
	[SendDay] [int] NULL,
	[LastSentMarkingPeriodRef] uniqueidentifier NULL constraint FK_ReportMailDelivery_MarkingPeriod foreign key references MarkingPeriod(Id),
	[LastSentTime] [datetime2](7) NULL
)
go


CREATE TABLE [dbo].[ApplicationInstallAction](
	[Id] uniqueidentifier primary key NOT NULL,
	[OwnerRef] uniqueidentifier NOT NULL constraint FK_ApplicationInstallAction_Owner foreign key references Person(Id),
	[PersonRef] uniqueidentifier NULL  constraint FK_ApplicationInstallAction_Person foreign key references Person(Id),
	[ApplicationRef] uniqueidentifier NOT NULL,
	[Description] [nvarchar](max) NULL
)

CREATE TABLE [dbo].[ApplicationInstallActionRole](
	[Id] uniqueidentifier primary key NOT NULL,
	[RoleId] [int] NOT NULL,
	[AppInstallActionRef] uniqueidentifier NOT NULL constraint FK_ApplicationInstallActionRole_ApplicationInstallAction foreign key references ApplicationInstallAction(Id)
)

CREATE TABLE [dbo].[ApplicationInstallActionClasses](
	[Id] uniqueidentifier primary key NOT NULL,
	[ClassRef] uniqueidentifier NOT NULL constraint FK_ApplicationInstallActionClasses_Class foreign key references Class(Id),
	[AppInstallActionRef]  uniqueidentifier NOT NULL constraint FK_ApplicationInstallActionClasses_ApplicationInstallAction foreign key references ApplicationInstallAction(Id)
)

CREATE TABLE [dbo].[ApplicationInstallActionDepartment](
	[Id] uniqueidentifier primary key NOT NULL,
	[DepartmentRef] uniqueidentifier NOT NULL,
	[AppInstallActionRef] uniqueidentifier NOT NULL constraint FK_ApplicationInstallActionDepartment_ApplicationInstallAction foreign key references ApplicationInstallAction(Id),
)

CREATE TABLE [dbo].[ApplicationInstallActionGradeLevel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GradeLevelRef] uniqueidentifier NOT NULL constraint FK_ApplicationInstallActionGradeLevel_GradeLevel foreign key references GradeLevel(Id),
	[AppInstallActionRef] uniqueidentifier NOT NULL constraint FK_ApplicationInstallActionGradeLevel_ApplicationInstallAction foreign key references ApplicationInstallAction(Id),
)

CREATE TABLE [dbo].[ApplicationInstall](
	[Id] uniqueidentifier primary key NOT NULL,
	[ApplicationRef] uniqueidentifier NOT NULL,
	[PersonRef] uniqueidentifier NOT NULL constraint FK_ApplicationInstall_Person foreign key references Person(Id),
	[InstallDate] [datetime] NOT NULL,
	[SchoolYearRef] uniqueidentifier NOT NULL constraint FK_ApplicationInstall_SchoolYear foreign key references SchoolYear(Id),
	[OwnerRef] uniqueidentifier NOT NULL constraint FK_ApplicationInstall_Owner foreign key references Person(Id),
	[Active] [bit] NOT NULL,
	[AppInstallActionRef] uniqueidentifier NOT NULL constraint FK_ApplicationInstall_ApplicationInstallAction foreign key references ApplicationInstallAction(Id),
)