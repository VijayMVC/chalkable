CREATE Procedure [dbo].[spCreateLessonPlan] @schoolYearId int, @classId int, @personId int, @created datetime2, @startDate datetime2, @endDate datetime2, @state int, @callerRole int
As
Begin Transaction
--Only Teacher can create Lesson Plan--
Declare @isDraft bit = 0
Declare @announcementId int

if @state = 0
Begin
	Select Top 1 @announcementId = Id
	From vwLessonPlan
	Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) and [State] = 0
	and SchoolYearRef  = @schoolYearId
	and ClassRef = @classId
	Order By Created Desc

	If @announcementId is not null
	update Announcement Set [State] = -1 Where Id = @announcementId
End

Declare @annIdT TInt32

Insert Into @annIdT
Select Id From vwLessonPlan
Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) and [State] = 0 and SchoolYearRef = @schoolYearId
Union
Select Id From vwClassAnnouncement
Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) and [State] = 0 and SchoolYearRef = @schoolYearId


/*Delete Lesson Plans datas*/
exec spDeleteAnnouncements @annIdT

/*RESTORE STATE FOR DRAFT*/
If @announcementId is not null
	Begin
		update Announcement Set [State] = 0 Where Id = @announcementId
		Set @isDraft = 1
	End
Else 
	Begin
		/*INSERT TO ANNOUNCEMENT*/
		Insert Into Announcement (Created, Title, Content, [State])
		Values(@created, null, null, @state)

		Set @announcementId = SCOPE_IDENTITY()

		Insert Into LessonPlan (Id, ClassRef, StartDate, EndDate, GalleryCategoryRef, SchoolYearRef, VisibleForStudent, InGallery)
		Values(@announcementId, @classId, @startDate, @endDate, null, @schoolYearId, 0, 0);


		/*GET CONTENT FROM PREV ANNOUNCEMENT*/
		--Declare @prevContent nvarchar(1024)
		--Select top 1
		--@prevContent = Content From vwLessonPlan
		--Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId)
		--and [State] = 1 and Content is not null
		--Order By Created Desc

		--update Announcement Set Content = @prevContent Where Id = @announcementId
	End

Exec spGetLessonPlanDetails @announcementId, @personId, @callerRole, @schoolYearId

Commit