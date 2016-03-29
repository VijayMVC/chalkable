
CREATE Procedure spGetBackgroundTaskForProcessing
	@currentTime DateTime2
as
declare @id uniqueidentifier = null

update 
	BackgroundTask 
set [State] = 1, 
	[Started] = @currentTime,
	@id = Id
where Id in
	(select top 1 Id from 
		BackgroundTask bt
		where 
			Scheduled <= @currentTime
			and [State] = 0
			and not exists (select * from BackgroundTask where Domain = bt.Domain and [State] = 1 )
		order by
			Scheduled
	)


select * from BackgroundTask where Id = @id