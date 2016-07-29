CREATE Procedure [dbo].[spGetLessonPlans]
	@id int, 
	@schoolYearId int, 
	@personId int, 
	@classId int, 
	@roleId int, 
	@teacherId int,
	@studentId int,
	@ownedOnly bit,
	@fromDate DateTime2, 
	@toDate DateTime2, 
	@complete bit
As

declare @TEACHER_ROLE int = 2,
		@STUDENT_ROLE int = 3,
		@DISTRICT_ADMIN_ROLE int = 10;

Select
	vwLessonPlan.*,
	cast((Case When GalleryOwnerRef = @personId or exists(Select * From ClassTeacher CT where CT.PersonRef = @personId and CT.ClassRef = vwLessonPlan.ClassRef) then 1 else 0 End) as bit)  as IsOwner,
	cast((case when annRecipientData.Complete is null then 0 else annRecipientData.Complete end) as bit) as Complete,
	count(vwLessonPlan.Id) over() as AllCount
From
	vwLessonPlan left join 
	(select * from AnnouncementRecipientData where PersonRef = @personId) annRecipientData 
		on annRecipientData.AnnouncementRef = vwLessonPlan.Id
Where
	(@id is not null  or [State] = 1)
	and (@id is null or vwLessonPlan.Id = @id)
	and (@classId is null or ClassRef = @classId)
	and (
			(@roleId = @TEACHER_ROLE and (@ownedOnly = 0 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef)))
			or
			(@roleId = @STUDENT_ROLE and exists(select * from ClassPerson where PersonRef = @personId and ClassPerson.ClassRef = vwLessonPlan.ClassRef) and VisibleForStudent = 1)
			or
			 @roleId = @DISTRICT_ADMIN_ROLE --District Id
		)
	and (@teacherId is null or exists(select * from ClassTeacher where PersonRef = @teacherId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef))
	and (@studentId is null or exists(select * from ClassPerson where PersonRef = @studentId and ClassPerson.ClassRef = vwLessonPlan.ClassRef))
	and (@fromDate is null or StartDate >= @fromDate)
	and (@toDate is null or StartDate <= @toDate)
	and (@complete is null or annRecipientData.Complete = @complete or (@complete = 0 and annRecipientData.Complete is null))
	and (InGallery = 0)
	and (@schoolYearId is null or SchoolYearRef = @schoolYearId)
