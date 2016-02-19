Alter Procedure [dbo].[spGetAdminAnnouncements] @id int, @personId int, @roleId int,  @ownedOnly bit
	,@fromDate DateTime2, @toDate DateTime2, @start int, @count int, @now DateTime2
	,@gradeLevelsIds TInt32 Readonly, @complete bit, @studentId int
As

Declare @allCount int;
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


set @allCount = (select COUNT(*) from
		vwAdminAnnouncement	
		left join (select * from AnnouncementRecipientData where PersonRef = @personId) adminAnnData on adminAnnData.AnnouncementRef = vwAdminAnnouncement.Id
	where
		(@id is not null  or [State] = 1) and
		(@id is null or vwAdminAnnouncement.Id = @id)
		and (@ownedOnly = 0 or vwAdminAnnouncement.AdminRef = @personId)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@complete is null or adminAnnData.Complete = @complete or (@complete = 0 and adminAnnData.Complete is null))
		and (Not Exists(select * from @gradeLevelsIds) or  
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
	)

Insert Into @adminAnnouncementT
Select 
	vwAdminAnnouncement.*,
	cast((case vwAdminAnnouncement.AdminRef when @personId then 1 else 0 end) as bit) as IsOwner,
	cast((case when adminAnnData.Complete is null then 0 else adminAnnData.Complete end) as bit) as Complete,
	@allCount as AllCount
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
order by Expires desc				
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

exec spSelectAdminAnnoucnement @adminAnnouncementT

GO




--------------------------------
-- Alter lessonPlan procecdures
---------------------------------
Alter Procedure [dbo].[spGetLessonPlans]
@id int, @schoolYearId int, @personId int, @classId int, @roleId int, @ownedOnly bit
,@fromDate DateTime2, @toDate DateTime2, @start int, @count int, @complete bit
, @galleryCategoryId int
As

Declare @lessonPlanT TLessonPlan
Declare @allCount int

set @allCount =
(select COUNT(*) from
vwLessonPlan
left join (select * from AnnouncementRecipientData where PersonRef = @personId) annRecipientData on annRecipientData.AnnouncementRef = vwLessonPlan.Id
where
(@id is not null  or [State] = 1) and
(@id is null or vwLessonPlan.Id = @id)
and (@classId is null or ClassRef = @classId)
and (
(@roleId = 2 and (@ownedOnly = 0 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef)))
or
(@roleId = 3 and exists(select * from ClassPerson where PersonRef = @personId and ClassPerson.ClassRef = vwLessonPlan.ClassRef) and VisibleForStudent = 1)
)
and (@fromDate is null or StartDate >= @fromDate)
and (@toDate is null or StartDate <= @toDate)
and (@complete is null or annRecipientData.Complete = @complete or (@complete = 0 and annRecipientData.Complete is null))
and (@galleryCategoryId is null or GalleryCategoryRef = @galleryCategoryId)
and SchoolYearRef = @schoolYearId

)

Insert into @lessonPlanT
Select
vwLessonPlan.*,
cast((case when (select count(*) from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef) >= 1 then 1 else 0 end) as bit)  as IsOwner,
cast((case when annRecipientData.Complete is null then 0 else annRecipientData.Complete end) as bit) as Complete,
@allCount as AllCount
from
vwLessonPlan
left join (select * from AnnouncementRecipientData where PersonRef = @personId) annRecipientData on annRecipientData.AnnouncementRef = vwLessonPlan.Id
where
(@id is not null  or [State] = 1) and
(@id is null or vwLessonPlan.Id = @id)
and (@classId is null or ClassRef = @classId)
and (
(@roleId = 2 and (@ownedOnly = 0 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef)))
or
(@roleId = 3 and exists(select * from ClassPerson where PersonRef = @personId and ClassPerson.ClassRef = vwLessonPlan.ClassRef) and VisibleForStudent = 1)
)
and (@fromDate is null or StartDate >= @fromDate)
and (@toDate is null or StartDate <= @toDate)
and (@complete is null or annRecipientData.Complete = @complete or (@complete = 0 and annRecipientData.Complete is null))
and (@galleryCategoryId is null or GalleryCategoryRef = @galleryCategoryId)
and SchoolYearRef = @schoolYearId

order by StartDate desc
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY


exec spSelectLessonPlans @lessonPlanT
GO


