ALter Table District
	Add MaxSyncTime int
GO
ALter Table District
	Add SyncLogFlushSize int
GO
ALter Table District
	Add SyncHistoryDays int
GO
Update District	Set
	MaxSyncTime = 1800,
	SyncLogFlushSize = 100,
	SyncHistoryDays = 15
GO
ALter Table District
	Alter Column MaxSyncTime int not null
GO
ALter Table District
	Alter Column SyncLogFlushSize int not null
GO
ALter Table District
	Alter Column SyncHistoryDays int not null
GO

Drop Type TDistrict
GO
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
	MaxSyncTime int not null,
	SyncLogFlushSize int not null,
	SyncHistoryDays int not null
) 
GO

Alter View vwDistrictSyncStatus 
as
	select 
		District.*,
		Pr.ProcessingId,
		Pr.ProcessingCreated,
		Pr.ProcessingStarted,
		Cp.CompletedId,
		Cp.CompletedCreated,
		Cp.CompletedStarted,
		Cp.CompletedCompleted,
		Fl.FailedId,
		Fl.FailedCreated,
		Fl.FailedStarted,
		Fl.FailedCompleted,
		Cl.CanceledId,
		Cl.CanceledCreated,
		Cl.CanceledStarted,
		Cl.CanceledCompleted
	from 
		District
		left join 
		(select * from 
			(select
				DistrictRef,
				Id as ProcessingId,
				Started as ProcessingStarted,
				Created as ProcessingCreated,
				Rank() over(partition by districtref order by started desc) R
			from 
			BackgroundTask 
			where 
				state = 1
			) X
		where
			R = 1
		) Pr on District.Id = Pr.DistrictRef
		left join 
		(select * from 
			(select
				DistrictRef,
				Id as CompletedId,
				Started as CompletedStarted,
				Created as CompletedCreated,
				Completed as CompletedCompleted,
				Rank() over(partition by districtref order by started desc) R
			from 
			BackgroundTask 
			where 
				state = 2
			) X
		where
			R = 1
		) Cp on District.Id = Cp.DistrictRef
		left join 
		(select * from 
			(select
				DistrictRef,
				Id as FailedId,
				Started as FailedStarted,
				Created as FailedCreated,
				Completed as FailedCompleted,
				Rank() over(partition by districtref order by started desc) R
			from 
			BackgroundTask 
			where 
				state = 4
			) X
		where
			R = 1
		) Fl on District.Id = Fl.DistrictRef
		left join 
		(select * from 
			(select
				DistrictRef,
				Id as CanceledId,
				Started as CanceledStarted,
				Created as CanceledCreated,
				Completed as CanceledCompleted,
				Rank() over(partition by districtref order by started desc) R
			from 
			BackgroundTask 
			where 
				state = 3
			) X
		where
			R = 1
		) Cl on District.Id = Cl.DistrictRef

GO
