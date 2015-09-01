Alter Table Standard
	Drop Column CCStandardCode
GO
Alter Table Standard
	Add AcademicBenchmarkId uniqueidentifier
GO
Drop Type TStandard
GO
Create Type TStandard as Table(
	[Id] [int] NOT NULL,
	[ParentStandardRef] [int] NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[StandardSubjectRef] [int] NOT NULL,
	[LowerGradeLevelRef] [int] NULL,
	[UpperGradeLevelRef] [int] NULL,
	[IsActive] [bit] NOT NULL,
	AcademicBenchmarkId uniqueidentifier
)
GO

	