alter VIEW [dbo].[vwAnnouncementQnA]
AS
	SELECT
		ana.id as Id,
		ana.AnsweredTime as AnsweredTime,
		ana.QuestionTime as QuestionTime,
		ana.Question as Question,
		ana.Answer as Answer,
		ana.AnnouncementRef as AnnouncementRef,
		a.ClassRef as ClassRef,
		ana.State as [State],
		sp1.Id as AskerId,
		sp1.FirstName as AskerFirstName,
		sp1.LastName as AskerLastName,
		sp1.Gender as AskerGender,
		sp1.RoleRef as AskerRoleRef,
		sp1.SchoolRef as AskerSchoolRef,
		sp2.Id as AnswererId,
		sp2.FirstName as AnswererFirstName,
		sp2.LastName as AnswererLastName,
		sp2.Gender as AnswererGender,
		sp2.RoleRef as AnswererRoleRef,
		sp2.SchoolRef as AnswererSchoolRef
	FROM
		AnnouncementQnA ana
		join vwPerson sp1 on sp1.Id = ana.AskerRef
		join Announcement a on a.Id = ana.AnnouncementRef
		left join vwPerson sp2 on sp2.Id = ana.AnswererRef
GO

alter procedure [dbo].[spGetAnnouncementsQnA] @callerId int, @announcementQnAId int, @announcementId int
											, @askerId int, @answererId int, @schoolId int
as
	declare @callerRolerId int = (select top 1 RoleRef from SchoolPerson where PersonRef = @callerId)

	select vwAnnouncementQnA.*,
		cast((case when @callerId = vwAnnouncementQnA.AskerId then 1 else 0 end) as bit) as IsOwner
	from vwAnnouncementQnA
	where (@announcementId is null or AnnouncementRef = @announcementId)
		and (@askerId is null or @askerId = AskerId)
		and (@answererId is  null or @answererId = AnswererId)
		and (@callerRolerId = 1 or @callerId = AnswererId  or @callerId = AskerId 
				or 	(ClassRef is not null and AnsweredTime is not null
						and exists(select * from ClassPerson cp where cp.ClassRef = ClassRef and @callerId = cp.PersonRef)
					)
				or (ClassRef is not null and exists(select * from ClassTeacher ct where ct.ClassRef = ClassRef and @callerId = ct.PersonRef))
			)
		and (@announcementQnAId is null or @announcementQnAId = Id)
		and (AskerSchoolRef = @schoolId and (AnswererSchoolRef is null or AnswererSchoolRef = @schoolId))
	order by QuestionTime
GO


---------------------------
---GET ANNOUNCEMENT DETAILS 
----------------------------
alter procedure [dbo].[spGetAnnouncementDetails] @id int, @callerId int, @callerRole int, @schoolId int
as

if @callerRole is null
select top 1 @callerRole = RoleRef from SchoolPerson where PersonRef = @callerId

declare @announcementTb table
(
	Id int not null,
	Created dateTime2 not null,
	Expires dateTime2 not null,
	[State] int not null,
	[Order] int not null,
	Content nvarchar(max),
	[Subject] nvarchar(max),
	[Title] nvarchar(max),
	GradingStyle int not null,
	Dropped bit not null,
	ClassAnnouncementTypeRef int null,
	SchoolRef int not null,
	SisActivityId int null,
    MaxScore decimal null,
    WeightAddition decimal null,
    WeightMultiplier decimal null,
    MayBeDropped bit,
	VisibleForStudent bit,
	ClassRef int,
	PrimaryTeacherName nvarchar(max),
	PrimaryTeacherGender nvarchar(max),
	ClassName nvarchar(max),
	FullClassName nvarchar(max),
	GradeLevelId int,
	PrimaryTeacherRef int,
	QnACount int,
	StudentsCount int,
	AttachmentsCount int,
	OwnerAttachmentsCount int,
	StudentsCountWithAttachments int,
	ApplicationCount int,
	IsOwner bit,
	RowNumber bigint,
	AllCount int
)

--if(@callerRole = 5 or @callerRole = 8  or @callerRole = 7 or @callerRole = 1)
--begin
--insert into @announcementTb
--exec spGetAdminAnnouncements @id, @schoolId, @callerId, null,  @callerRole, 0, 0, null, null, null, 0, 1, null, null
--end
if(@callerRole = 3)
begin
insert into @announcementTb
exec spGetStudentAnnouncements @id, @schoolId, @callerId, null, @callerRole, 0, 0, null, null, null, 0, 1, null, null
end
if(@callerRole = 2)
begin
insert into @announcementTb
exec spGetTeacherAnnouncements @id, @schoolId, @callerId, null,  @callerRole, 0, 0, null, null, null, 0, 1, null, 1, null
end

declare @annExists bit
if(exists(select * from @announcementTb))
set @annExists = 1
else set @annExists = 0

if(@annExists = 1)
begin
declare @ownersIds table(id int)
declare @primaryTeacherId int
declare @isOwner bit
declare @classId int
--declare @annTypeId int
declare @isGradeble bit = 0, @isGradebleType bit = 0

select @isOwner = a.IsOwner, @primaryTeacherId = a.PrimaryTeacherRef, @classId = a.ClassRef, @isGradebleType = 1 
from @announcementTb a

insert into @ownersIds
select PersonRef from ClassTeacher
where ClassRef = @classId

--if(@annTypeId = 2 or @annTypeId = 3 or @annTypeId = 4 or @annTypeId = 5
--	or @annTypeId =6 or @annTypeId =7 or @annTypeId =8 or @annTypeId =9 or @annTypeId = 10)
--set @isGradebleType = 1

if(@isOwner = 1 and @isGradebleType = 1) set @isGradeble = 1

--declare @markingPeriodClassId uniqueidentifier  = (select MarkingPeriodClassRef from @announcementTb)
declare @wasSubmittedToAdmin bit = 0;
--declare @finalGradeStatus int =  (select [Status] from FinalGrade where Id = @markingPeriodClassId)

select *, @isGradeble as IsGradeble,
	@isGradebleType as IsGradebleType
--,
--@finalGradeStatus as FinalGradeStatus
from @announcementTb

--TODO: announcementQnA stored procedure
exec spGetAnnouncementsQnA @callerId, null, @id, null, null, @schoolId


select * from AnnouncementAttachment
where AnnouncementRef = @id
	and (((@callerRole = 5 or @callerRole = 8  or @callerRole = 7) and (PersonRef = @callerId))
			or(@callerRole = 2 and (@isOwner = 1 or PersonRef = @callerId
									or (PersonRef in (select id from @ownersIds) and exists(select * from AnnouncementRecipient 
																		where RoleRef = @callerRole or PersonRef = @callerId or ToAll = 1))
								   )	
			  )
			or(@callerRole = 3 and (PersonRef = @callerId
									 or (PersonRef in (select id from @ownersIds) and exists(select * from ClassPerson where ClassRef = @classId and PersonRef = @callerId))
									)
			  )
		)

select aa.*
--as AnnouncementApplicationId,
--aa.AnnouncementRef as AnnouncementId,
--aa.Active as Active,
--aa.[Order] as [Order]
--,
--cast(case when exists(select * from ApplicationInstall where PersonRef = @callerId and Active = 1 and ApplicationRef = a.Id)
--	then 1 else 0
--end as bit)  as IsInstalledForMe
from AnnouncementApplication aa
where aa.AnnouncementRef = @id and (@annExists = 1) and aa.Active = 1

declare @date datetime2 = (select top 1 a.Expires from @announcementTb a)
declare @markingPeriodId int = (select top 1 Id from MarkingPeriod where @date between StartDate and EndDate)

exec spGetPersons @schoolId, @primaryTeacherId, @callerId, null, 0, 1, null,null,null,null,null,null,null, 1, @callerRole, @markingPeriodId, null, null

select * from AnnouncementStandard 
join [Standard] on [Standard].Id = AnnouncementStandard.StandardRef
where AnnouncementStandard.AnnouncementRef = @id
end


GO


