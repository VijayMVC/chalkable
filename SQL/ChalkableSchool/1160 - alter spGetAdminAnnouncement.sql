ALTER Procedure [dbo].[spGetAdminAnnouncements]  
	@id int, @personId int, @roleId int,  @ownedOnly bit
	,@fromDate DateTime2, @toDate DateTime2, @start int, @count int, @now DateTime2
	,@gradeLevelsIds nvarchar(256), @complete bit, @studentId int
as 

declare @allCount int;
declare @gradeLevelsIdsT table(value int);
if(@gradeLevelsIds is not null)
begin
	insert into @gradeLevelsIdsT(value)
	select cast(s as int) from dbo.split(',', @gradeLevelsIds)
end

set @allCount = (select COUNT(*) from
	vwAnnouncement	
    left join (select * from AdminAnnouncementData where PersonRef = @personId) adminAnnData on adminAnnData.AnnouncementRef = vwAnnouncement.Id
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@ownedOnly = 0 or vwAnnouncement.AdminRef = @personId)
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@complete is null or adminAnnData.Complete = @complete or (@complete = 0 and adminAnnData.Complete is null))
	and (@gradeLevelsIds is null or  
			exists
			(
				select * from AdminAnnouncementRecipient ar
				join StudentGroup on StudentGroup.GroupRef = ar.GroupRef
				join StudentSchoolYear on StudentSchoolYear.StudentRef = StudentGroup.StudentRef
				join SchoolYear on SchoolYear.Id = StudentSchoolYear.SchoolYearRef
				join @gradeLevelsIdsT gl on gl.value = StudentSchoolYear.GradeLevelRef
				where ar.AnnouncementRef = vwAnnouncement.Id and @now between SchoolYear.StartDate and SchoolYear.EndDate
			)
		)
	and (@studentId is null or 				
			exists
			(
				select * from AdminAnnouncementRecipient ar
				join StudentGroup on StudentGroup.GroupRef = ar.GroupRef
				where StudentGroup.StudentRef = @studentId
			)
		)
)

Select 
	vwAnnouncement.*,
	cast((case vwAnnouncement.AdminRef when @personId then 1 else 0 end) as bit) as IsOwner,
	cast((case when adminAnnData.Complete is null then 0 else adminAnnData.Complete end) as bit) as Complete,
	ROW_NUMBER() OVER(ORDER BY vwAnnouncement.Created desc) as RowNumber,
	@allCount as AllCount
from 
	vwAnnouncement	
	left join (select * from AdminAnnouncementData where PersonRef = @personId) adminAnnData on adminAnnData.AnnouncementRef = vwAnnouncement.Id
where
	(@id is not null or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@ownedOnly = 0 or vwAnnouncement.AdminRef = @personId)
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@complete is null or adminAnnData.Complete = @complete or (@complete = 0 and adminAnnData.Complete is null))
	and (@gradeLevelsIds is null or  
				exists
				(
					select * from AdminAnnouncementRecipient ar
					join StudentGroup on StudentGroup.GroupRef = ar.GroupRef
					join StudentSchoolYear on StudentSchoolYear.StudentRef = StudentGroup.StudentRef
					join SchoolYear on SchoolYear.Id = StudentSchoolYear.SchoolYearRef
					join @gradeLevelsIdsT gl on gl.value = StudentSchoolYear.GradeLevelRef
					where ar.AnnouncementRef = vwAnnouncement.Id and @now between SchoolYear.StartDate and SchoolYear.EndDate
				)
		 )
	and (@studentId is null or 				
			exists
			(
				select * from AdminAnnouncementRecipient ar
				join StudentGroup on StudentGroup.GroupRef = ar.GroupRef
				where StudentGroup.StudentRef = @studentId
			)
		)
order by Created desc				
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
GO


