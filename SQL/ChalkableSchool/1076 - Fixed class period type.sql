DROP TYPE [dbo].[TClassPeriod]
GO
CREATE TYPE [dbo].[TClassPeriod] AS TABLE(
	[PeriodRef] [int] NOT NULL,
	[DayTypeRef] [int] NOT NULL,
	[ClassRef] [int] NOT NULL
)
GO


