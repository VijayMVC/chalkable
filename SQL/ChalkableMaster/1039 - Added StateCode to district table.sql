Alter Table District
	Add StateCode nvarchar(2)
GO
Drop Type [TDistrict]
GO
Create Type [dbo].[TDistrict] as Table(
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[SisUrl] [nvarchar](1024) NULL,
	[SisUserName] [nvarchar](1024) NULL,
	[SisPassword] [nvarchar](1024) NULL,
	[Status] [int] NOT NULL,
	[TimeZone] [nvarchar](1024) NOT NULL,
	[ServerUrl] [nvarchar](256) NOT NULL,
	[SisRedirectUrl] [nvarchar](1024) NULL,
	[LastSync] [datetime2](7) NULL,
	[MaxSyncTime] [int] NOT NULL,
	[SyncLogFlushSize] [int] NOT NULL,
	[SyncHistoryDays] [int] NOT NULL,
	[SyncFrequency] [int] NULL,
	[MaxSyncFrequency] [int] NULL,
	[FailCounter] [int] NOT NULL,
	[FailDelta] [int] NOT NULL,
	StateCode nvarchar(2)
)
GO