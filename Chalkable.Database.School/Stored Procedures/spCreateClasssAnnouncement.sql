


-----------------------------
-- CREATE CLASS ANNOUNCEMENT
-----------------------------
CREATE Procedure [dbo].[spCreateClasssAnnouncement] @schoolYearId int, @classAnnouncementTypeId int, @personId int, @created datetime2,
@expires datetime2, @state int, @gradingStyle int, @classId int
as
begin transaction
	--- Only teacher can create class announcement ---
	declare @callerRole int = 2
	declare @announcementId int
	declare @isDraft bit = 0

	if @state = 0
	begin
		select top 1 @announcementId = Id
		from vwClassAnnouncement
		where 
			ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId) 
			and [State] = 0
			and (@classAnnouncementTypeId is null or ClassAnnouncementTypeRef = @classAnnouncementTypeId)
			and SchoolYearRef  = @schoolYearId
		order by Created desc

		if @announcementId is null
		select top 1 @announcementId = Id from vwClassAnnouncement
		where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId) and [State] = 0
		and SchoolYearRef  = @schoolYearId
		order by Created desc

		if @announcementId is not null
		update Announcement set [State] = -1 where Id = @announcementId
	end


	declare @annIdT TInt32
	insert into @annIdT
	select Id from vwClassAnnouncement
	Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId)
	AND ClassAnnouncementTypeRef = @classAnnouncementTypeId AND [State] = 0
	and SchoolYearRef  = @schoolYearId
	Union
	Select Id From vwLessonPlan
	Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) And [State] = 0
	and SchoolYearRef  = @schoolYearId

	exec spDeleteAnnouncements @annIdT


	/*RESTORE STATE FOR DRAFT*/
	if @announcementId is not null
	begin
		update Announcement set [State] = 0 where Id = @announcementId
		set @isDraft = 1
		end
		else begin
		/*INSERT TO ANNOUNCEMENT*/
		insert into Announcement (Created, Title, Content, [State], DiscussionEnabled, PreviewCommentsEnabled, RequireCommentsEnabled)
		values(@created, null, null, @state, 0 ,0 ,0);

		set @announcementId = SCOPE_IDENTITY()

		insert into ClassAnnouncement(Id, ClassRef, ClassAnnouncementTypeRef, Expires, SchoolYearRef, Dropped,MayBeDropped, MaxScore, [Order],VisibleForStudent, WeightAddition, WeightMultiplier, IsScored)
		values(@announcementId, @classId, @classAnnouncementTypeId, @expires, @schoolYearId, 0, 0, 0, 0, 1, 0, 1, 1)


		/*GET CONTENT FROM PREV ANNOUNCEMENT*/
		--declare @prevContent nvarchar(1024)
		--select top 1
		--@prevContent = Content from vwClassAnnouncement
		--where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
		--and ClassAnnouncementTypeRef = @classAnnouncementTypeId
		--and [State] = 1
		--and Content is not null
		--order by Created desc

		--update Announcement set Content = @prevContent where Id = @announcementId
	end

	if(@classAnnouncementTypeId is not null and @classId is not null)
	begin
	declare @reorderRes TInt32
	insert into @reorderRes
	exec [spReorderAnnouncements] @schoolYearId, @classAnnouncementTypeId,  @classId
	end
	exec spGetClassAnnouncementDetails @announcementId, @personId, @callerRole, @schoolYearId

commit