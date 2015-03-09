Alter Table BackgroundTask 
	Add Domain nvarchar(256)
GO
Update BackgroundTask 
	Set Domain = isnull(cast (DistrictRef as nvarchar(256)), '')
Alter Table BackgroundTask 
	Alter Column Domain nvarchar(256) not null
GO
Create Index IX_BackgroundTas_Domain on BackgroundTask(Domain) 
GO

Alter Procedure spGetBackgroundTaskForProcessing
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

GO