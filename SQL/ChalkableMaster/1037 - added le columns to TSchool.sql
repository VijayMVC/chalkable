DROP TYPE [dbo].[TSchool]
GO
CREATE TYPE [dbo].[TSchool] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[DistrictRef] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[LocalId] [int] NOT NULL,
	[IsChalkableEnabled] [bit] NOT NULL,
	[IsLEEnabled] [bit] NOT NULL,
	[IsLESyncComplete] [bit] NOT NULL,
	[StudyCenterEnabledTill] [datetime2](7) NULL
)
GO



