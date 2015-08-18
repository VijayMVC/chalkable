Alter procedure [dbo].[spGetTeacherAnnouncements]  
	@id int, @schoolId int, @personId int, @classId int, @roleId int, @ownedOnly bit, @gradedOnly bit
	,@fromDate DateTime2, @toDate DateTime2, @markingPeriodId int, @start int, @count int, @now DateTime2, @allSchoolItems bit
	, @sisActivitiesIds nvarchar(max)
as 

declare @sisActivitiesIdsT table(Id int)
if(@sisActivitiesIds is not null and LTRIM(@sisActivitiesIds) <> '')
begin
	insert into @sisActivitiesIdsT(Id)
	select cast(s as int) from dbo.split(',', @sisActivitiesIds)
end

declare @mpStartDate datetime2, @mpEndDate datetime2
if(@markingPeriodId is not null)
	select @mpStartDate = StartDate, @mpEndDate = EndDate from MarkingPeriod where Id = @markingPeriodId


declare @allCount int;
set @allCount = (select COUNT(*) from
	vwAnnouncement	
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@classId is null or ClassRef = @classId)
	and ((@allSchoolItems = 1 and @roleId = 2) or vwAnnouncement.PrimaryTeacherRef = @personId 
			 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef))
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
	and (@ownedOnly = 0 or vwAnnouncement.PrimaryTeacherRef = @personId 
		 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef))
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
)


Select 
	vwAnnouncement.*,
	--cast((case vwAnnouncement.PrimaryTeacherRef when @personId then 1 else 0 end) as bit) as IsOwner,
	cast((case when (select count(*) from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef) >= 1 then 1 else 0 end) as bit)  as IsOwner,
	ROW_NUMBER() OVER(ORDER BY vwAnnouncement.Created desc) as RowNumber,
 	@allCount as AllCount
from 
	vwAnnouncement	
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@classId is null or ClassRef = @classId)and ((@allSchoolItems = 1 and @roleId = 2) or vwAnnouncement.PrimaryTeacherRef = @personId 
		 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef))			
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
	and (@ownedOnly = 0 or vwAnnouncement.PrimaryTeacherRef = @personId 
		 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef))
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
	
order by Created desc				
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
GO


