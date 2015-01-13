Alter Table MarkingPeriod
	Drop FK_MarkingPeriod_School
Alter Table MarkingPeriod
	Drop Column SchoolRef
Alter Table MarkingPeriodClass
	Drop FK_MarkingPeriodClass_School
Alter Table MarkingPeriodClass
	Drop Column SchoolRef
Alter Table ClassPerson
	Drop FK_ClassPerson_School
Alter Table ClassPerson
	Drop Column SchoolRef
GO

DROP TYPE [dbo].[TMarkingPeriod]
GO
CREATE TYPE [dbo].[TMarkingPeriod] AS TABLE(
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[StartDate] [datetime2](7) NULL,
	[EndDate] [datetime2](7) NULL,
	[Description] [nvarchar](1024) NULL,
	[SchoolYearRef] [int] NOT NULL,
	[WeekDays] [int] NOT NULL
)
GO

DROP TYPE [dbo].[TMarkingPeriodClass]
GO

CREATE TYPE [dbo].[TMarkingPeriodClass] AS TABLE(
	[ClassRef] [int] NOT NULL,
	[MarkingPeriodRef] [int] NOT NULL
)
GO





