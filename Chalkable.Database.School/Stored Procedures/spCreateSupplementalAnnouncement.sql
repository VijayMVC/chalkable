CREATE Procedure [dbo].[spCreateSupplementalAnnouncement] 
	@created datetime, 
	@expires datetime,
	@classId int, 
	@classAnnouncementTypeId int,
	@personId int, 
	@state int
AS
Begin Transaction
--Only Teacher can create Suplement Announcement--
Declare @callerRole int = 2
Declare @isDraft bit = 0
Declare @announcementId int
Declare @schoolYearId int;
Set @schoolYearId = (Select Top 1 SchoolYearRef From Class Where Id = @classId)

if @state = 0
Begin
	Select 
		Top 1 @announcementId = Id
	From 
		vwSupplementalAnnouncement
	Where 
		ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) and [State] = 0
			and SchoolYearRef  = @schoolYearId
			and ClassRef = @classId
	Order By 
		Created Desc

	If @announcementId is not null
		update 
			Announcement 
		Set 
			[State] = -1 
		Where 
			Id = @announcementId
End

Declare @annIdT TInt32

Insert Into @annIdT
	Select 
		Id 
	From 
		vwSupplementalAnnouncement
	Where 
		ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) and [State] = 0 and SchoolYearRef = @schoolYearId
	Union
		Select 
			Id 
		From 
			vwClassAnnouncement
		Where 
			ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) and [State] = 0 and SchoolYearRef = @schoolYearId


/*Delete Lesson Plans datas*/
exec spDeleteAnnouncements @annIdT

/*RESTORE STATE FOR DRAFT*/
If @announcementId is not null
Begin
	update Announcement Set [State] = 0 Where Id = @announcementId
	Set @isDraft = 1
	End
	Else Begin

	/*INSERT TO ANNOUNCEMENT*/
	Insert Into Announcement (Created, Title, Content, [State])
	Values(@created, null, null, @state)

	Set @announcementId = SCOPE_IDENTITY()

	Insert Into SupplementalAnnouncement (Id, Expires, VisibleForStudent, ClassRef, ClassAnnouncementTypeRef, SchoolYearRef)
	Values(@announcementId, @expires, 1, @classId, @classAnnouncementTypeId, @schoolYearId);

End


Declare @supplementedAnn TInt32;
Insert Into @supplementedAnn
values (@announcementId)

Exec spGetListOfSupplementalAnnouncementDetailsByIds @supplementedAnn, @personId, @callerRole, @schoolYearId

Commit
GO