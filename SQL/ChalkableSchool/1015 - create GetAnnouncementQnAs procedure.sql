create view [dbo].[vwAnnouncementQnA]
as
select
	ana.id as AnnouncementQnAId,
	ana.AnsweredTime as AnsweredTime,
	ana.QuestionTime as QuestionTime,
	ana.Question as Question,
	ana.Answer as Answer,
	ana.AnnouncementRef as AnnouncementRef,
	a.MarkingPeriodClassRef as MarkingPeriodClassRef,
	ana.State as [State],
	sp1.Id as AskerId,
	sp1.FirstName as AskerFirstName,
	sp1.LastName as AskerLastName,
	sp1.Gender as AskerGender,
	sp2.Id as AnswererId,
	sp2.FirstName as AnswererFirstName,
	sp2.LastName as AnswererLastName,
	sp2.Gender as AnswererGender
from AnnouncementQnA ana
join vwPerson sp1 on sp1.Id = ana.PersonRef
join Announcement a on a.Id = ana.AnnouncementRef
join vwPerson sp2 on sp2.Id = a.PersonRef
GO

create procedure [dbo].[spGetAnnouncementsQnA] @callerId uniqueidentifier, @announcementQnAId uniqueidentifier
, @announcementId uniqueidentifier, @askerId uniqueidentifier, @answererId uniqueidentifier
as
declare @callerRolerId int = (select RoleRef from Person where Id = @callerId)
select vwAnnouncementQnA.*,
	   cast((case when @callerId = vwAnnouncementQnA.AskerId then 1 else 0 end) as bit) as IsOwner
from vwAnnouncementQnA 
where (@announcementId is null or AnnouncementRef = @announcementId)
	 and (@askerId is null or @askerId = AskerId)
	 and (@answererId is  null or @answererId = AnswererId)
	 and (@callerRolerId = 1 or @callerId = AnswererId  or @callerId = AskerId or 
			(MarkingPeriodClassRef is not null and AnsweredTime is not null 
											and exists(select * from MarkingPeriodClass mpc
															join ClassPerson csp on csp.ClassRef = mpc.ClassRef
															where mpc.Id = MarkingPeriodClassRef and @callerId = csp.PersonRef
												 	  )
			 )
		  )
	 and (@announcementQnAId is null or @announcementQnAId = AnnouncementQnAId)

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
	exec spGetAnnouncementsQnA @callerId, null, @id, null, null
	
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



