USE [ChalkableMaster]
GO

/****** Object:  StoredProcedure [dbo].[spGetBackgroundTaskForProcessing]    Script Date: 8/15/2014 3:24:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


Alter Procedure [dbo].[spGetBackgroundTaskForProcessing]
	@currentTime DateTime2
as
declare @id uniqueidentifier = null;
set transaction isolation level repeatable read
begin transaction
select top 1 @id =Id from 
BackgroundTask bt with(rowlock,updlock)
where 
	Scheduled <= @currentTime
	and [State] = 0
	and not exists (select * from BackgroundTask where (DistrictRef = bt.DistrictRef or (DistrictRef is null and bt.DistrictRef is null))and [State] = 1 )
order by
	Scheduled

update BackgroundTask set [State] = 1, [Started] = @currentTime where Id = @id;
commit
select * from BackgroundTask where Id = @id
GO


