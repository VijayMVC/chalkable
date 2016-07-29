CREATE View [dbo].[vwDistrictSyncStatus]
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
		Cl.CanceledCompleted,
		Nw.NewId,
		Nw.NewCreated
	from 
		District
		left join 
		(select top 1 * from 
			(select
				DistrictRef,
				Id as ProcessingId,
				[Started] as ProcessingStarted,
				Created as ProcessingCreated,
				Scheduled as ProcessingScheduled,
				Rank() over(partition by districtref order by started desc) R
			from 
			BackgroundTask 
			where 
				state = 1 and type = 1
			) X
		where
			R = 1
		Order By IIF(ProcessingStarted is null,ProcessingCreated,ProcessingStarted) desc
		) Pr on District.Id = Pr.DistrictRef
		left join 
		(select top 1 * from 
			(select
				DistrictRef,
				Id as CompletedId,
				Started as CompletedStarted,
				Created as CompletedCreated,
				Completed as CompletedCompleted,
				Scheduled as CompletedScheduled,
				Rank() over(partition by districtref order by started desc) R
			from 
			BackgroundTask 
			where 
				state = 2 and type = 1
			) X
		where
			R = 1
		Order By IIF(CompletedStarted is null, CompletedCreated, CompletedStarted) desc
		) Cp on District.Id = Cp.DistrictRef
		left join 
		(select top 1 * from 
			(select
				DistrictRef,
				Id as FailedId,
				Started as FailedStarted,
				Created as FailedCreated,
				Completed as FailedCompleted,
				Scheduled as FailedScheduled,
				Rank() over(partition by districtref order by started desc) R
			from 
			BackgroundTask 
			where 
				state = 4 and type = 1
			) X
		where
			R = 1
		Order By IIF(FailedStarted is null, FailedCreated, FailedStarted) desc
		) Fl on District.Id = Fl.DistrictRef
		left join 
		(select top 1 * from 
			(select
				DistrictRef,
				Id as CanceledId,
				Started as CanceledStarted,
				Created as CanceledCreated,
				Completed as CanceledCompleted,
				Scheduled as CanceledScheduled,
				Rank() over(partition by districtref order by started desc) R
			from 
			BackgroundTask 
			where 
				state = 3 and type = 1
			) X
		where
			R = 1
		Order By IIF(CanceledStarted is null, CanceledCreated, CanceledStarted) desc
		) Cl on District.Id = Cl.DistrictRef
		left join 
		(select top 1 * from 
			(select
				DistrictRef,
				Id as NewId,
				Created as NewCreated,
				Scheduled as NewScheduled,
				Rank() over(partition by districtref order by created desc) R
			from 
			BackgroundTask 
			where 
				state = 0 and type = 1
			) X
		where
			R = 1
		Order By NewCreated desc
		) Nw on District.Id = Nw.DistrictRef
GO