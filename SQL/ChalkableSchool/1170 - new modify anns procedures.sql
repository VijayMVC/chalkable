ALTER procedure [dbo].[spReorderAnnouncements] @schoolYearId int, @classAnnType int, @classId int
as
with AnnView as
               (
                select a.Id, Row_Number() over(order by ClassAnnouncement.Expires, a.[Created]) as [Order]  
                from Announcement a
				join ClassAnnouncement on ClassAnnouncement.Id = a.Id
                join Class c on c.Id = ClassAnnouncement.ClassRef
				where c.SchoolYearRef = @schoolYearId and ClassAnnouncement.ClassAnnouncementTypeRef = @classAnnType and ClassAnnouncement.ClassRef = @classId
               )
update ClassAnnouncement
set [Order] = AnnView.[Order]
from AnnView 
where AnnView.Id = ClassAnnouncement.Id
select  1
GO


Create Procedure spDeleteAnnouncements @announcementIdList TIntId readonly
as
Begin Transaction

	/*DELETE AnnouncementGroups*/
	Delete From AnnouncementGroup Where AnnouncementRef in (Select Id From @announcementIdList)

	/*DELETE AdminAnnouncement*/
	Delete From AdminAnnouncement Where Id in (Select Id From @announcementIdList)

	/*Delete LessonPlan*/
	Delete From LessonPlan Where Id in (Select Id From @announcementIdList)

	/*Delete ClassAnnouncement*/
	Delete From ClassAnnouncement Where Id in (Select Id From @announcementIdList)
			
	/*DELETE AnnouncementRecipientData*/
	Delete From AnnouncementRecipientData where AnnouncementRef in (select Id from @announcementIdList)

	/*DELETE Attachment*/
	Delete From AnnouncementAttachment where AnnouncementRef in (select Id from @announcementIdList)
	/*DELETE AnnouncementApplication*/

	Delete From [Notification]
	Where AnnouncementRef in (select Id from @announcementIdList)

	/*DELETE ANOUNCEMENTQNA*/
	Delete From AnnouncementQnA
	Where AnnouncementRef in (select Id from @announcementIdList)

	/*DELETE AutoGrade*/
	Delete From AutoGrade
	Where AnnouncementApplicationRef IN 
		(SELECT AnnouncementApplication.Id From AnnouncementApplication
		 Where AnnouncementRef in (Select Id From @announcementIdList))


	/*DELETE AnnouncementApplication*/
	Delete From AnnouncementApplication
	Where AnnouncementRef in (Select Id From @announcementIdList)

	/*DELETE AnnouncementStandard*/
	Delete From AnnouncementStandard
	Where AnnouncementRef in (Select Id From @announcementIdList)

	/*DELETE Announcement*/
	Delete From Announcement Where Id in (Select Id From @announcementIdList)
	
Commit
Go
-------------------------
-- CREATE LESSON PLAN 
-------------------------

Create Procedure spCreateLessonPlan @schoolId int, @classId int, @personId int, @created datetime2, @startDate datetime2, @endDate datetime2, @state int
As
Begin Transaction
--Only Teacher can create Lesson Plan-- 	
	Declare @callerRole int =2 
	Declare @isDraft bit = 0
	Declare @announcementId int

	if @state = 0
	Begin
		Select Top 1 @announcementId = Id 
		From vwLessonPlan
		Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) and [State] = 0
		Order By Created Desc

		If @announcementId is not null
			update Announcement Set [State] = -1 Where Id = @announcementId
	End

	Declare @annIdT TIntId
	
	Insert Into @annIdT
	Select Id From vwLessonPlan
	Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) and [State] = 0
	Union
	Select Id From vwClassAnnouncement
	Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) and [State] = 0

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

		Insert Into LessonPlan (Id, ClassRef, StartDate, EndDate, GalleryCategoryRef, SchoolRef, VisibleForStudent)
		Values(@announcementId, @classId, @startDate, @endDate, null, @schoolId, 1);
		
		
		/*GET CONTENT FROM PREV ANNOUNCEMENT*/
		Declare @prevContent nvarchar(1024)
		Select top 1
		@prevContent = Content From vwLessonPlan
		Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId)
				and [State] = 1
  				and Content is not null
		Order By Created Desc

		update Announcement Set Content = @prevContent Where Id = @announcementId
	End

	Exec spGetLessonPlanDetails @announcementId, @personId, @callerRole, @schoolId

Commit
Go

------------------------------
-- CREATE LESSON FROM TEMPLATE 
------------------------------

Create Procedure spCreateFromTemplate @lessonPlanTemplateId int, @schoolId int, @personId int, @classId int
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



-----------------------------
-- CREATE CLASS ANNOUNCEMENT 
-----------------------------
Create Procedure [dbo].[spCreateClasssAnnouncement] @schoolId int, @classAnnouncementTypeId int, @personId int, @created datetime2,
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
	where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId) and [State] = 0
		  and (@classAnnouncementTypeId is null or ClassAnnouncementTypeRef = @classAnnouncementTypeId)
	order by Created desc

	if @announcementId is null
		select top 1 @announcementId = Id from vwClassAnnouncement
		where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId) and [State] = 0
		order by Created desc

	if @announcementId is not null
	update Announcement set [State] = -1 where Id = @announcementId
end


declare @annIdT TIntId
insert into @annIdT
select Id from vwClassAnnouncement
Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId)
	  AND ClassAnnouncementTypeRef = @classAnnouncementTypeId AND [State] = 0
Union 
Select Id From vwLessonPlan
Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) And [State] = 0

exec spDeleteAnnouncements @annIdT


/*RESTORE STATE FOR DRAFT*/
if @announcementId is not null
begin
	update Announcement set [State] = 0 where Id = @announcementId
	set @isDraft = 1
end
else begin
	/*INSERT TO ANNOUNCEMENT*/
	insert into Announcement (Created, Title, Content, [State])
	values(@created, null, null, @state);
	set @announcementId = SCOPE_IDENTITY()

	insert into ClassAnnouncement(Id, ClassRef, ClassAnnouncementTypeRef, Expires, SchoolRef, Dropped,MayBeDropped, MaxScore, [Order],VisibleForStudent, WeightAddition, WeightMultiplier)
	values(@announcementId, @classId, @classAnnouncementTypeId, @expires, @schoolId, 0, 0, 0, 0, 1, 0, 1)


	/*GET CONTENT FROM PREV ANNOUNCEMENT*/
	declare @prevContent nvarchar(1024)
	select top 1
	@prevContent = Content from vwClassAnnouncement
	where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
		  and ClassAnnouncementTypeRef = @classAnnouncementTypeId
		  and [State] = 1
  		  and Content is not null
	order by Created desc

	update Announcement set Content = @prevContent where Id = @announcementId
end

if(@classAnnouncementTypeId is not null and @classId is not null)
begin 
	declare @schoolYearId int
	select @schoolYearId = Id from SchoolYear where @created between StartDate and EndDate
	declare @resT table (res int)
	insert into @resT
	exec [spReorderAnnouncements] @schoolYearId, @classAnnouncementTypeId,  @classId
end
exec spGetClassAnnouncementDetails @announcementId, @personId, @callerRole, @schoolId
commit
GO

-----------------------------
-- CREATE ADMIN ANNOUNCEMENT 
-----------------------------

Alter Procedure [dbo].[spCreateAdminAnnouncement] @personId int, @created datetime2, @expires datetime2, @state int
as
begin transaction
--Only districtAdmn can create admin announcement 	
declare @callerRole int = 10 
declare @announcementId int
declare @isDraft bit = 0

if @state = 0 
begin
	select top 1 @announcementId = Id
	from vwAdminAnnouncement
	where AdminRef = @personId and [State] = 0
	order by Created desc

	if @announcementId is not null
	update Announcement set [State] = -1 where Id = @announcementId
end

/*Delete AdminAnnouncementData*/

declare @annIdT TIntId
Insert Into @annIdT
Select Id From vwAdminAnnouncement
Where AdminRef = @personId AND [State] = 0

exec spDeleteAnnouncements @annIdT

if @announcementId is not null
begin
	update Announcement set [State] = 0 where Id = @announcementId
	set @isDraft = 1
end
else begin
		/*INSERT TO ANNOUNCEMENT*/
		insert into Announcement (Created, Title, Content, [State])
		values(@created, null, null, @state)
		
		set @announcementId = SCOPE_IDENTITY()

		insert into AdminAnnouncement(Id, AdminRef, Expires)
		values(@announcementId, @personId, @expires);
				

	/*GET CONTENT FROM PREV ANNOUNCEMENT*/
	declare @prevContent nvarchar(1024)
	select top 1
	@prevContent = Content from vwAdminAnnouncement
	where AdminRef = @personId and [State] = 1 and Content is not null
	order by Created desc
	
	update Announcement set Content = @prevContent where Id = @announcementId
end

exec spGetAdminAnnouncementDetails @announcementId, @personId, @callerRole
commit
GO

Drop Procedure spCreateAnnouncement
Go

Drop Procedure spDeleteAnnouncement
Go
