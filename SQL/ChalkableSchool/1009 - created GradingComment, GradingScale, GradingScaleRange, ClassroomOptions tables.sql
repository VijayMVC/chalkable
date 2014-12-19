CREATE TABLE [dbo].[GradingComment](
	[Id] [int] NOT NULL primary key,
	[SchoolRef] [int] NOT NULL constraint FK_GradingComment_School foreign key references School(Id),
	[Code] [varchar](5) NOT NULL,
	[Comment] [varchar](50) NOT NULL
)
go

alter table GradingComment
add constraint UQ_GradingComment_SchoolRef_Code unique (SchoolRef, Code)
GO


CREATE TABLE [dbo].[GradingScale](
	[Id] [int] NOT NULL primary key,
	[SchoolRef] [int] NOT NULL constraint FK_GradingScale_School foreign key references School(Id),
	[Name] [varchar](20) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[HomeGradeToDisplay] [int] NULL
)
go

alter table GradingScale
add constraint UQ_GradingScale_SchoolRef_Name unique (SchoolRef, Name)
go

CREATE TABLE [dbo].[GradingScaleRange](
	[GradingScaleRef] [int] NOT NULL constraint FK_GradingScaleRange_GradingScale foreign key references GradingScale(Id),
	[AlphaGradeRef] [int] NOT NULL constraint FK_GradingScaleRange_Alphagrade foreign key references AlphaGrade(Id),
	[LowValue] [decimal](9, 6) NOT NULL,
	[HighValue] [decimal](9, 6) NOT NULL,
	[AveragingEquivalent] [int] NOT NULL,
	[AwardGradCredit] [bit] NOT NULL,
	[IsPassing] [bit] NOT NULL,
	CONSTRAINT [PK_GradingScaleRange] PRIMARY KEY ([GradingScaleRef], [AlphaGradeRef])
)
GO

CREATE TABLE [dbo].[ClassroomOption](
	[Id] [int] NOT NULL primary key constraint FK_ClassroomOption_Class foreign key references Class(Id),
	[DefaultActivitySortOrder] nvarchar(1) NOT NULL,
	[GroupByCategory] [bit] NOT NULL,
	[AveragingMethod] nvarchar(1) NOT NULL,
	[CategoryAveraging] [bit] NOT NULL,
	[IncludeWithdrawnStudents] [bit] NOT NULL,
	[DisplayStudentAverage] [bit] NOT NULL,
	[DisplayTotalPoints] [bit] NOT NULL,
	[RoundDisplayedAverages] [bit] NOT NULL,
	[DisplayAlphaGrade] [bit] NOT NULL,
	[DisplayStudentNames] [bit] NOT NULL,
	[DisplayMaximumScore] [bit] NOT NULL,
	[StandardsGradingScaleRef] [int] NULL constraint FK_ClassroomOption_GradingScale foreign key references GradingScale(Id),
	[StandardsCalculationMethod] nvarchar(1) NOT NULL,
	[StandardsCalculationRule] nvarchar(1) NOT NULL,
	[StandardsCalculationWeightMaximumValues] [bit] NOT NULL,
	[DefaultStudentSortOrder] nvarchar(1) NOT NULL,
	[SeatingChartRows] [int] NOT NULL,
	[SeatingChartColumns] [int] NOT NULL
)
GO

