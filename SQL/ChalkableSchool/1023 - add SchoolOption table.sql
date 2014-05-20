CREATE TABLE [dbo].[SchoolOption]
(
	[Id] [int] NOT NULL primary key constraint FK_SchoolOption_School foreign key references School(Id),
	[AllowSectionAverageModification] [bit] NOT NULL,
	[EarliestPaymentDate] datetime2 NULL,
	[NextReceiptNumber] [int] NULL,
	[DefaultCombinationIndex] [int] NULL,
	[TimeZoneName] [varchar](32) NULL,
	[BaseHoursOffset] [int] NULL,
	[BaseMinutesOffset] [int] NULL,
	[ObservesDst] [bit] NULL,
	[AllowScoreEntryForUnexcused] [bit] NOT NULL,
	[DisciplineOverwritesAttendance] [bit] NOT NULL,
	[AllowDualEnrollment] [bit] NOT NULL,
	[CompleteStudentScheduleDefinition] nvarchar(1) NOT NULL,
	[AveragingMethod] nvarchar(1) NOT NULL,
	[CategoryAveraging] [bit] NOT NULL,
	[StandardsGradingScaleRef] [int] NULL constraint FK_SchoolOption_GradinScale foreign key references GradingScale(Id),
	[StandardsCalculationMethod] nvarchar(1) NOT NULL,
	[StandardsCalculationRule] nvarchar(1) NOT NULL,
	[StandardsCalculationWeightMaximumValues] [bit] NOT NULL,
	[LockCategories] [bit] NOT NULL,
	[IncludeReportCardCommentsInGradebook] [bit] NOT NULL,
	[MergeRostersForAttendance] [bit] NOT NULL,
)
go





