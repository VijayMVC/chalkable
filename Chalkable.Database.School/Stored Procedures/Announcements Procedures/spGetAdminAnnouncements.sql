CREATE Procedure [dbo].[spGetAdminAnnouncements] 
	@id int, 
	@personId int,
	@roleId int,
	@ownedOnly bit,
	@fromDate DateTime2,
	@toDate DateTime2,
	@now DateTime2,
	@gradeLevelsIds TInt32 Readonly, 
	@complete bit, 
	@studentId int
As

Declare @gradeLevelsIdsT table(value int);
Declare @adminAnnouncementT TAdminAnnouncement

If Exists(Select * From @gradeLevelsIds)
Begin
	Insert Into 
		@gradeLevelsIdsT(value)
	Select 
		value 
	From 
		@gradeLevelsIds
End

If @roleId = 3 and (@studentId is null or @studentId = @personId) 
	Set @studentId = @personId

Select 
	vwAdminAnnouncement.*,
	cast((case vwAdminAnnouncement.AdminRef when @personId then 1 else 0 end) as bit) as IsOwner,
	cast((case when adminAnnData.Complete is null then 0 else adminAnnData.Complete end) as bit) as Complete,
	count(vwAdminAnnouncement.Id) over()
from 
	vwAdminAnnouncement	
	left join (select * from AnnouncementRecipientData where PersonRef = @personId) adminAnnData on adminAnnData.AnnouncementRef = vwAdminAnnouncement.Id
where
	(@id is not null or [State] = 1) and
	(@id is null or vwAdminAnnouncement.Id = @id)
	and (@ownedOnly = 0 or vwAdminAnnouncement.AdminRef = @personId)
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@complete is null or adminAnnData.Complete = @complete or (@complete = 0 and adminAnnData.Complete is null))
	and (Not Exists(Select * from @gradeLevelsIds) or  
				exists
				(
					select * from AnnouncementGroup ar
					join StudentGroup on StudentGroup.GroupRef = ar.GroupRef
					join StudentSchoolYear on StudentSchoolYear.StudentRef = StudentGroup.StudentRef
					join SchoolYear on SchoolYear.Id = StudentSchoolYear.SchoolYearRef
					join @gradeLevelsIdsT gl on gl.value = StudentSchoolYear.GradeLevelRef
					where ar.AnnouncementRef = vwAdminAnnouncement.Id and @now between SchoolYear.StartDate and SchoolYear.EndDate
				)
		 )
	and (@studentId is null or 				
			exists
			(
				select * from AnnouncementGroup ar
				join StudentGroup on StudentGroup.GroupRef = ar.GroupRef
				where StudentGroup.StudentRef = @studentId and AnnouncementRef = vwAdminAnnouncement.Id
			)
		)



