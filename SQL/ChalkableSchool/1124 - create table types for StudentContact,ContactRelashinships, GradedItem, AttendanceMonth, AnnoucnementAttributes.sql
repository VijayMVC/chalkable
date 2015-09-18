CREATE TYPE TAttendanceMonth AS TABLE
(
	Id int not null,
	SchoolYearRef int not null,
	Name nvarchar(30) not null,
	StartDate datetime2 not null,
	EndDate datetime2 not null,
	EndTime datetime2 not null,
	IsLockedAttendance bit not null, 
	IsLockedDiscipline bit not null
)
go

CREATE TYPE TGradedItem as TABLE
(
	Id int not null,
	GradingPeriodRef int not null,
	Name nvarchar(20) not null, 
	[Description] nvarchar(255) not null, 
	AlphaOnly bit not null,
	AppearsOnReportCard bit not null,
	DetGradePoints bit not null,
	DetGradeCredit bit not null,
	PostToTranscript bit not null, 
	AllowExemption bit not null,
	DisplayAsAvgInGradebook bit not null,
	PostRoundedAverage bit not null,
	Sequence int not null,
	AveragingRule nchar(1) not null 
)
go

create TYPE TAnnouncementAttribute as table
(
	Id int not null,
	Code nvarchar(5) not null,
	Name nvarchar(50) not null,
	[Description] nvarchar(255) not null,
	StateCode nvarchar(10) not null,
	SIFCode nvarchar(10) not null,
	NCESCode nvarchar(10) not null,
	IsActive bit not null,
	IsSystem bit not null
)
Go

create type TContactRelationship as table (
	[Id] int NOT NULL,
	[Code] nvarchar(5) NOT NULL,
	[Name] nvarchar(50) NOT NULL,
	[Description] nvarchar(255) NOT NULL,
	[ReceivesMailings] [bit] NOT NULL,
	[CanPickUp] [bit] NOT NULL,
	[IsFamilyMember] [bit] NOT NULL,
	[IsCustodian] [bit] NOT NULL,
	[IsEmergencyContact] [bit] NOT NULL,
	[StateCode] nvarchar(10) NOT NULL,
	[SIFCode] nvarchar(10) NOT NULL,
	[NCESCode] nvarchar(10) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsSystem] [bit] NOT NULL
)
go

CREATE type TStudentContact as table
(
	StudentRef [int] NOT NULL,
	ContactRef [int] NOT NULL,
	[ContactRelationshipRef] int NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[ReceivesMailings] [bit] NOT NULL,
	[CanPickUp] [bit] NOT NULL,
	[IsFamilyMember] [bit] NOT NULL,
	[IsCustodian] [bit] NOT NULL,
	[IsEmergencyContact] [bit] NOT NULL,
	[IsResponsibleForBill] [bit] NOT NULL,
	[ReceivesBill] [bit] NOT NULL,
	[StudentVisibleInHome] [bit] NOT NULL
)
Go