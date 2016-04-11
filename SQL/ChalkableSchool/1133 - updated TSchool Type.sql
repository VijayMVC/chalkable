
DROP TYPE [dbo].[TSchool]
GO
CREATE TYPE [dbo].[TSchool] AS TABLE(
	[Id] [int] NOT NULL,
	[Name] [nvarchar](1024) NULL,
	[IsActive] [bit] NOT NULL,
	[IsPrivate] [bit] NOT NULL,
	[IsChalkableEnabled] [bit] NOT NULL,
	[IsLEEnabled] [bit] NOT NULL,
	[IsLESyncComplete] [bit] NOT NULL
)
GO

