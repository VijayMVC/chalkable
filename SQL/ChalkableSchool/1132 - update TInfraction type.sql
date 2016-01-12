
DROP TYPE [dbo].[TInfraction]
GO


CREATE TYPE [dbo].[TInfraction] AS TABLE(
	[Id] [int] NOT NULL,
	[Code] [varchar](5) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[Demerits] [tinyint] NOT NULL,
	[StateCode] [varchar](10) NOT NULL,
	[SIFCode] [varchar](10) NOT NULL,
	[NCESCode] [varchar](10) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsSystem] [bit] NOT NULL,
	[VisibleInClassroom] [bit] NOT NULL
)
GO


