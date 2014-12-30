Create procedure [dbo].[spGetStudentAnnouncements]  
	@id int, @schoolId int, @personId int, @classId int,  @roleId int,  @ownedOnly bit,  @gradedOnly bit
	, @fromDate DateTime2, @toDate DateTime2, @markingPeriodId int
	, @start int, @count int, @now DateTime2, @sisActivitiesIds nvarchar(max)
as 

declare @gradeLevelRef int = (select top 1 GradeLevelRef  
							  from StudentSchoolYear 
							  join SchoolYear on SchoolYear.Id = StudentSchoolYear.SchoolYearRef 
							  where StudentRef = @personId and SchoolYear.StartDate <= @now and SchoolYear.EndDate >= @now)

declare @mpStartDate datetime2, @mpEndDate datetime2
if(@markingPeriodId is not null)
	select @mpStartDate = StartDate, @mpEndDate = EndDate from MarkingPeriod where Id = @markingPeriodId

declare @sisActivitiesIdsT table(Id int)
if(@sisActivitiesIds is not null and LTRIM(@sisActivitiesIds) <> '')
begin
	insert into @sisActivitiesIdsT(Id)
	select cast(s as int) from dbo.split(',', @sisActivitiesIds)
end

declare @allCount int = (select COUNT(*) from
	vwAnnouncement
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))	
	and(@classId is null or ClassRef = @classId)
	and (exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))					
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
)
declare @notExpiredCount int = (select count(*) 
    from 
		vwAnnouncement	
	where
		(@id is not null or Expires >= @now) and
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and(@classId is null or ClassRef = @classId)
		and (exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))				
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
		)

select *
from
	(
	Select 
		vwAnnouncement.*,
		cast(0 as bit) as IsOwner,
		ROW_NUMBER() OVER(ORDER BY  vwAnnouncement.Expires) as RowNumber,
		@allCount as AllCount
	from 
		vwAnnouncement	
	where
		(@id is not null or Expires >= @now) and
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and(@classId is null or ClassRef = @classId)
		and (exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))					
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))

	union 
	Select 
		vwAnnouncement.*,
		cast(0 as bit) as IsOwner,
		(ROW_NUMBER() OVER(ORDER BY  vwAnnouncement.Expires desc)) + @notExpiredCount as RowNumber,
		@allCount as AllCount
	from 
		vwAnnouncement	
	where
		(@id is not null or Expires < @now) and
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and(@classId is null or ClassRef = @classId)
		and (exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))					
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))

	) x
where RowNumber > @start and RowNumber <= @start + @count
order by RowNumber
GO


