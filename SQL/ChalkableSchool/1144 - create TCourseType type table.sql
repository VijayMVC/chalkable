create type TCourseType as table
(
	[Id] [int] NOT NULL,
	[Code] [nvarchar](5) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[StateCode] [nvarchar](10) NOT NULL,
	[SIFCode] [nvarchar](10) NOT NULL,
	[NCESCode] [nvarchar](10) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsSystem] [bit] NOT NULL
)
Go