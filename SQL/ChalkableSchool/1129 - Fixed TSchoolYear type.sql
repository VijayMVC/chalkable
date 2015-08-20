Drop Type [TSchoolYear]

CREATE TYPE [dbo].[TSchoolYear] AS TABLE(
	[Id] [int] NOT NULL,	
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[StartDate] [datetime2](7) NULL,
	[EndDate] [datetime2](7) NULL,
	[SchoolRef] [int] NOT NULL
)
GO


