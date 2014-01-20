ALTER Procedure [dbo].[spGetStudentAnnouncementsForAnnouncement]
	@announcementId uniqueidentifier, @personId uniqueidentifier
as

		Insert Into StudentAnnouncement
		(AnnouncementRef, ClassPersonRef, Dropped, [State])
		select x.AnnouncementRef, x.classPersonRef, 0, 1 from
		StudentAnnouncement
		right join
		(select Announcement.Id as AnnouncementRef, ClassPerson.Id as classPersonRef from
		Announcement
		join MarkingPeriodClass on Announcement.MarkingPeriodClassRef = MarkingPeriodClass.Id
		join ClassPerson on ClassPerson.ClassRef = MarkingPeriodClass.ClassRef
		where Announcement.[State] = 1)x
		on StudentAnnouncement.ClassPersonRef = x.classPersonRef and x.AnnouncementRef = StudentAnnouncement.AnnouncementRef
		where
		x.AnnouncementRef = @announcementId
		and StudentAnnouncement.Id is null
	
	declare @roleId int
	select top 1 @roleId = RoleRef from Person where Id = @personId

	select vwPerson.*,
		   StudentAnnouncement.Id as StudentAnnouncement_Id,
		   StudentAnnouncement.ClassPersonRef as StudentAnnouncement_ClassPersonRef,
		   StudentAnnouncement.AnnouncementRef as StudentAnnouncement_AnnouncementRef,
		   StudentAnnouncement.ApplicationRef as StudentAnnouncement_ApplicationRef,
		   StudentAnnouncement.Comment as StudentAnnouncement_Comment,
		   StudentAnnouncement.Dropped as StudentAnnouncement_Dropped,
		   StudentAnnouncement.ExtraCredit as StudentAnnouncement_ExtraCredit,
		   StudentAnnouncement.GradeValue as StudentAnnouncement_GradeValue,
		   StudentAnnouncement.State as StudentAnnouncement_State,
		   ClassPerson.ClassRef as StudentAnnouncement_ClassId
	from StudentAnnouncement 
	join Announcement on StudentAnnouncement.AnnouncementRef = Announcement.Id
	join ClassPerson on StudentAnnouncement.ClassPersonRef = ClassPerson.Id
	join vwPerson  on vwPerson.Id = ClassPerson.PersonRef
	where  Announcement.[State] = 1 
	and AnnouncementRef = @announcementId
	and (ClassPerson.PersonRef = @personId or @roleId = 2 or @roleId = 5 or @roleId = 8 or @roleId = 7)
GO
ALTER procedure [dbo].[spGetAnnouncementDetails] @id uniqueIdentifier, @callerId uniqueIdentifier
as

declare @callerRole int

select top 1 @callerRole = RoleRef from Person where Id = @callerId

declare @announcementTb table
(
	Id uniqueidentifier not null,
	Created dateTime2 not null,
	Expires dateTime2 not null,
	[State] int not null,
	[Order] int not null,
	Content nvarchar(2048),
	[Subject] nvarchar(1024),
	GradingStyle int not null,
	Dropped bit not null,
	AnnouncementTypeRef int not null,
	PersonRef uniqueidentifier not null,
	MarkingPeriodClassRef uniqueidentifier,
	PersonName nvarchar(255),
	PersonGender nvarchar(10),
	AnnouncementTypeName nvarchar(255),
	ClassName nvarchar(255),
	GradeLevelId uniqueidentifier,
	CourseId uniqueidentifier,
	ClassId uniqueidentifier,
	MarkingPeriodId uniqueidentifier,
	QnACount int,
	StudentsCount int,
	AttachmentsCount int,
	OwnerAttachmentsCount int,
	StudentsCountWithAttachments int,
	GradingStudentsCount int,
	[Avg] int,
	ApplicationCount int,
	ApplicationName nvarchar(255),	
	IsOwner bit,
	RecipientDataSchoolPersonId int,
	Starred bit,
	RowNumber bigint,

	--StarredCount int,
	AllCount int
)

if(@callerRole = 5 or @callerRole = 8  or @callerRole = 7)
begin
	insert into @announcementTb
	exec spGetAdminAnnouncements @id, @callerId, null,  @callerRole, 0, 0, null, null, null, 0, 1, null, null
end
if(@callerRole = 3)
begin
	insert into @announcementTb
	exec spGetStudentAnnouncements @id, @callerId, null, @callerRole, 0, 0, 0, null, null, null, 0, 1, null
end
if(@callerRole = 2)
begin
	insert into @announcementTb
	exec spGetTeacherAnnouncements @id, @callerId, null,  @callerRole, 0, 0, 0, null, null, null, 0, 1, null, 1	 
end

declare @annExists bit
if(exists(select * from @announcementTb))
	set @annExists = 1
else set @annExists = 0

if(@annExists = 1)
begin
	declare @ownerId uniqueidentifier
	declare @classId uniqueidentifier
	declare @annTypeId int
	select @ownerId = PersonRef , @classId = ClassId, @annTypeId = AnnouncementTypeRef from @announcementTb

	declare @isGradeble bit = 0, @isGradebleType bit = 0
	if(@annTypeId = 2 or @annTypeId = 3 or @annTypeId = 4 or @annTypeId = 5 
	   or @annTypeId =6 or @annTypeId =7 or @annTypeId =8 or @annTypeId =9 or @annTypeId = 10)
	   set @isGradebleType = 1

	if(@ownerId = @callerId and @isGradebleType = 1)
		set @isGradeble = 1

	declare @markingPeriodClassId uniqueidentifier  = (select MarkingPeriodClassRef from @announcementTb)
	declare @wasSubmittedToAdmin bit = 0;
	if(@markingPeriodClassId is not null and exists(select * from FinalGrade where MarkingPeriodClassRef = @markingPeriodClassId and [Status] = 1))
		set @wasSubmittedToAdmin = 1
	
	select *, @isGradeble as IsGradeble,
			  @isGradebleType as IsGradebleType,
			  @wasSubmittedToAdmin as WasSubmittedToAdmin
	from @announcementTb

	--TODO: announcementQnA stored procedure
	--exec spGetAnnouncementsQnA @callerId, null, @id, null, null
	
	exec spGetStudentAnnouncementsForAnnouncement @id, @callerId
	
	select * from AnnouncementAttachment
	where AnnouncementRef = @id 
		and (
				((@callerRole = 5 or @callerRole = 8  or @callerRole = 7) and (PersonRef = @callerId))
				or(@callerRole = 2 and (@ownerId = @callerId or PersonRef = @callerId 
										or (@ownerId = PersonRef and exists(select * from AnnouncementRecipient where RoleRef = @callerRole or PersonRef = @callerId or ToAll = 1))
									)
				)
				or(@callerRole = 3 and (PersonRef = @callerId 
										or (PersonRef = @ownerId and exists(select * from ClassPerson where ClassRef = @classId and PersonRef = @callerId))
										)
				)
			) 
	
	select * from AnnouncementReminder
	where AnnouncementRef = @id and (@annExists = 1) 
		and (@callerRole = 1 or (@ownerId = @callerId and PersonRef is null) 
							or (PersonRef is not null and @callerId = PersonRef))


	--TODO: get AnnouncementApplicationInfo	
	--select aa.Id as AnnouncementApplicationId,
	--	aa.AnnouncementRef as AnnouncementId,
	--	aa.Active as Active,
	--	aa.[Order] as [Order],
	--	a.*,
	--	cast(case when exists(select * from ApplicationInstall where PersonRef = @callerId and Active = 1 and ApplicationRef = a.Id)
	--		then 1 else 0 
	--	end as bit)  as IsInstalledForMe 
	--from AnnouncementApplication aa
	--join Application a on a.Id = aa.ApplicationRef
	--where aa.AnnouncementRef = @announcementId and (@annExists = 1) 
	--	 and aa.Active = 1
	exec spGetPersons  @ownerId, @callerId, null, 0, 1, null,null,null,null,null,null,null, 1
end


GO


