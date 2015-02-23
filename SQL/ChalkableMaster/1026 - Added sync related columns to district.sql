Alter Table District
	Add SyncFrequency int
Alter Table District
	Add MaxSyncFrequency int
Alter Table District	
	Add FailCounter int
Alter Table District	
	Add FailDelta int
Go
Update District
	Set FailCounter = 0, FailDelta = 0
Go
Alter Table District
	Alter Column FailCounter int not null

Alter Table District
	Alter Column FailDelta int not null
Go

Drop Type TDistrict

CREATE TYPE [dbo].[TDistrict] AS TABLE(
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
	SyncFrequency int,
	MaxSyncFrequency int,
	FailCounter int not null,
	FailDelta int not null
)
GO
