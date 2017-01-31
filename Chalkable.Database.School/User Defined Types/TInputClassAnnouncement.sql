CREATE TYPE [dbo].[TInputClassAnnouncement] AS TABLE(
	[Id] [int] NULL,
	[Content] [nvarchar](max) NULL,
	[Created] [datetime2](7) NOT NULL,
	[State] [int] NOT NULL,
	[Title] [nvarchar](max) NULL,
	[DiscussionEnabled]			BIT	 NOT NULL,
	[PreviewCommentsEnabled]	BIT	 NOT NULL,
	[RequireCommentsEnabled]	BIT	 NOT NULL,
	[Expires] [datetime2](7) NOT NULL,
	[ClassAnnouncementTypeRef] [int] NULL,
	[ClassRef] [int] NOT NULL,
	[SchoolYearRef] [int] NULL,
	[Dropped] [bit] NOT NULL,
	[SisActivityId] [int] NULL,
	[MaxScore] [decimal](18, 2) NULL,
	[WeightAddition] [decimal](9, 6) NULL,
	[WeightMultiplier] [decimal](9, 6) NULL,
	[VisibleForStudent] [bit] NULL,
	[MayBeDropped] [bit] NULL,
	[IsScored] [bit] NOT NULL
)
GO