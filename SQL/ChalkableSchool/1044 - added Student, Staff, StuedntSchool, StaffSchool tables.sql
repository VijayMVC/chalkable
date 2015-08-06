create table Student
(
	Id int not null primary key constraint FK_Student_Person foreign key references Person(Id),
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[BirthDate] [datetime2](7) NULL,
	[Gender] [nvarchar](255) NULL,
	[HasMedicalAlert] [bit] NOT NULL,
	[IsAllowedInetAccess] [bit] NOT NULL,
	[SpecialInstructions] [nvarchar](1024) NOT NULL,
	[SpEdStatus] [nvarchar](256) NULL,
	[PhotoModifiedDate] [datetime2](7) NULL,
	[UserId] [int] NOT NULL
)
go

create table Staff
(
	Id int not null primary key constraint FK_Staff_Person foreign key references Person(Id),
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[BirthDate] [datetime2](7) NULL,
	[Gender] [nvarchar](255) NULL,
	[UserId] [int] NULL
)
go

create table StudentSchool
(
	StudentRef int not null constraint FK_StudentSchool_Student foreign key references [Student](Id),
	SchoolRef int not null constraint FK_StudentSchool_School foreign key references [School](Id)
)
go
ALTER TABLE StudentSchool
ADD CONSTRAINT PK_StudentSchool PRIMARY KEY (StudentRef,SchoolRef)
go
create table StaffSchool
(
	StaffRef int not null constraint FK_StaffSchool_Staff foreign key references [Staff](Id),
	SchoolRef  int not null constraint FK_StaffSchool_School foreign key references [School](Id)	
)
go
ALTER TABLE StaffSchool
ADD CONSTRAINT PK_StaffSchool PRIMARY KEY (StaffRef,SchoolRef)
go

