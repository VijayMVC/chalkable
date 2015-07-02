Create Procedure spSelectAdminAnnoucnement @adminAnnouncementT TAdminAnnouncement readonly
 As
	Select 
		t.*,
		(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = t.Id) as QnACount,	
		0 as StudentsCount, --todo get student from recipients table
		(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = t.Id) as AttachmentsCount,
		(select count(*) from AnnouncementAttachment where AnnouncementRef = t.Id  and PersonRef = t.AdminRef) as OwnerAttachmentsCount,
		0 as StudentsCountWithAttachments, --todo get student from recipients table
		(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = t.Id and Active = 1) as ApplicationCount
	From @adminAnnouncementT t
 Go

Create Procedure spGetAdminAnnouncements @id int, @personId int, @roleId int,  @ownedOnly bit
	,@fromDate DateTime2, @toDate DateTime2, @start int, @count int, @now DateTime2
	,@gradeLevelsIds nvarchar(256), @complete bit, @studentId int
As

Declare @allCount int;
Declare @gradeLevelsIdsT table(value int);
Declare @adminAnnouncementT TAdminAnnouncement

if(@gradeLevelsIds is not null)
begin
	insert into @gradeLevelsIdsT(value)
	select cast(s as int) from dbo.split(',', @gradeLevelsIds)
end

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
		and (@gradeLevelsIds is null or  
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
					where StudentGroup.StudentRef = @studentId
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
	and (@gradeLevelsIds is null or  
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
				where StudentGroup.StudentRef = @studentId
			)
		)
order by Created desc				
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

exec spSelectAdminAnnoucnement @adminAnnouncementT
Go

Create Procedure spSelectClassAnnouncement  @classAnnT TClassAnnouncement readonly
As
Select 
	t.*,
	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = t.Id) as QnACount,	
	(Select COUNT(*) from ClassPerson where ClassRef = t.ClassRef) as StudentsCount,
	(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = t.Id) as AttachmentsCount,
	(select count(*) from AnnouncementAttachment where AnnouncementRef = t.Id  and PersonRef in
		(select PersonRef from ClassTeacher where ClassRef = t.ClassRef)
	) as OwnerAttachmentsCount,
	(	select COUNT(*) from
			(Select distinct PersonRef from AnnouncementAttachment 
			 where AnnouncementRef = t.Id and 
				   ClassRef in (select ClassPerson.ClassRef from ClassPerson 
											 where ClassPerson.PersonRef = AnnouncementAttachment.PersonRef)) as x
	) as StudentsCountWithAttachments,
	(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = t.Id and Active = 1) as ApplicationCount 

From @classAnnT t
Go

Create Procedure spGetClassAnnouncementsBySisActivities @personId int, @sisActivityIds nvarchar(max)
As

Declare @sisActivityIdsT table(Id int)
Declare @classAnnouncement TClassAnnouncement


If(@sisActivityIds is not null and LTRIM(@sisActivityIds) <> '')
Begin
	Insert Into @sisActivityIdsT(Id)
	Select cast(s as int) from dbo.split(',', @sisActivityIds)
End

Insert @classAnnouncement
Select 
	vwClassAnnouncement.*,
	cast((case when (select count(*) from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwClassAnnouncement.ClassRef) >= 1 then 1 else 0 end) as bit) as IsOwner,
	cast(0 as bit) as Complete,
	cast(0 as int) as AllCount
From vwClassAnnouncement
Where (@sisActivityIds is not null and SisActivityId is not null and SisActivityId in (select Id from @sisActivityIdsT))

exec spSelectClassAnnouncement @classAnnouncement
Go

select * from LessonPlan

select * from Announcement
where Id = 1000002199



Create Procedure spSelectLessonPlans @lessonPlanT TLessonPlan readonly
As
Select 
	t.*,
	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = t.Id) as QnACount,	
	(Select COUNT(*) from ClassPerson where ClassRef = t.ClassRef) as StudentsCount,
	(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = t.Id) as AttachmentsCount,
	(select count(*) from AnnouncementAttachment where AnnouncementRef = t.Id  and PersonRef in
		(select PersonRef from ClassTeacher where ClassRef = t.ClassRef)
	) as OwnerAttachmentsCount,
	(select COUNT(*) from
		(Select distinct PersonRef from AnnouncementAttachment 
			where AnnouncementRef = t.Id and 
				ClassRef in (select ClassPerson.ClassRef from ClassPerson 
											where ClassPerson.PersonRef = AnnouncementAttachment.PersonRef)) as x
	) as StudentsCountWithAttachments,
	(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = t.Id and Active = 1) as ApplicationCount  
From @lessonPlanT t
Go

Create Procedure spGetLessonPlans 	
	 @id int, @schoolId int, @personId int, @classId int, @roleId int, @ownedOnly bit
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
			(@roleId = 2 and exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef))
			or
			(@roleId = 3 and exists(select * from ClassPerson where PersonRef = @personId and ClassPerson.ClassRef = vwLessonPlan.ClassRef)) 
		)

	and (@ownedOnly = 0 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef))
	
	and (@fromDate is null or StartDate >= @fromDate)
	and (@toDate is null or StartDate <= @toDate)
	and (@complete is null or annRecipientData.Complete = @complete or (@complete = 0 and annRecipientData.Complete is null))
	and (@galleryCategoryId is null or GalleryCategoryRef = @galleryCategoryId)
	and SchoolRef = @schoolId
	
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
			(@roleId = 2 and exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef))
		  or
			(@roleId = 3 and exists(select * from ClassPerson where PersonRef = @personId and ClassPerson.ClassRef = vwLessonPlan.ClassRef)) 
		)

	and (@ownedOnly = 0 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef))
	
	and (@fromDate is null or StartDate >= @fromDate)
	and (@toDate is null or StartDate <= @toDate)
	and (@complete is null or annRecipientData.Complete = @complete or (@complete = 0 and annRecipientData.Complete is null))
	and (@galleryCategoryId is null or GalleryCategoryRef = @galleryCategoryId)
	and SchoolRef = @schoolId
	
order by Created desc				
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY


exec spSelectLessonPlans @lessonPlanT
Go
