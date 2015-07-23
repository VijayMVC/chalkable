-----------------------------------------
-- Add SchoolYearRef to ClassAnnouncement
-----------------------------------------
Alter Table ClassAnnouncement
Add SchoolYearRef int  null Constraint FK_ClassAnnouncement_SchoolYear Foreign Key References SchoolYear(Id)
Go 

Update a1
 	Set a1.SchoolYearRef = c.SchoolYearRef
From ClassAnnouncement a1
Join ClassAnnouncement a2 on a2.Id = a1.Id
Join Class c on c.Id = a2.ClassRef
Where c.SchoolYearRef is not null 

Update a1
 	Set a1.SchoolYearRef = sy.Id
From ClassAnnouncement a1
Join ClassAnnouncement a2 on a2.Id = a1.Id
join Announcement a on a.Id = a2.Id
join SchoolYear sy on sy.SchoolRef = a2.SchoolRef
Where a.Created between sy.StartDate and sy.EndDate


Alter Table ClassAnnouncement
Alter Column SchoolYearRef int not null 
Go

Alter Table ClassAnnouncement
Drop Constraint FK_ClassAnnouncement_School
Go

Alter Table ClassAnnouncement
Drop Column SchoolRef
Go


-----------------------------------------
-- Add SchoolYearRef to LessonPlan
-----------------------------------------

Alter Table LessonPlan
Add SchoolYearRef int  null Constraint FK_LessonPlan_SchoolYear Foreign Key References SchoolYear(Id)
Go 

Update a1
 	Set a1.SchoolYearRef = c.SchoolYearRef
From LessonPlan a1
Join LessonPlan a2 on a2.Id = a1.Id
Join Class c on c.Id = a2.ClassRef
Where c.SchoolYearRef is not null 

Update a1
 	Set a1.SchoolYearRef = sy.Id
From LessonPlan a1
Join LessonPlan a2 on a2.Id = a1.Id
join Announcement a on a.Id = a2.Id
join SchoolYear sy on sy.SchoolRef = a2.SchoolRef
Where a.Created between sy.StartDate and sy.EndDate

Alter Table LessonPlan
Alter Column SchoolYearRef int not null 
Go

Alter Table LessonPlan
Drop Constraint FK_LessonPlan_School
Go

Alter Table LessonPlan
Drop Column SchoolRef
Go

---------------------------------------------
-- added SchoolYearRef to vwClassAnnouncement
---------------------------------------------

Alter View [dbo].[vwClassAnnouncement]
As
Select
Announcement.Id as Id,
Announcement.Created as Created,
Announcement.[State] as [State],
Announcement.Content as Content,
Announcement.Title as [Title],
ClassAnnouncement.Expires as Expires,
ClassAnnouncement.[Order] as [Order],
ClassAnnouncement.Dropped as Dropped,
ClassAnnouncement.ClassAnnouncementTypeRef as ClassAnnouncementTypeRef,
ClassAnnouncement.SchoolYearRef as SchoolYearRef,
ClassAnnouncement.SisActivityId as SisActivityId,
ClassAnnouncement.MaxScore as MaxScore,
ClassAnnouncement.WeightAddition as WeightAddition,
ClassAnnouncement.WeightMultiplier as WeightMultiplier,
ClassAnnouncement.MayBeDropped as MayBeDropped,
ClassAnnouncement.VisibleForStudent as VisibleForStudent,
ClassAnnouncement.ClassRef as ClassRef,

Staff.FirstName + ' ' + Staff.LastName as PrimaryTeacherName,
Staff.Gender as PrimaryTeacherGender,
Class.Name as ClassName,
Class.Name + ' ' + (case when Class.ClassNumber is null then '' else Class.ClassNumber end) as FullClassName,
Class.MinGradeLevelRef as MinGradeLevelId,
Class.MaxGradeLevelRef as MaxGradeLevelId,
Class.PrimaryTeacherRef as PrimaryTeacherRef,
Class.ChalkableDepartmentRef as DepartmentId

From ClassAnnouncement
Join Announcement on Announcement.Id = ClassAnnouncement.Id
Join Class on Class.Id = ClassAnnouncement.ClassRef
Left Join Staff on Staff.Id = Class.PrimaryTeacherRef
GO

Alter procedure [dbo].[spSelectClassAnnouncement]  --@classAnnT TClassAnnouncement readonly
As
--Select 
--	t.*,
--	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = t.Id) as QnACount,	
--	(Select COUNT(*) from ClassPerson where ClassRef = t.ClassRef) as StudentsCount,
--	(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = t.Id) as AttachmentsCount,
--	(select count(*) from AnnouncementAttachment where AnnouncementRef = t.Id  and PersonRef in
--		(select PersonRef from ClassTeacher where ClassRef = t.ClassRef)
--	) as OwnerAttachmentsCount,
--	(	select COUNT(*) from
--			(Select distinct PersonRef from AnnouncementAttachment 
--			 where AnnouncementRef = t.Id and 
--				   ClassRef in (select ClassPerson.ClassRef from ClassPerson 
--											 where ClassPerson.PersonRef = AnnouncementAttachment.PersonRef)) as x
--	) as StudentsCountWithAttachments,
--	(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = t.Id and Active = 1) as ApplicationCount 

--From @classAnnT t

GO

Drop Type [TClassAnnouncement]
Go


Create Type [dbo].[TClassAnnouncement] AS TABLE(
	[Id] [int] NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[State] [int] NOT NULL,
	[Content] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
	[Expires] [datetime2](7) NOT NULL,
	[Order] [int] NOT NULL,
	[Dropped] [bit] NOT NULL,
	[ClassAnnouncementTypeRef] [int] NULL,
	[SchoolYearRef] [int] NULL,
	[SisActivityId] [int] NULL,
	[MaxScore] [decimal](18, 0) NULL,
	[WeightAddition] [decimal](9, 6) NULL,
	[WeightMultiplier] [decimal](9, 6) NULL,
	[MayBeDropped] [bit] NULL,
	[VisibleForStudent] [bit] NULL,
	[ClassRef] [int] NOT NULL,
	[PrimaryTeacherName] [nvarchar](max) NULL,
	[PrimaryTeacherGender] [nvarchar](max) NULL,
	[ClassName] [nvarchar](max) NULL,
	[FullClassName] [nvarchar](max) NULL,
	[MinGradeLevelId] [int] NULL,
	[MaxGradeLevelId] [int] NULL,
	[PrimaryTeacherRef] [int] NULL,
	[DepartmentId] [uniqueidentifier] NULL,
	[IsOwner] [bit] NULL,
	[Complete] [bit] NULL,
	[AllCount] [int] NULL
)
GO

Alter procedure [dbo].[spSelectClassAnnouncement]  @classAnnT TClassAnnouncement readonly
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
GO



---------------------------------------
-- Adding SchoolYearRef to vwLessonPlan
---------------------------------------



Alter View [dbo].[vwLessonPlan]
 As
 Select
	Announcement.Id as Id,
	Announcement.Created as Created,
	Announcement.[State] as [State],
	Announcement.Content as Content,
	Announcement.Title as [Title],
	LessonPlan.ClassRef as ClassRef,
	LessonPlan.SchoolYearRef as SchoolYearRef,
	LessonPlan.StartDate as StartDate,
	LessonPlan.EndDate as EndDate,
	LessonPlan.GalleryCategoryRef as GalleryCategoryRef,
	LessonPlan.VisibleForStudent as VisibleForStudent,
	
	Staff.FirstName + ' ' + Staff.LastName as PrimaryTeacherName,
	Staff.Gender as PrimaryTeacherGender,
	Class.Name as ClassName,
	Class.Name + ' ' + (case when Class.ClassNumber is null then '' else Class.ClassNumber end) as FullClassName,
	Class.MinGradeLevelRef as MinGradeLevelId,
	Class.MaxGradeLevelRef as MaxGradeLevelId,
	Class.PrimaryTeacherRef as PrimaryTeacherRef,  
	Class.ChalkableDepartmentRef as DepartmentId
	
 From LessonPlan
 Join Announcement on Announcement.Id = LessonPlan.Id
 Join Class on Class.Id = LessonPlan.ClassRef
 Left Join Staff on Staff.Id = Class.PrimaryTeacherRef

GO


Alter procedure [dbo].[spSelectLessonPlans] --@lessonPlanT TLessonPlan readonly
As
--Select 
--	t.*,
--	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = t.Id) as QnACount,	
--	(Select COUNT(*) from ClassPerson where ClassRef = t.ClassRef) as StudentsCount,
--	(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = t.Id) as AttachmentsCount,
--	(select count(*) from AnnouncementAttachment where AnnouncementRef = t.Id  and PersonRef in
--		(select PersonRef from ClassTeacher where ClassRef = t.ClassRef)
--	) as OwnerAttachmentsCount,
--	(select COUNT(*) from
--		(Select distinct PersonRef from AnnouncementAttachment 
--			where AnnouncementRef = t.Id and 
--				ClassRef in (select ClassPerson.ClassRef from ClassPerson 
--											where ClassPerson.PersonRef = AnnouncementAttachment.PersonRef)) as x
--	) as StudentsCountWithAttachments,
--	(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = t.Id and Active = 1) as ApplicationCount  
--From @lessonPlanT t
GO

Drop Type [TLessonPlan]
Go

Create Type [dbo].[TLessonPlan] AS TABLE(
	[Id] [int] NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[State] [int] NOT NULL,
	[Content] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
	[ClassRef] [int] NOT NULL,
	[SchoolYearRef] [int] NOT NULL,
	[StartDate] [datetime2](7) NULL,
	[EndDate] [datetime2](7) NULL,
	[GalleryCategoryRef] [int] NULL,
	[VisibleForStudent] [bit] NULL,
	[PrimaryTeacherName] [nvarchar](max) NULL,
	[PrimaryTeacherGender] [nvarchar](max) NULL,
	[ClassName] [nvarchar](max) NULL,
	[FullClassName] [nvarchar](max) NULL,
	[MinGradeLevelId] [int] NULL,
	[MaxGradeLevelId] [int] NULL,
	[PrimaryTeacherRef] [int] NULL,
	[DepartmentId] [uniqueidentifier] NULL,
	[IsOwner] [bit] NULL,
	[Complete] [bit] NULL,
	[AllCount] [int] NULL
)
GO


Alter procedure [dbo].[spSelectLessonPlans] @lessonPlanT TLessonPlan readonly
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
GO





Alter Procedure [dbo].[spGetClassAnnouncementDetails] @classAnnouncementId int, @callerId int, @callerRole int, @schoolYearId int
As

Declare @classAnn TClassAnnouncement
Declare @allCount int = 1
Declare @isOwner bit = 1
Declare @complete bit =  cast((Select Top 1 Complete from AnnouncementRecipientData where AnnouncementRef = @classAnnouncementId and PersonRef = @callerId) as bit)

If @complete is null 
	Set @complete = 0

If @callerRole = 2
Begin
	insert into @classAnn
	Select 
		vwClassAnnouncement.*, @isOwner, @complete, @allCount
	From vwClassAnnouncement
	Where Id = @classAnnouncementId and SchoolYearRef = @schoolYearId
		  and exists(select * from ClassTeacher where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwClassAnnouncement.ClassRef)   
End
If @callerRole = 3
Begin
	Set @isOwner = 0
	Insert into @classAnn
	Select vwClassAnnouncement.*, @isOwner, @complete, @allCount
	From vwClassAnnouncement
	Where Id = @classAnnouncementId and SchoolYearRef = @schoolYearId
		  and exists(select * from ClassPerson where ClassPerson.PersonRef = @callerId and ClassPerson.ClassRef = vwClassAnnouncement.ClassRef)   
End

Declare @primaryTeacherId int, @classId int
Select @primaryTeacherId = t.PrimaryTeacherRef, @classId = t.ClassRef, @classAnnouncementId = t.Id From @classAnn t


Declare @schoolId int = cast((Select Top 1 SchoolRef From SchoolYear Where Id = @schoolYearId) as int)
Exec spSelectClassAnnouncement @classAnn
Exec spSelectAnnouncementAddionalData @classAnnouncementId, @primaryTeacherId, @callerId, @schoolId

Declare @teacherIds TInt32
Insert Into @teacherIds
Select PersonRef From ClassTeacher Where ClassRef = @classId


Select * From AnnouncementAttachment
Where AnnouncementRef = @classAnnouncementId
		and (@callerRole = 2 or (@callerRole = 3 and (PersonRef = @callerId or exists(select * from @teacherIds t where t.value = PersonRef))))

GO



-----------------------------
-- CREATE CLASS ANNOUNCEMENT 
-----------------------------
Alter Procedure [dbo].[spCreateClasssAnnouncement] @schoolYearId int, @classAnnouncementTypeId int, @personId int, @created datetime2,
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
	insert into Announcement (Created, Title, Content, [State])
	values(@created, null, null, @state);
	set @announcementId = SCOPE_IDENTITY()

	insert into ClassAnnouncement(Id, ClassRef, ClassAnnouncementTypeRef, Expires, SchoolYearRef, Dropped,MayBeDropped, MaxScore, [Order],VisibleForStudent, WeightAddition, WeightMultiplier)
	values(@announcementId, @classId, @classAnnouncementTypeId, @expires, @schoolYearId, 0, 0, 0, 0, 1, 0, 1)


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
	 declare @reorderRes TInt32
	 insert into @reorderRes
	 exec [spReorderAnnouncements] @schoolYearId, @classAnnouncementTypeId,  @classId
end
exec spGetClassAnnouncementDetails @announcementId, @personId, @callerRole, @schoolYearId

commit
GO


---------------------------------
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
	
order by Created desc				
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY


exec spSelectLessonPlans @lessonPlanT

GO


Alter Procedure [dbo].[spGetLessonPlanDetails] @lessonPlanId int, @callerId int, @callerRole int, @schoolYearId int
As

Declare @lessonPlan TLessonPlan
Declare @allCount int = 1
Declare @isOwner bit = 1
Declare @complete bit =  cast((Select Top 1 Complete from AnnouncementRecipientData where AnnouncementRef = @lessonPlanId and PersonRef = @callerId) as bit)

If @complete is null 
	Set @complete = 0

If @callerRole = 2
Begin
	insert into @lessonPlan
	Select 
		vwLessonPlan.*, @isOwner, @complete, @allCount
	From vwLessonPlan
	Where Id = @lessonPlanId and SchoolYearRef  = @schoolYearId
			and exists(select * from ClassTeacher where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwLessonPlan.ClassRef)   
End
If @callerRole = 3
Begin
	Set @isOwner = 0
	Insert into @lessonPlan
	Select vwLessonPlan.*, @isOwner, @complete, @allCount
	From vwLessonPlan
	Where Id = @lessonPlanId and VisibleForStudent = 1 and SchoolYearRef  = @schoolYearId 
			and exists(select * from ClassPerson where ClassPerson.PersonRef = @callerId and ClassPerson.ClassRef = vwLessonPlan.ClassRef)   
End

Declare @primaryTeacherId int, @classId int
Select @primaryTeacherId = t.PrimaryTeacherRef, @classId = t.ClassRef, @lessonPlanId = t.Id From @lessonPlan t


Declare @schoolId int  = (Select SchoolRef from SchoolYear Where Id = @schoolYearId)
exec spSelectLessonPlans @lessonPlan
exec spSelectAnnouncementAddionalData @lessonPlanId, @primaryTeacherId, @callerId, @schoolId

Declare @teacherIds TInt32
Insert Into @teacherIds
Select PersonRef From ClassTeacher Where ClassRef = @classId


select * from AnnouncementAttachment
where AnnouncementRef = @lessonPlanId
	  and (@callerRole = 2 or (@callerRole = 3 and (PersonRef = @callerId or exists(select * from @teacherIds t where t.value = PersonRef))))
		
GO


-------------------------
-- CREATE LESSON PLAN 
-------------------------

Alter Procedure [dbo].[spCreateLessonPlan] @schoolYearId int, @classId int, @personId int, @created datetime2, @startDate datetime2, @endDate datetime2, @state int
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
		Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) and [State] = 0 and SchoolYearRef  = @schoolYearId
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
	Else Begin
		
		/*INSERT TO ANNOUNCEMENT*/
		Insert Into Announcement (Created, Title, Content, [State])
		Values(@created, null, null, @state)
		
		Set @announcementId = SCOPE_IDENTITY()

		Insert Into LessonPlan (Id, ClassRef, StartDate, EndDate, GalleryCategoryRef, SchoolYearRef, VisibleForStudent)
		Values(@announcementId, @classId, @startDate, @endDate, null, @schoolYearId, 1);
		
		
		/*GET CONTENT FROM PREV ANNOUNCEMENT*/
		Declare @prevContent nvarchar(1024)
		Select top 1
		@prevContent = Content From vwLessonPlan
		Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId)
				and [State] = 1 and Content is not null 
		Order By Created Desc

		update Announcement Set Content = @prevContent Where Id = @announcementId
	End

	Exec spGetLessonPlanDetails @announcementId, @personId, @callerRole, @schoolYearId

Commit
GO

Alter Procedure [dbo].[spCreateFromTemplate] @lessonPlanTemplateId int, @schoolYearId int, @personId int, @classId int
As
Begin Transaction
	Declare @callerRole int =2 
	Declare @isDraft bit = 0
	Declare @announcementId int
	

	Declare @content nvarchar(max), @title nvarchar(max), @startDate datetime2, @endDate datetime2, @visibleForStudent bit

	Select Top 1 @announcementId = Id,
				@title = vwLessonPlan.Title
	From vwLessonPlan
	Where ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where PersonRef = @personId) and [State] = 0 and SchoolYearRef = @schoolYearId
	Order By Created Desc

	Select Top 1
			@content = Content,
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
		
		Insert Into LessonPlan(Id, ClassRef, StartDate, EndDate, GalleryCategoryRef, SchoolYearRef, VisibleForStudent)
		Values(@announcementId, @classId, @startDate, @endDate, null, @schoolYearId, @visibleForStudent)
	End
	
    Insert Into AnnouncementStandard(AnnouncementRef, StandardRef)
	Select @announcementId, StandardRef
	From AnnouncementStandard
	Where AnnouncementRef = @lessonPlanTemplateId


	exec spGetLessonPlanDetails @announcementId, @personId, @callerRole, @schoolYearId
Commit
GO