alter table Standard
add CCStandardCode nvarchar(255) null
go
Drop TYPE [dbo].[TStandard] 
GO
CREATE TYPE [dbo].[TStandard] AS TABLE(
	[Id] [int] NOT NULL,
	[ParentStandardRef] [int] NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[StandardSubjectRef] [int] NOT NULL,
	[LowerGradeLevelRef] [int] NULL,
	[UpperGradeLevelRef] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[CCStandardCode] [nvarchar](255) NULL
)
GO


