Alter Table District
	Drop Column DbName
GO
Alter Table District
	Drop Column SisDistrictId
GO
Create View vwDistrictSyncStatus 
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
		Fl.FailedCompleted
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

Go
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
	[LastSync] [datetime2](7) NULL
) 
GO
