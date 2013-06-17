Create Table BackgroundTask
(
	Id uniqueidentifier not null primary key,
	SchoolRef uniqueidentifier constraint FK_BackgroundTask_School foreign key references School(Id),
	[Type] int not null,
	[State] int not null,
	Created DateTime2 not null,
	Scheduled DateTime2 not null,
	[Started] DateTime2,
	Data nvarchar(2048)
)
GO

Create procedure spGetBackgroundTaskForProcessing
	@currentTime DateTime2
as
declare @id uniqueidentifier = null;
set @id = 
(select top 1 Id from 
BackgroundTask bt
where 
	Scheduled >= @currentTime
	and [State] = 0
	and not exists (select * from BackgroundTask where SchoolRef = bt.SchoolRef and [State] = 1)
order by
	Scheduled
);
update BackgroundTask set [State] = 1 where Id = @id;
select * from BackgroundTask where Id = @id
GO