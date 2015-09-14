Alter Procedure spCreateFromTemplate @lessonPlanTemplateId int, @schoolId int, @personId int, @classId int
As
Begin Transaction
	Declare @callerRole int =2 
	Declare @isDraft bit = 0
	Declare @announcementId int
	

	Declare @content nvarchar(max), @title nvarchar(max), @startDate datetime2, @endDate datetime2, @visibleForStudent bit

	Select Top 1 @announcementId = Id 
	From vwLessonPlan
	Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) and [State] = 0
	Order By Created Desc

	Select Top 1
			@content = Content,
			@title = Title,
			@startDate = StartDate,
			@endDate = EndDate,
			@visibleForStudent = VisibleForStudent 
	From vwLessonPlan
	Where Id = @lessonPlanTemplateId

	
	If @announcementId is not null
	Begin
		--todo maybe create delete stored procedure for this 
		delete from AnnouncementAttachment
		where AnnouncementRef = @announcementId
		
		delete from AnnouncementQnA
		where AnnouncementRef = @announcementId
		
		delete from AnnouncementApplication
		where AnnouncementRef = @announcementId
		
		delete from AnnouncementStandard
		where AnnouncementRef = @announcementId

		delete from AnnouncementAssignedAttribute
		where AnnouncementRef = @announcementId

		update Announcement
		set Content = @content, Title = @title
		where Id = @announcementId

		update LessonPlan
		set StartDate = @startDate, EndDate = @endDate, VisibleForStudent = @visibleForStudent
		where Id = @announcementId
	End
	Else Begin
		Insert Into Announcement
		Values(@content, GETDATE(), 0, @title)

		Set @announcementId = SCOPE_IDENTITY()
		
		Insert Into LessonPlan(Id, ClassRef, StartDate, EndDate, GalleryCategoryRef, SchoolRef, VisibleForStudent)
		Values(@announcementId, @classId, @startDate, @endDate, null, @schoolId, @visibleForStudent)
	End

	Insert Into AnnouncementApplication (AnnouncementRef, ApplicationRef, Active, [Order])
	Select @announcementId, ApplicationRef, 0, [Order] 
	From AnnouncementApplication
	Where AnnouncementRef = @lessonPlanTemplateId

    Insert Into AnnouncementStandard(AnnouncementRef, StandardRef)
	Select @announcementId, StandardRef
	From AnnouncementStandard
	Where AnnouncementRef = @lessonPlanTemplateId


	exec spGetLessonPlanDetails @announcementId, @personId, @callerRole, @schoolId
Commit
Go
