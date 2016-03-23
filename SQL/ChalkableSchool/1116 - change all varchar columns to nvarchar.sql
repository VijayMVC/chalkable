ALTER TABLE [Address]
	ALTER COLUMN [AddressNumber] NCHAR(15) NOT NULL

ALTER TABLE [Address]
	ALTER COLUMN [PostalCode] NCHAR(10) NOT NULL

ALTER TABLE [AlphaGrade]
	ALTER COLUMN [Name] NVARCHAR(5) NOT NULL

ALTER TABLE [AlphaGrade]
	ALTER COLUMN [Description] NVARCHAR(255) NOT NULL

ALTER TABLE [AlternateScore]
	ALTER COLUMN [Name] NVARCHAR(3) NOT NULL

ALTER TABLE [AlternateScore]
	ALTER COLUMN [Description] NVARCHAR(255) NOT NULL

ALTER TABLE [GradingComment]
	ALTER COLUMN [Comment] NVARCHAR(50) NOT NULL

ALTER TABLE [GradingScale]
	ALTER COLUMN [Description] NVARCHAR(255) NOT NULL

ALTER TABLE [Infraction]
	ALTER COLUMN [Description] NVARCHAR(255) NOT NULL

ALTER TABLE [Infraction]
	ALTER COLUMN [StateCode] NVARCHAR(10) NOT NULL

ALTER TABLE [Infraction]
	ALTER COLUMN [SIFCode] NVARCHAR(10) NOT NULL

ALTER TABLE [Infraction]
	ALTER COLUMN [NCESCode] NVARCHAR(10) NOT NULL

ALTER TABLE [ScheduledTimeSlot]
	ALTER COLUMN [Description] NVARCHAR(255) NOT NULL

ALTER TABLE [ScheduledTimeSlotVariation]
	ALTER COLUMN [Description] NVARCHAR(255) NOT NULL

ALTER TABLE [SchoolOption]
	ALTER COLUMN [TimeZoneName] NVARCHAR(32) NULL

ALTER TABLE [Standard]
	ALTER COLUMN [Name] NVARCHAR(100) NOT NULL

ALTER TABLE [Standard]
	ALTER COLUMN [Description] NVARCHAR(max) NOT NULL

ALTER TABLE [StandardSubject]
	ALTER COLUMN [Name] NVARCHAR(100) NOT NULL

ALTER TABLE [StandardSubject]
	ALTER COLUMN [Description] NVARCHAR(200) NOT NULL

ALTER TABLE [Student]
	ALTER COLUMN [SpecialInstructions] NVARCHAR(max) NOT NULL

ALTER TABLE [GradingComment]
	DROP CONSTRAINT [UQ_GradingComment_SchoolRef_Code]

ALTER TABLE [GradingComment]
	ALTER COLUMN [Code] NVARCHAR(5) NOT NULL

ALTER TABLE [GradingComment] ADD CONSTRAINT [UQ_GradingComment_SchoolRef_Code] UNIQUE (
	SchoolRef
	,Code
	)

ALTER TABLE [GradingScale]
	DROP CONSTRAINT [UQ_GradingScale_SchoolRef_Name]

ALTER TABLE [GradingScale]
	ALTER COLUMN [Name] NVARCHAR(20) NOT NULL

ALTER TABLE [GradingScale] ADD CONSTRAINT [UQ_GradingScale_SchoolRef_Name] UNIQUE (
	SchoolRef
	,NAME
	)

ALTER TABLE [Infraction]
	DROP CONSTRAINT [UQ_Infraction_Code]

ALTER TABLE [Infraction]
	ALTER COLUMN [Code] NVARCHAR(5) NOT NULL

ALTER TABLE [Infraction] ADD CONSTRAINT [UQ_Infraction_Code] UNIQUE (Code)

ALTER TABLE [Infraction]
	DROP CONSTRAINT [UQ_Infraction_Name]

ALTER TABLE [Infraction]
	ALTER COLUMN [Name] NVARCHAR(50) NOT NULL

ALTER TABLE [Infraction] ADD CONSTRAINT [UQ_Infraction_Name] UNIQUE (NAME)

ALTER TABLE [ScheduledTimeSlotVariation]
	DROP CONSTRAINT [UQ_BellSchedule_TimeSlot_Name]

ALTER TABLE [ScheduledTimeSlotVariation]
	ALTER COLUMN [Name] NVARCHAR(50) NOT NULL

ALTER TABLE [ScheduledTimeSlotVariation] ADD CONSTRAINT [UQ_BellSchedule_TimeSlot_Name] UNIQUE (
	BellScheduleRef
	,PeriodRef
	,NAME
	)

DROP type [dbo].[TGradingComment]

CREATE type [TGradingComment] AS TABLE (
	[Id] [int] NOT NULL
	,[SchoolRef] [int] NOT NULL
	,[Code] NVARCHAR(5) NOT NULL
	,[Comment] NVARCHAR(50) NOT NULL
	)

DROP type [dbo].[TGradingScale]

CREATE type [TGradingScale] AS TABLE (
	[Id] [int] NOT NULL
	,[SchoolRef] [int] NOT NULL
	,[Name] NVARCHAR(20) NOT NULL
	,[Description] NVARCHAR(255) NOT NULL
	,[HomeGradeToDisplay] [int] NULL
	)

DROP type [dbo].[TInfraction]

CREATE type [TInfraction] AS TABLE (
	[Id] [int] NOT NULL
	,[Code] NVARCHAR(5) NOT NULL
	,[Name] NVARCHAR(50) NOT NULL
	,[Description] NVARCHAR(255) NOT NULL
	,[Demerits] [tinyint] NOT NULL
	,[StateCode] NVARCHAR(10) NOT NULL
	,[SIFCode] NVARCHAR(10) NOT NULL
	,[NCESCode] NVARCHAR(10) NOT NULL
	,[IsActive] [bit] NOT NULL
	,[IsSystem] [bit] NOT NULL
	)

DROP type [dbo].[TScheduledTimeSlot]

CREATE type [TScheduledTimeSlot] AS TABLE (
	[BellScheduleRef] [int] NOT NULL
	,[PeriodRef] [int] NOT NULL
	,[StartTime] [int] NULL
	,[EndTime] [int] NULL
	,[Description] NVARCHAR(255) NOT NULL
	,[IsDailyAttendancePeriod] [bit] NOT NULL
	)

DROP type [dbo].[TStudent]

CREATE type [TStudent] AS TABLE (
	[Id] [int] NOT NULL
	,[FirstName] NVARCHAR(510) NOT NULL
	,[LastName] NVARCHAR(510) NOT NULL
	,[BirthDate] [datetime2] NULL
	,[Gender] NVARCHAR(510) NULL
	,[HasMedicalAlert] [bit] NOT NULL
	,[IsAllowedInetAccess] [bit] NOT NULL
	,[SpecialInstructions] NVARCHAR(max) NOT NULL
	,[SpEdStatus] NVARCHAR(512) NULL
	,[PhotoModifiedDate] [datetime2] NULL
	,[UserId] [int] NOT NULL
	)

DROP type [dbo].[TSchoolOption]

CREATE type [TSchoolOption] AS TABLE (
	[Id] [int] NOT NULL
	,[AllowSectionAverageModification] [bit] NOT NULL
	,[EarliestPaymentDate] [datetime2] NULL
	,[NextReceiptNumber] [int] NULL
	,[DefaultCombinationIndex] [int] NULL
	,[TimeZoneName] NVARCHAR(32) NULL
	,[BaseHoursOffset] [int] NULL
	,[BaseMinutesOffset] [int] NULL
	,[ObservesDst] [bit] NULL
	,[AllowScoreEntryForUnexcused] [bit] NOT NULL
	,[DisciplineOverwritesAttendance] [bit] NOT NULL
	,[AllowDualEnrollment] [bit] NOT NULL
	,[CompleteStudentScheduleDefinition] NVARCHAR(2) NOT NULL
	,[AveragingMethod] NVARCHAR(2) NOT NULL
	,[CategoryAveraging] [bit] NOT NULL
	,[StandardsGradingScaleRef] [int] NULL
	,[StandardsCalculationMethod] NVARCHAR(2) NOT NULL
	,[StandardsCalculationRule] NVARCHAR(2) NOT NULL
	,[StandardsCalculationWeightMaximumValues] [bit] NOT NULL
	,[LockCategories] [bit] NOT NULL
	,[IncludeReportCardCommentsInGradebook] [bit] NOT NULL
	,[MergeRostersForAttendance] [bit] NOT NULL
	)

DROP type [dbo].[TStandardSubject]

CREATE type [TStandardSubject] AS TABLE (
	[Id] [int] NOT NULL
	,[Name] NVARCHAR(100) NOT NULL
	,[Description] NVARCHAR(200) NOT NULL
	,[AdoptionYear] [int] NULL
	,[IsActive] [bit] NOT NULL
	)

DROP type [dbo].[TScheduledTimeSlotVariation]

CREATE type [TScheduledTimeSlotVariation] AS TABLE (
	[Id] [int] NOT NULL
	,[BellScheduleRef] [int] NOT NULL
	,[PeriodRef] [int] NOT NULL
	,[Name] NVARCHAR(50) NOT NULL
	,[Description] NVARCHAR(255) NOT NULL
	,[StartTime] [int] NOT NULL
	,[EndTime] [int] NOT NULL
	)

DROP type [dbo].[TStandard]

CREATE type [TStandard] AS TABLE (
	[Id] [int] NOT NULL
	,[ParentStandardRef] [int] NULL
	,[Name] NVARCHAR(100) NOT NULL
	,[Description] NVARCHAR(max) NOT NULL
	,[StandardSubjectRef] [int] NOT NULL
	,[LowerGradeLevelRef] [int] NULL
	,[UpperGradeLevelRef] [int] NULL
	,[IsActive] [bit] NOT NULL
	,[AcademicBenchmarkId] [uniqueidentifier] NULL
	)

DROP type [dbo].[TAddress]

CREATE type [TAddress] AS TABLE (
	[Id] [int] NOT NULL
	,[AddressNumber] NCHAR(15) NOT NULL
	,[StreetNumber] NVARCHAR(20) NOT NULL
	,[AddressLine1] NVARCHAR(150) NOT NULL
	,[AddressLine2] NVARCHAR(150) NOT NULL
	,[City] NVARCHAR(100) NOT NULL
	,[State] NVARCHAR(10) NOT NULL
	,[PostalCode] NCHAR(10) NOT NULL
	,[Country] NVARCHAR(120) NOT NULL
	,[CountyId] [int] NULL
	,[Latitude] [decimal](10, 7) NULL
	,[Longitude] [decimal](10, 7) NULL
	)

DROP type [dbo].[TAlphaGrade]

CREATE type [TAlphaGrade] AS TABLE (
	[Id] [int] NOT NULL
	,[SchoolRef] [int] NOT NULL
	,[Name] NVARCHAR(5) NOT NULL
	,[Description] NVARCHAR(255) NOT NULL
	)

DROP type [dbo].[TAlternateScore]

CREATE type [TAlternateScore] AS TABLE (
	[Id] [int] NOT NULL
	,[Name] NVARCHAR(3) NOT NULL
	,[Description] NVARCHAR(255) NOT NULL
	,[IncludeInAverage] [bit] NOT NULL
	,[PercentOfMaximumScore] [decimal](6, 2) NULL
)