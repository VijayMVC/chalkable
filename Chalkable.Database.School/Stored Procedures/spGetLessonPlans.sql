

---------------------------------
-- Alter lessonPlan procecdures
---------------------------------


CREATE Procedure [dbo].[spGetLessonPlans]
	@id int, 
	@schoolYearId int, 
	@personId int, 
	@classId int, 
	@roleId int, 
	@ownedOnly bit,
	@fromDate DateTime2, 
	@toDate DateTime2, 
	@start int, 
	@count int,
	@complete bit, 
	@galleryCategoryId int
As

Declare @lessonPlanT TLessonPlan
Declare @allCount int

set @allCount =
(Select COUNT(*) from
	vwLessonPlan left join 
	(select * from AnnouncementRecipientData where PersonRef = @personId) annRecipientData 
		on annRecipientData.AnnouncementRef = vwLessonPlan.Id
Where
	(@id is not null  or [State] = 1) and
	(@id is null or vwLessonPlan.Id = @id)
	and (@classId is null or ClassRef = @classId)
	and (
			(@roleId = 2 and (@ownedOnly = 0 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef)))
			or
			(@roleId = 3 and exists(select * from ClassPerson where PersonRef = @personId and ClassPerson.ClassRef = vwLessonPlan.ClassRef) and VisibleForStudent = 1)
			or
			 @roleId = 10 --District Admin
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
From
	vwLessonPlan left join 
	(select * from AnnouncementRecipientData where PersonRef = @personId) annRecipientData 
		on annRecipientData.AnnouncementRef = vwLessonPlan.Id
Where
	(@id is not null  or [State] = 1)
	and (@id is null or vwLessonPlan.Id = @id)
	and (@classId is null or ClassRef = @classId)
	and (
			(@roleId = 2 and (@ownedOnly = 0 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef)))
			or
			(@roleId = 3 and exists(select * from ClassPerson where PersonRef = @personId and ClassPerson.ClassRef = vwLessonPlan.ClassRef) and VisibleForStudent = 1)
			or
			 @roleId = 10 --District Id
		)
	and (@fromDate is null or StartDate >= @fromDate)
	and (@toDate is null or StartDate <= @toDate)
	and (@complete is null or annRecipientData.Complete = @complete or (@complete = 0 and annRecipientData.Complete is null))
	and (@galleryCategoryId is null or GalleryCategoryRef = @galleryCategoryId)
	and SchoolYearRef = @schoolYearId
Order By Created Desc
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

exec spSelectLessonPlans @lessonPlanT