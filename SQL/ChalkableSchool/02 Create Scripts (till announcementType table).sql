create table Person
(
	 [Id] [uniqueidentifier] primary key NOT NULL,
	 RoleRef int not null,
	 LastMailNotification datetime2 null,
	 SisId int null
)
go

CREATE TABLE [dbo].[Address](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[Value] [nvarchar](2048) NOT NULL,
	[Note] [nvarchar](2048) NULL,
	[Type] [int] NOT NULL,
	[PersonRef] [uniqueidentifier] NOT NULL,
	[SisId] [int] NULL,
)
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Person] FOREIGN KEY([PersonRef])
REFERENCES [dbo].[Person] ([Id])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_Person]
GO


---------------------------------
-- Create AnnouncementType
---------------------------------

CREATE TABLE [dbo].[AnnouncementType](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[IsSystem] [bit] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[Gradeable] [bit] NULL,
)
GO

--------------------------------
--SchoolYear
--------------------------------

CREATE TABLE [dbo].[SchoolYear](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[IsCurrent] [bit] NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[SisId] [int] NULL
)

GO

alter table SchoolYear
add constraint QU_SchoolYear_Name unique(Name)
go 

--------------------------------
--- MarkingPeriod
--------------------------------

CREATE TABLE [dbo].[MarkingPeriod](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[SchoolYearRef] [uniqueidentifier] NOT NULL,
	[WeekDays] [int] NOT NULL,
	[SisId] [int] NULL,
)
GO
ALTER TABLE [dbo].[MarkingPeriod] ADD  DEFAULT ((126)) FOR [WeekDays]
GO
ALTER TABLE [dbo].[MarkingPeriod]  WITH NOCHECK ADD  CONSTRAINT [FK_MarkingPeriod_SchoolYear] FOREIGN KEY([SchoolYearRef])
REFERENCES [dbo].[SchoolYear] ([Id])
GO
ALTER TABLE [dbo].[MarkingPeriod] CHECK CONSTRAINT [FK_MarkingPeriod_SchoolYear]
GO

--------------------------------
--Create Courseinfo
--------------------------------
CREATE TABLE [dbo].[CourseInfo](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[Code] [nvarchar](255) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[ChalkableDepartmentRef] [uniqueidentifier] NULL,
	[SisId] [int] NULL,
)
GO

---------------------------------
--Create GradeLevel
---------------------------------
CREATE TABLE [dbo].[GradeLevel](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
)
GO
alter table [GradeLevel]
add constraint QU_GradeLevel_Name unique(Name)
go 

---------------------------------
--Create Class
---------------------------------

CREATE TABLE [dbo].[Class](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[SchoolYearRef] [uniqueidentifier] NOT NULL constraint FK_Class_SchoolYearRef foreign key references SchoolYear(Id),
	[CourseInfoRef] [uniqueidentifier] NOT NULL constraint FK_Class_CourseInfoRef foreign key references CourseInfo(Id),
	[TeacherRef] [uniqueidentifier] NULL constraint FK_Class_Person foreign key references Person(Id),
	[GradeLevelRef] [uniqueidentifier] NOT NULL constraint FK_Class_GradeLevel foreign key references GradeLevel(Id),
	[SisId] [int] NULL,
)
GO


--------------------------------
-- Create MarkingPeriodClass
--------------------------------
CREATE TABLE [dbo].[MarkingPeriodClass](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[ClassRef] [uniqueidentifier] NOT NULL constraint [FK_MarkingPeriodClass_Class] foreign key references Class(Id),
	[MarkingPeriodRef] [uniqueidentifier] NOT NULL constraint [FK_MarkingPeriodClass_MarkingPeriod] foreign key references MarkingPeriod(Id),
)
GO

---------------------------------
-- Create Announcement
---------------------------------
CREATE TABLE [dbo].[Announcement](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[PersonRef] [uniqueidentifier] NOT NULL constraint [FK_Announcement_Person] foreign key references Person(Id),
	[Content] [nvarchar](max) NULL,
	[Created] [datetime2](7) NOT NULL,
	[Expires] [datetime2](7) NOT NULL,
	[AnnouncementTypeRef] [uniqueidentifier] NOT NULL constraint [FK_Announcement_AnnouncementType] foreign key references AnnouncementType(Id),
	[State] [int] NOT NULL,
	[GradingStyle] [int] NOT NULL,
	[Subject] [nvarchar](255) NULL,
	[MarkingPeriodClassRef] [uniqueidentifier] NULL constraint [FK_Announcement_MarkingPeriodClass] foreign key references MarkingPeriodClass(Id),
	[Order] [int] NOT NULL,
	[Dropped] [bit] NOT NULL,
)
GO

---------------------------------
-- Create AnnouncementApplication
---------------------------------
CREATE TABLE [dbo].[AnnouncementApplication](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[AnnouncementRef] [uniqueidentifier] NOT NULL constraint [FK_AnnouncementApplication_Announcement] foreign key references [Announcement](Id),
	[ApplicationRef] [uniqueidentifier] NOT NULL,
	[Active] [bit] NOT NULL,
	[Order] [int] NOT NULL,
)
GO

---------------------------------
-- Create [AnnouncementAttachment]
---------------------------------
CREATE TABLE [dbo].[AnnouncementAttachment](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[PersonRef] [uniqueidentifier] NOT NULL constraint [FK_AnnouncementAttachment_Person] foreign key references Person(Id),
	[AnnouncementRef] [uniqueidentifier] NOT NULL constraint [FK_AnnouncementAttachment_Announcement] foreign key references [Announcement](Id),
	[AttachedDate] [datetime2](7) NOT NULL,
	[Uuid] [nvarchar](255) NULL,
	[Order] [int] NOT NULL,
)
GO


---------------------------------
-- Create [AnnouncementQnA]
---------------------------------
CREATE TABLE [dbo].[AnnouncementQnA](
	[id] [uniqueidentifier] primary key NOT NULL,
	[AnnouncementRef] [uniqueidentifier] NOT NULL CONSTRAINT [FK_AnnouncementQnA_Announcement] FOREIGN KEY REFERENCES [dbo].[Announcement] ([Id]),
	[PersonRef] [uniqueidentifier] NOT NULL CONSTRAINT [FK_AnnouncementQnA_Person] FOREIGN KEY REFERENCES [dbo].[Person] ([Id]),
	[Question] [nvarchar](max) NOT NULL,
	[Answer] [nvarchar](max) NULL,
	[State] [int] NOT NULL,
	[AnsweredTime] [datetime2](7) NULL,
	[QuestionTime] [datetime2](7) NULL,
)
GO

---------------------------------
-- Create [AnnouncementRecipient]
---------------------------------

CREATE TABLE [dbo].[AnnouncementRecipient](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[AnnouncementRef] [uniqueidentifier] NOT NULL  CONSTRAINT [FK_AnnouncementRecipient_Announcement] FOREIGN KEY REFERENCES [dbo].[Announcement] ([Id]),
	[ToAll] [bit] NOT NULL,
	[RoleRef] [int] NULL,
	[GradeLevelRef] [uniqueidentifier] NULL CONSTRAINT [FK_AnnouncementRecipient_GradeLevel] FOREIGN KEY REFERENCES [dbo].[GradeLevel] ([Id]),
	[PersonRef] [uniqueidentifier] NULL CONSTRAINT [FK_AnnouncementRecipient_Person] FOREIGN KEY REFERENCES [dbo].[Person] ([Id]),
)
GO

---------------------------------
-- Create [AnnouncementRecipientData]
---------------------------------
CREATE TABLE [dbo].[AnnouncementRecipientData](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[AnnouncementRef] [uniqueidentifier] NOT NULL CONSTRAINT [FK_AnnouncementRecipientData_Announcement] FOREIGN KEY REFERENCES [dbo].[Announcement] ([Id]),
	[PersonRef] [uniqueidentifier] NOT NULL CONSTRAINT [FK_AnnouncementRecipientData_Person] FOREIGN KEY REFERENCES [dbo].[Person] ([Id]),
	[Starred] [bit] NOT NULL,
	[StarredAutomatically] [int] NOT NULL,
)
GO

---------------------------------
-- Create [AnnouncementReminder]
---------------------------------

CREATE TABLE [dbo].[AnnouncementReminder](
	[Id] [uniqueidentifier] primary key NOT NULL,
	[ReminDate] [datetime2](7) NULL,
	[Processed] [bit] NOT NULL,
	[AnnouncementRef] [uniqueidentifier] NOT NULL CONSTRAINT [FK_AnnouncementReminder_Announcement] FOREIGN KEY REFERENCES [dbo].[Announcement] ([Id]),
	[Before] [int] NULL,
	[PersonRef] [uniqueidentifier] NULL CONSTRAINT [FK_AnnouncementReminder_Person] FOREIGN KEY REFERENCES [dbo].[Person] ([Id]),
)
GO


