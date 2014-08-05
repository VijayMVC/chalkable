CREATE TYPE [dbo].[TAddress] AS TABLE (
	[Id] [int] NOT NULL,
	[AddressNumber] [char](15) NOT NULL,
	[StreetNumber] [nvarchar](10) NOT NULL,
	[AddressLine1] [nvarchar](75) NOT NULL,
	[AddressLine2] [nvarchar](75) NOT NULL,
	[City] [nvarchar](50) NOT NULL,
	[State] [nvarchar](5) NOT NULL,
	[PostalCode] [char](10) NOT NULL,
	[Country] [nvarchar](60) NOT NULL,
	[CountyId] [int] NULL,
	[Latitude] [decimal](10, 7) NULL,
	[Longitude] [decimal](10, 7) NULL
)

CREATE TYPE [dbo].[TAlphaGrade] AS TABLE (
	[Id] [int] NOT NULL,
	[SchoolRef] [int] NOT NULL,
	[Name] [varchar](5) NOT NULL,
	[Description] [varchar](255) NOT NULL
)

CREATE TYPE [dbo].[TAlternateScore] AS TABLE (
	[Id] [int] NOT NULL,
	[Name] [varchar](3) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[IncludeInAverage] [bit] NOT NULL,
	[PercentOfMaximumScore] [decimal](6, 2) NULL
)

CREATE TYPE [dbo].[TAnnouncement] AS TABLE (
	[Id] [int] NOT NULL,
	[Content] [nvarchar](max) NULL,
	[Created] [datetime2](7) NOT NULL,
	[Expires] [datetime2](7) NOT NULL,
	[ClassAnnouncementTypeRef] [int] NOT NULL,
	[State] [int] NOT NULL,
	[GradingStyle] [int] NOT NULL,
	[Subject] [nvarchar](255) NULL,
	[ClassRef] [int] NOT NULL,
	[Order] [int] NOT NULL,
	[Dropped] [bit] NOT NULL,
	[SchoolRef] [int] NOT NULL,
	[SisActivityId] [int] NULL,
	[MaxScore] [decimal](18, 0) NULL,
	[WeightAddition] [decimal](18, 0) NULL,
	[WeightMultiplier] [decimal](18, 0) NULL,
	[MayBeDropped] [bit] NOT NULL,
	[Title] [nvarchar](30) NULL,
	[VisibleForStudent] [bit] NOT NULL
)

CREATE TYPE [dbo].[TAnnouncementApplication] AS TABLE (
	[Id] [int] NOT NULL,
	[AnnouncementRef] [int] NOT NULL,
	[ApplicationRef] [uniqueidentifier] NOT NULL,
	[Active] [bit] NOT NULL,
	[Order] [int] NOT NULL
)

CREATE TYPE [dbo].[TAnnouncementAttachment] AS TABLE (
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[PersonRef] [int] NOT NULL,
	[AnnouncementRef] [int] NOT NULL,
	[AttachedDate] [datetime2](7) NOT NULL,
	[Uuid] [nvarchar](255) NULL,
	[Order] [int] NOT NULL,
	[SisAttachmentId] [int] NULL
)

CREATE TYPE [dbo].[TAnnouncementQnA] AS TABLE (
	[id] [int] NOT NULL,
	[AnnouncementRef] [int] NOT NULL,
	[AskerRef] [int] NOT NULL,
	[Question] [nvarchar](max) NOT NULL,
	[Answer] [nvarchar](max) NULL,
	[State] [int] NOT NULL,
	[AnsweredTime] [datetime2](7) NULL,
	[QuestionTime] [datetime2](7) NULL,
	[AnswererRef] [int] NULL
)

CREATE TYPE [dbo].[TAnnouncementRecipient] AS TABLE (
	[Id] [int] NOT NULL,
	[AnnouncementRef] [int] NOT NULL,
	[ToAll] [bit] NOT NULL,
	[RoleRef] [int] NULL,
	[GradeLevelRef] [int] NULL,
	[PersonRef] [int] NULL
)

CREATE TYPE [dbo].[TAnnouncementStandard] AS TABLE (
	[StandardRef] [int] NOT NULL,
	[AnnouncementRef] [int] NOT NULL
 )

CREATE TYPE [dbo].[TApplicationInstall] AS TABLE (
	[Id] [int] NOT NULL,
	[ApplicationRef] [uniqueidentifier] NOT NULL,
	[PersonRef] [int] NOT NULL,
	[InstallDate] [datetime2](7) NOT NULL,
	[SchoolYearRef] [int] NOT NULL,
	[OwnerRef] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[AppInstallActionRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TApplicationInstallAction] AS TABLE (
	[Id] [int] NOT NULL,
	[OwnerRef] [int] NOT NULL,
	[PersonRef] [int] NULL,
	[ApplicationRef] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](max) NULL
)

CREATE TYPE [dbo].[TApplicationInstallActionClasses] AS TABLE (
	[Id] [int] NOT NULL,
	[ClassRef] [int] NOT NULL,
	[AppInstallActionRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TApplicationInstallActionDepartment] AS TABLE (
	[Id] [int] NOT NULL,
	[DepartmentRef] [uniqueidentifier] NOT NULL,
	[AppInstallActionRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TApplicationInstallActionGradeLevel] AS TABLE (
	[Id] [int] NOT NULL,
	[GradeLevelRef] [int] NOT NULL,
	[AppInstallActionRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TApplicationInstallActionRole] AS TABLE (
	[Id] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[AppInstallActionRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TAttendanceLevelReason] AS TABLE (
	[Id] [int] NOT NULL,
	[Level] [nvarchar](255) NOT NULL,
	[AttendanceReasonRef] [int] NOT NULL,
	[IsDefault] [bit] NOT NULL
)

CREATE TYPE [dbo].[TAttendanceReason] AS TABLE (
	[Id] [int] NOT NULL,
	[Code] [nvarchar](255) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Category] [nvarchar](255) NOT NULL,
	[IsSystem] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL
)

CREATE TYPE [dbo].[TClass] AS TABLE (
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[ChalkableDepartmentRef] [uniqueidentifier] NULL,
	[SchoolYearRef] [int] NULL,
	[PrimaryTeacherRef] [int] NULL,
	[GradeLevelRef] [int] NOT NULL,
	[SchoolRef] [int] NULL,
	[RoomRef] [int] NULL,
	[CourseRef] [int] NULL,
	[GradingScaleRef] [int] NULL,
	[ClassNumber] [nvarchar](41) NULL
)

CREATE TYPE [dbo].[TClassPeriod] AS TABLE (
	[PeriodRef] [int] NOT NULL,
	[DayTypeRef] [int] NOT NULL,
	[ClassRef] [int] NOT NULL,
	[SchoolRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TClassPerson] AS TABLE (
	[ClassRef] [int] NOT NULL,
	[PersonRef] [int] NOT NULL,
	[MarkingPeriodRef] [int] NOT NULL,
	[SchoolRef] [int] NOT NULL,
	[IsEnrolled] [bit] NOT NULL
)

CREATE TYPE [dbo].[TClassroomOption] AS TABLE (
	[Id] [int] NOT NULL,
	[DefaultActivitySortOrder] [nvarchar](1) NOT NULL,
	[GroupByCategory] [bit] NOT NULL,
	[AveragingMethod] [nvarchar](1) NOT NULL,
	[CategoryAveraging] [bit] NOT NULL,
	[IncludeWithdrawnStudents] [bit] NOT NULL,
	[DisplayStudentAverage] [bit] NOT NULL,
	[DisplayTotalPoints] [bit] NOT NULL,
	[RoundDisplayedAverages] [bit] NOT NULL,
	[DisplayAlphaGrade] [bit] NOT NULL,
	[DisplayStudentNames] [bit] NOT NULL,
	[DisplayMaximumScore] [bit] NOT NULL,
	[StandardsGradingScaleRef] [int] NULL,
	[StandardsCalculationMethod] [nvarchar](1) NOT NULL,
	[StandardsCalculationRule] [nvarchar](1) NOT NULL,
	[StandardsCalculationWeightMaximumValues] [bit] NOT NULL,
	[DefaultStudentSortOrder] [nvarchar](1) NOT NULL,
	[SeatingChartRows] [int] NOT NULL,
	[SeatingChartColumns] [int] NOT NULL
)

CREATE TYPE [dbo].[TClassStandard] AS TABLE (
	[ClassRef] [int] NOT NULL,
	[StandardRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TClassTeacher] AS TABLE (
	[PersonRef] [int] NOT NULL,
	[ClassRef] [int] NOT NULL,
	[IsHighlyQualified] [bit] NOT NULL,
	[IsCertified] [bit] NOT NULL,
	[IsPrimary] [bit] NOT NULL
)

CREATE TYPE [dbo].[TDate] AS TABLE (
	[Day] [datetime2](7) NOT NULL,
	[DayTypeRef] [int] NULL,
	[SchoolYearRef] [int] NOT NULL,
	[IsSchoolDay] [bit] NOT NULL,
	[SchoolRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TDayType] AS TABLE (
	[Id] [int] NOT NULL,
	[Number] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[SchoolYearRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TGradeLevel] AS TABLE (
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[Number] [int] NOT NULL
)

CREATE TYPE [dbo].[TGradingComment] AS TABLE (
	[Id] [int] NOT NULL,
	[SchoolRef] [int] NOT NULL,
	[Code] [varchar](5) NOT NULL,
	[Comment] [varchar](50) NOT NULL
)

CREATE TYPE [dbo].[TGradingPeriod] AS TABLE (
	[Id] [int] NOT NULL,
	[Code] [nvarchar](5) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[MarkingPeriodRef] [int] NOT NULL,
	[SchoolYearRef] [int] NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[EndTime] [datetime2](7) NOT NULL,
	[SchoolAnnouncement] [nvarchar](255) NULL,
	[AllowGradePosting] [bit] NOT NULL
)

CREATE TYPE [dbo].[TGradingScale] AS TABLE (
	[Id] [int] NOT NULL,
	[SchoolRef] [int] NOT NULL,
	[Name] [varchar](20) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[HomeGradeToDisplay] [int] NULL
)

CREATE TYPE [dbo].[TGradingScaleRange] AS TABLE (
	[GradingScaleRef] [int] NOT NULL,
	[AlphaGradeRef] [int] NOT NULL,
	[LowValue] [decimal](9, 6) NOT NULL,
	[HighValue] [decimal](9, 6) NOT NULL,
	[AveragingEquivalent] [int] NOT NULL,
	[AwardGradCredit] [bit] NOT NULL,
	[IsPassing] [bit] NOT NULL
)

CREATE TYPE [dbo].[TGradingStyle] AS TABLE (
	[Id] [int] NOT NULL,
	[GradingStyleValue] [int] NOT NULL,
	[MaxValue] [int] NOT NULL,
	[StyledValue] [int] NOT NULL
)

CREATE TYPE [dbo].[TInfraction] AS TABLE (
	[Id] [int] NOT NULL,
	[Code] [varchar](5) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[Demerits] [tinyint] NOT NULL,
	[StateCode] [varchar](10) NOT NULL,
	[SIFCode] [varchar](10) NOT NULL,
	[NCESCode] [varchar](10) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsSystem] [bit] NOT NULL
)

CREATE TYPE [dbo].[TMarkingPeriod] AS TABLE (
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[StartDate] [datetime2](7) NULL,
	[EndDate] [datetime2](7) NULL,
	[Description] [nvarchar](1024) NULL,
	[SchoolYearRef] [int] NOT NULL,
	[WeekDays] [int] NOT NULL,
	[SchoolRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TMarkingPeriodClass] AS TABLE (
	[ClassRef] [int] NOT NULL,
	[MarkingPeriodRef] [int] NOT NULL,
	[SchoolRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TNotification] AS TABLE (
	[Id] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Message] [nvarchar](1024) NULL,
	[Shown] [bit] NOT NULL,
	[PersonRef] [int] NOT NULL,
	[AnnouncementRef] [int] NULL,
	[PrivateMessageRef] [int] NULL,
	[ApplicationRef] [uniqueidentifier] NULL,
	[QuestionPersonRef] [int] NULL,
	[Created] [datetime2](7) NOT NULL,
	[MarkingPeriodRef] [int] NULL,
	[WasSend] [bit] NOT NULL
)

CREATE TYPE [dbo].[TPeriod] AS TABLE (
	[Id] [int] NOT NULL,
	[StartTime] [int] NOT NULL,
	[EndTime] [int] NOT NULL,
	[SchoolYearRef] [int] NOT NULL,
	[Order] [int] NOT NULL,
	[SchoolRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TPerson] AS TABLE (
	[Id] [int] NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[BirthDate] [datetime2](7) NULL,
	[Gender] [nvarchar](255) NULL,
	[Salutation] [nvarchar](255) NULL,
	[Active] [bit] NOT NULL,
	[LastPasswordReset] [datetime2](7) NULL,
	[FirstLoginDate] [datetime2](7) NULL,
	[LastMailNotification] [datetime2](7) NULL,
	[Email] [nvarchar](256) NOT NULL,
	[AddressRef] [int] NULL,
	[HasMedicalAlert] [bit] NOT NULL,
	[IsAllowedInetAccess] [bit] NOT NULL,
	[SpecialInstructions] [nvarchar](1024) NOT NULL,
	[SpEdStatus] [nvarchar](256) NULL,
	[PhotoModifiedDate] [datetime2](7) NULL
)

CREATE TYPE [dbo].[TPhone] AS TABLE (
	[PersonRef] [int] NOT NULL,
	[Value] [nvarchar](256) NOT NULL,
	[Type] [int] NOT NULL,
	[IsPrimary] [bit] NOT NULL,
	[DigitOnlyValue] [nvarchar](256) NOT NULL
)

CREATE TYPE [dbo].[TPrivateMessage] AS TABLE (
	[Id] [int] NOT NULL,
	[FromPersonRef] [int] NOT NULL,
	[ToPersonRef] [int] NOT NULL,
	[Sent] [datetime2](7) NULL,
	[Subject] [nvarchar](1024) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[Read] [bit] NOT NULL,
	[DeletedBySender] [bit] NOT NULL,
	[DeletedByRecipient] [bit] NOT NULL
)

CREATE TYPE [dbo].[TReportDownload] AS TABLE (
	[Id] [int] NOT NULL,
	[Format] [int] NOT NULL,
	[PersonRef] [int] NOT NULL,
	[ReportType] [int] NOT NULL,
	[DownloadDate] [datetime2](7) NOT NULL,
	[FriendlyName] [nvarchar](1024) NOT NULL
)

CREATE TYPE [dbo].[TReportMailDelivery] AS TABLE (
	[Id] [int] NOT NULL,
	[ReportType] [int] NOT NULL,
	[Format] [int] NOT NULL,
	[Frequency] [int] NOT NULL,
	[PersonRef] [int] NOT NULL,
	[SendHour] [int] NULL,
	[SendDay] [int] NULL,
	[LastSentMarkingPeriodRef] [int] NULL,
	[LastSentTime] [datetime2](7) NULL
)

CREATE TYPE [dbo].[TRoom] AS TABLE (
	[Id] [int] NOT NULL,
	[RoomNumber] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[Size] [nvarchar](255) NULL,
	[Capacity] [int] NULL,
	[PhoneNumber] [nvarchar](255) NULL,
	[SchoolRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TSchool] AS TABLE (
	[Id] [int] NOT NULL,
	[Name] [nvarchar](1024) NULL,
	[IsActive] [bit] NOT NULL,
	[IsPrivate] [bit] NOT NULL
)

CREATE TYPE [dbo].[TSchoolGradeLevel] AS TABLE (
	[SchoolRef] [int] NOT NULL,
	[GradeLevelRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TSchoolOption] AS TABLE (
	[Id] [int] NOT NULL,
	[AllowSectionAverageModification] [bit] NOT NULL,
	[EarliestPaymentDate] [datetime2](7) NULL,
	[NextReceiptNumber] [int] NULL,
	[DefaultCombinationIndex] [int] NULL,
	[TimeZoneName] [varchar](32) NULL,
	[BaseHoursOffset] [int] NULL,
	[BaseMinutesOffset] [int] NULL,
	[ObservesDst] [bit] NULL,
	[AllowScoreEntryForUnexcused] [bit] NOT NULL,
	[DisciplineOverwritesAttendance] [bit] NOT NULL,
	[AllowDualEnrollment] [bit] NOT NULL,
	[CompleteStudentScheduleDefinition] [nvarchar](1) NOT NULL,
	[AveragingMethod] [nvarchar](1) NOT NULL,
	[CategoryAveraging] [bit] NOT NULL,
	[StandardsGradingScaleRef] [int] NULL,
	[StandardsCalculationMethod] [nvarchar](1) NOT NULL,
	[StandardsCalculationRule] [nvarchar](1) NOT NULL,
	[StandardsCalculationWeightMaximumValues] [bit] NOT NULL,
	[LockCategories] [bit] NOT NULL,
	[IncludeReportCardCommentsInGradebook] [bit] NOT NULL,
	[MergeRostersForAttendance] [bit] NOT NULL
)

CREATE TYPE [dbo].[TSchoolPerson] AS TABLE (
	[SchoolRef] [int] NOT NULL,
	[PersonRef] [int] NOT NULL,
	[RoleRef] [int] NOT NULL
)

CREATE TYPE [dbo].[TSchoolYear] AS TABLE (
	[Id] [int] NOT NULL,
	[SchoolRef] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[StartDate] [datetime2](7) NULL,
	[EndDate] [datetime2](7) NULL
)

CREATE TYPE [dbo].[TSisUser] AS TABLE (
	[Id] [int] NOT NULL,
	[UserName] [nvarchar](127) NOT NULL,
	[LockedOut] [bit] NOT NULL,
	[IsDisabled] [bit] NOT NULL,
	[IsSystem] [bit] NOT NULL
)

CREATE TYPE [dbo].[TStandard] AS TABLE (
	[Id] [int] NOT NULL,
	[ParentStandardRef] [int] NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[StandardSubjectRef] [int] NOT NULL,
	[LowerGradeLevelRef] [int] NULL,
	[UpperGradeLevelRef] [int] NULL,
	[IsActive] [bit] NOT NULL
)

CREATE TYPE [dbo].[TStandardSubject] AS TABLE (
	[Id] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](200) NOT NULL,
	[AdoptionYear] [int] NULL,
	[IsActive] [bit] NOT NULL
)

CREATE TYPE [dbo].[TStudentSchoolYear] AS TABLE (
	[SchoolYearRef] [int] NOT NULL,
	[GradeLevelRef] [int] NOT NULL,
	[StudentRef] [int] NOT NULL,
	[EnrollmentStatus] [int] NOT NULL
)

CREATE TYPE [dbo].[TSyncVersion] AS TABLE (
	[TableName] [nvarchar](256) NULL,
	[Version] [int] NULL
) 
