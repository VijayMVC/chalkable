CREATE TYPE [dbo].[TCommonCoreStandard] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[ParentStandardRef] [uniqueidentifier] NULL,
	[StandardCategoryRef] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](255) NULL,
	[DEscription] [nvarchar](max) NULL
)
GO

CREATE TYPE [dbo].[TABToCCMapping] AS TABLE(
	[CCStandardRef] [uniqueidentifier] NOT NULL,
	[AcademicBenchmarkId] [uniqueidentifier] NOT NULL
)
GO


