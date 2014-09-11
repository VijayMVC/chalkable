create type TStudent as table
(
	Id int not null,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[BirthDate] [datetime2](7) NULL,
	[Gender] [nvarchar](255) NULL,
	[Salutation] [nvarchar](255) NULL,
	[Active] [bit] NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[AddressRef] [int] NULL,
	[HasMedicalAlert] [bit] NOT NULL,
	[IsAllowedInetAccess] [bit] NOT NULL,
	[SpecialInstructions] [nvarchar](1024) NOT NULL,
	[SpEdStatus] [nvarchar](256) NULL,
	[PhotoModifiedDate] [datetime2](7) NULL,
	[UserId] [int] NOT NULL
)
go
create type TStaff as table 
(
	Id int not null,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[BirthDate] [datetime2](7) NULL,
	[Gender] [nvarchar](255) NULL,
	[Salutation] [nvarchar](255) NULL,
	[Email] [nvarchar](256) NOT NULL,
	[AddressRef] [int] NULL,
	[Active] [bit] NOT NULL,
	[UserId] [int] NULL
)
go
create type TStaffSchool as table 
(
	StaffRef int not null,
	SchoolRef  int not null
)
go
create type TStudentSchool as table
(
	StudentRef int not null,
	SchoolRef int not null	
)
go