-------------------------
--TABLES
-------------------------
alter table Announcement
drop constraint FK_Announcement_Person
go

alter table Announcement
drop column PersonRef
go

alter table Announcement
alter column ClassRef int not null
go

alter table Announcement
alter column ClassAnnouncementTypeRef int not null
go

EXEC sp_RENAME 'Announcement.CREATEd' , 'Created', 'COLUMN'
go


CREATE TABLE [dbo].[ClassTeacher]
(
	[PersonRef] [int] NOT NULL constraint FK_ClassTeacher_Person foreign key references Person(Id),
	[ClassRef] [int] NOT NULL constraint FK_ClassTeacher_Class foreign key references Class(Id),
	[IsHighlyQualified] [bit] NOT NULL,
	[IsCertified] [bit] NOT NULL,
	[IsPrimary] [bit] NOT NULL
)
go

alter table ClassTeacher
add constraint PK_ClassTeacher primary key (PersonRef, ClassRef)
go

EXEC sp_RENAME 'Class.TeacherRef' , 'PrimaryTeacherRef', 'COLUMN'
go


alter table AnnouncementQnA
add AnswererRef int null constraint FK_AnnouncementQnA_PersonRef foreign key references Person(Id)
go

EXEC sp_RENAME 'AnnouncementQnA.PersonRef' , 'AskerRef', 'COLUMN'
go



----------------------------
----------VIEWS-------------
----------------------------
ALTER VIEW [dbo].[vwClass]
AS
SELECT
	Class.Id as Class_Id,
	Class.ClassNumber as Class_ClassNumber,
	Class.Name as Class_Name,
	Class.Description as Class_Description,
	Class.SchoolYearRef as Class_SchoolYearRef,
	Class.PrimaryTeacherRef as Class_PrimaryTeacherRef,
	Class.GradeLevelRef as Class_GradeLevelRef,
	Class.ChalkableDepartmentRef as Class_ChalkableDepartmentRef,
	Class.SchoolRef as Class_SchoolRef,
	Class.CourseRef as Class_CourseRef,
	GradeLevel.Id as GradeLevel_Id,
	GradeLevel.Name as GradeLevel_Name,
	GradeLevel.Number as GradeLevel_Number,
	Person.Id as Person_Id,
	Person.FirstName as Person_FirstName,
	Person.LastName as Person_LastName,
	Person.Gender as Person_Gender,
	Person.Salutation as Person_Salutation,
	Person.Email as Person_Email,
	Person.AddressRef as Person_AddressRef,
	SchoolPerson.RoleRef as Person_RoleRef,
	SchoolYear.SchoolRef as Class_SchoolId 
FROM 
	Class	
	join GradeLevel on GradeLevel.Id = Class.GradeLevelRef
	left join Person on Person.Id = Class.PrimaryTeacherRef
	left join SchoolPerson on SchoolPerson.PersonRef = Class.PrimaryTeacherRef and SchoolPerson.SchoolRef = Class.SchoolRef
	left join SchoolYear on SchoolYear.Id = Class.SchoolYearRef
where Class.PrimaryTeacherRef is null or SchoolPerson.RoleRef is not null
GO
alter VIEW [dbo].[vwAnnouncement] 
AS 
SELECT
	Announcement.Id as Id,
	Announcement.Created as Created,
	Announcement.Expires as Expires,
	Announcement.[State] as [State],
	Announcement.[Order] as [Order],
	Announcement.Content as Content,
	Announcement.[Subject] as [Subject],
	Announcement.Title as [Title],
	Announcement.GradingStyle as GradingStyle,
	Announcement.Dropped as Dropped,
	Announcement.ClassAnnouncementTypeRef as ClassAnnouncementTypeRef,
	Announcement.SchoolRef as SchoolRef,
	Announcement.SisActivityId as SisActivityId,
    Announcement.MaxScore as MaxScore,
    Announcement.WeightAddition as WeightAddition,
    Announcement.WeightMultiplier as WeightMultiplier,
    Announcement.MayBeDropped as MayBeDropped,
	Announcement.VisibleForStudent as VisibleForStudent,
	ClassAnnouncementType.Name as ClassAnnouncementTypeName,
	ClassAnnouncementType.ChalkableAnnouncementTypeRef as ChalkableAnnouncementType, 
	Announcement.ClassRef as ClassRef,
	Person.FirstName + ' ' + Person.LastName as PrimaryTeacherName,
	Person.Gender as PrimaryTeacherGender,
	Class.Name as ClassName,
	Class.GradeLevelRef as GradeLevelId,
	Class.PrimaryTeacherRef as PrimaryTeacherRef,  
	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = Announcement.Id) as QnACount,	
	(Select COUNT(*) from ClassPerson where ClassRef = Announcement.ClassRef) as StudentsCount,
	(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id) as AttachmentsCount,
	(select count(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id  and Class.PrimaryTeacherRef = PersonRef) as OwnerAttachmentsCount,
	(	select COUNT(*) from
			(Select distinct PersonRef from AnnouncementAttachment 
			 where AnnouncementRef = Announcement.Id and 
				   Announcement.ClassRef in (select ClassPerson.ClassRef from ClassPerson 
											 where ClassPerson.PersonRef = AnnouncementAttachment.PersonRef)) as x
	) as StudentsCountWithAttachments,
	(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = Announcement.Id and Active = 1) as ApplicationCount

FROM 
	Announcement
	join Class on Class.Id = Announcement.ClassRef
	left join ClassAnnouncementType on Announcement.ClassAnnouncementTypeRef = ClassAnnouncementType.Id
	left join Person on Person.Id = Class.PrimaryTeacherRef
GO
ALTER VIEW [dbo].[vwAnnouncementQnA]
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
		sp2.Id as AnswererId,
		sp2.FirstName as AnswererFirstName,
		sp2.LastName as AnswererLastName,
		sp2.Gender as AnswererGender,
		sp2.RoleRef as AnswererRoleRef
	FROM
		AnnouncementQnA ana
		join vwPerson sp1 on sp1.Id = ana.AskerRef
		join Announcement a on a.Id = ana.AnnouncementRef
		join vwPerson sp2 on sp2.Id = ana.AnswererRef
GO



-------------------------------
----STORED---PORCEDURES--------
-------------------------------
alter procedure [dbo].[spGetTeacherAnnouncements]  
	@id int, @schoolId int, @personId int, @classId int, @roleId int, @staredOnly bit, @ownedOnly bit, @gradedOnly bit
	,@fromDate DateTime2, @toDate DateTime2, @markingPeriodId int, @start int, @count int, @now DateTime2, @allSchoolItems bit
	, @sisActivitiesIds nvarchar(max)
as 

declare @sisActivitiesIdsT table(Id int)
if(@sisActivitiesIds is not null and LTRIM(@sisActivitiesIds) <> '')
begin
	insert into @sisActivitiesIdsT(Id)
	select cast(s as int) from dbo.split(',', @sisActivitiesIds)
end


declare @gradeLevelsT table(Id int)
insert into @gradeLevelsT(Id)
select GradeLevelRef from Class
where PrimaryTeacherRef = @personId
group by GradeLevelRef 

declare @mpStartDate datetime2, @mpEndDate datetime2
if(@markingPeriodId is not null)
	select @mpStartDate = StartDate, @mpEndDate = EndDate from MarkingPeriod where Id = @markingPeriodId


declare @allCount int;
set @allCount = (select COUNT(*) from
	vwAnnouncement	
	left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
		 on vwAnnouncement.Id = ard.AnnouncementRef
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@classId is null or ClassRef = @classId)
	and ((@allSchoolItems = 1 and @roleId = 2) or vwAnnouncement.PrimaryTeacherRef = @personId 
			 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef))
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
	and (@staredOnly = 0 or Starred = 1)
	and (@ownedOnly = 0 or vwAnnouncement.PrimaryTeacherRef = @personId 
		 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef))
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
)


Select 
	vwAnnouncement.*,
	--cast((case vwAnnouncement.PrimaryTeacherRef when @personId then 1 else 0 end) as bit) as IsOwner,
	(select count(*) from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef) as IsOwner,
	ard.PersonRef as RecipientDataPersonId,
	ard.Starred as Starred,
	ROW_NUMBER() OVER(ORDER BY vwAnnouncement.Created desc) as RowNumber,
 	@allCount as AllCount
from 
	vwAnnouncement	
	left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
			on vwAnnouncement.Id = ard.AnnouncementRef
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@classId is null or ClassRef = @classId)and ((@allSchoolItems = 1 and @roleId = 2) or vwAnnouncement.PrimaryTeacherRef = @personId 
		 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef))			
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
	and (@staredOnly = 0 or Starred = 1)
	and (@ownedOnly = 0 or vwAnnouncement.PrimaryTeacherRef = @personId 
		 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwAnnouncement.ClassRef))
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
	
order by Created desc				
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
GO

alter procedure [dbo].[spGetStudentAnnouncements]  
	@id int, @schoolId int, @personId int, @classId int,  @roleId int, @staredOnly bit, @ownedOnly bit,  @gradedOnly bit
	, @fromDate DateTime2, @toDate DateTime2, @markingPeriodId int
	, @start int, @count int, @now DateTime2, @sisActivitiesIds nvarchar(max)
as 

declare @gradeLevelRef int = (select top 1 GradeLevelRef  
							  from StudentSchoolYear 
							  join SchoolYear on SchoolYear.Id = StudentSchoolYear.SchoolYearRef 
							  where StudentRef = @personId and SchoolYear.StartDate <= @now and SchoolYear.EndDate >= @now)

declare @mpStartDate datetime2, @mpEndDate datetime2
if(@markingPeriodId is not null)
	select @mpStartDate = StartDate, @mpEndDate = EndDate from MarkingPeriod where Id = @markingPeriodId

declare @sisActivitiesIdsT table(Id int)
if(@sisActivitiesIds is not null and LTRIM(@sisActivitiesIds) <> '')
begin
	insert into @sisActivitiesIdsT(Id)
	select cast(s as int) from dbo.split(',', @sisActivitiesIds)
end

declare @allCount int = (select COUNT(*) from
	vwAnnouncement	
	left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
		 on vwAnnouncement.Id = ard.AnnouncementRef
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))	
	and(@classId is null or ClassRef = @classId)
	and (exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))					
	and (@staredOnly = 0 or Starred = 1)
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
	and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
)
declare @notExpiredCount int = (select count(*) 
    from 
		vwAnnouncement	
		left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
			 on vwAnnouncement.Id = ard.AnnouncementRef
	where
		(@id is not null or Expires >= @now) and
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and(@classId is null or ClassRef = @classId)
		and (exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))				
		and (@staredOnly = 0 or Starred = 1)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))
		)

select *
from
	(
	Select 
		vwAnnouncement.*,
		0 as IsOwner,
		ard.PersonRef as RecipientDataPersonId,
		ard.Starred as Starred,
		ROW_NUMBER() OVER(ORDER BY  vwAnnouncement.Expires) as RowNumber,
		@allCount as AllCount
	from 
		vwAnnouncement	
		left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
			 on vwAnnouncement.Id = ard.AnnouncementRef
	where
		(@id is not null or Expires >= @now) and
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and(@classId is null or ClassRef = @classId)
		and (exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))					
		and (@staredOnly = 0 or Starred = 1)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))

	union 
	Select 
		vwAnnouncement.*,
		0 as IsOwner,
		ard.PersonRef as RecipientDataPersonId,
		ard.Starred as Starred,
		(ROW_NUMBER() OVER(ORDER BY  vwAnnouncement.Expires desc)) + @notExpiredCount as RowNumber,
		@allCount as AllCount
	from 
		vwAnnouncement	
		left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
			 on vwAnnouncement.Id = ard.AnnouncementRef
	where
		(@id is not null or Expires < @now) and
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@roleId = 1 or (@schoolId is not null and SchoolRef = @schoolId))
		and(@classId is null or ClassRef = @classId)
		and (exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))					
		and (@staredOnly = 0 or Starred = 1)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or (Expires between @mpStartDate and @mpEndDate))
		and (@sisActivitiesIds is null or (SisActivityId is not null and SisActivityId in (select Id from @sisActivitiesIdsT)))

	) x
where RowNumber > @start and RowNumber <= @start + @count
order by RowNumber

GO

ALTER procedure [dbo].[spReorderAnnouncements] @schoolYearId int, @classAnnType int, @classId int
as
with AnnView as
               (
                select a.Id, Row_Number() over(order by a.Expires, a.[Created]) as [Order]  
                from Announcement a
                join Class c on c.Id = a.ClassRef
				where c.SchoolYearRef = @schoolYearId and a.ClassAnnouncementTypeRef = @classAnnType and a.ClassRef = @classId
               )
update Announcement
set [Order] = AnnView.[Order]
from AnnView 
where AnnView.Id = Announcement.Id
select  1
GO


ALTER PROCEDURE [dbo].[spDeleteAnnouncement] @id int, @personId int, @classId int, @state int, @classAnnouncementTypeId int
AS

declare @ann table
(
	Id int,
	SchoolYearId int,
	AnnouncementTypeId int,
	PersonId int,
	[State] int,
	ClassId int
)

insert into @ann(Id, SchoolYearId, AnnouncementTypeId, PersonId, [State], ClassId)
select a.Id, c.SchoolYearRef, a.ClassAnnouncementTypeRef, c.PrimaryTeacherRef, a.State, a.ClassRef
from Announcement a
join Class c on c.Id = a.ClassRef
where (@id is null or a.Id = @id)
	and (@personId is null or c.PrimaryTeacherRef = @personId or exists(select * from ClassTeacher ct where ct.PersonRef = @personId and ct.ClassRef = c.Id))
	and (@classId is null or a.ClassRef = @classId)
	and (@classAnnouncementTypeId is null or ClassAnnouncementTypeRef = @classAnnouncementTypeId)
	and (a.ClassRef is not null or a.ClassAnnouncementTypeRef is null  or a.[State] = 0)
	and (@state is null or a.[State] = @state)

delete from AnnouncementReminder where AnnouncementRef in (select Id from @ann)
/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef in (select Id from @ann)
/*DELETE AnnouncementApplication*/

/*DELETE AnnouncementRecipient*/
delete from AnnouncementRecipient
where AnnouncementRef in (select Id from @ann)
/*DELETE AnnouncementRecipientData*/
delete from AnnouncementRecipientData
where AnnouncementRef in (select Id from @ann)

delete from [Notification]
where AnnouncementRef in (select Id from @ann)

/*DELETE ANOUNCEMENTQNA*/
delete from AnnouncementQnA
where AnnouncementRef in (select Id from @ann)

/*DELETE AnnouncementApplication*/
delete from AnnouncementApplication
where AnnouncementRef in (select Id from @ann)

/*DELETE AnnouncementStandard*/
delete from AnnouncementStandard
where AnnouncementRef in (select Id from @ann)

/*DELETE Announcement*/
delete from Announcement where Id in (select Id from @ann)


declare @schoolYearId int

declare  AnnCursor cursor for
select SchoolYearId, AnnouncementTypeId, PersonId,  ClassId
from @ann

open AnnCursor
fetch next from AnnCursor
into @schoolYearId, @classAnnouncementTypeId, @personId, @classId

declare @oldSchoolYear int, @oldannouncementTypeId int,
@oldPersonId int, @oldClassId int

while @@FETCH_STATUS = 0
begin

if(@schoolYearId <> @oldSchoolYear or @classAnnouncementTypeId <> @oldannouncementTypeId
	or @personId <> @oldPersonId or @classId <> @oldClassId
	or (@oldSchoolYear is null and @oldannouncementTypeId is null
	and @oldPersonId is null and @oldClassId is null))
begin
/*Reordering Process*/
exec spReorderAnnouncements @schoolYearId, @classAnnouncementTypeId, @classId

set @oldannouncementTypeId = @classAnnouncementTypeId
set @oldClassId = @classId
set @oldPersonId = @personId
set @oldSchoolYear = @schoolYearId

end
fetch next from AnnCursor
into @schoolYearId, @classAnnouncementTypeId, @personId, @classId
end
CLOSE AnnCursor;
DEALLOCATE AnnCursor;
GO

ALTER procedure [dbo].[spGetAnnouncementsQnA] @callerId int, @announcementQnAId int
												, @announcementId int, @askerId int, @answererId int
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
	order by QuestionTime
GO


----------------------------
--- GET ANNOUNCEMENT DETAILS 
----------------------------
ALTER procedure [dbo].[spGetAnnouncementDetails] @id int, @callerId int, @callerRole int, @schoolId int
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
	ClassAnnouncementTypeName nvarchar(max),
	AnnouncementType int,
	ClassRef int,
	PrimaryTeacherName nvarchar(max),
	PrimaryTeacherGender nvarchar(max),
	ClassName nvarchar(max),
	GradeLevelId int,
	PrimaryTeacherRef int,
	QnACount int,
	StudentsCount int,
	AttachmentsCount int,
	OwnerAttachmentsCount int,
	StudentsCountWithAttachments int,
	ApplicationCount int,
	IsOwner bit,
	RecipientDataSchoolPersonId int,
	Starred bit,
	RowNumber bigint,

	--StarredCount int,
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
exec spGetStudentAnnouncements @id, @schoolId, @callerId, null, @callerRole, 0, 0, 0, null, null, null, 0, 1, null, null
end
if(@callerRole = 2)
begin
insert into @announcementTb
exec spGetTeacherAnnouncements @id, @schoolId, @callerId, null,  @callerRole, 0, 0, 0, null, null, null, 0, 1, null, 1, null
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

select @isOwner = a.IsOwner, @primaryTeacherId = a.PrimaryTeacherRef, @classId = a.ClassRef, @isGradebleType = cat.Gradable 
from @announcementTb a
join ClassAnnouncementType cat on cat.Id = a.ClassAnnouncementTypeRef 

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
exec spGetAnnouncementsQnA @callerId, null, @id, null, null


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

exec spGetPersons @schoolId, @primaryTeacherId, @callerId, null, 0, 1, null,null,null,null,null,null,null, 1, @callerRole, @markingPeriodId

select * from AnnouncementStandard 
join [Standard] on [Standard].Id = AnnouncementStandard.StandardRef
where AnnouncementStandard.AnnouncementRef = @id
end
GO


--------------------------
-- CREATE ANNOUNCEMENT 
-----------------------------
ALTER procedure [dbo].[spCreateAnnouncement] @schoolId int, @classAnnouncementTypeId int, @personId int, @created datetime2,
											  @expires datetime2, @state int, @gradingStyle int, @classId int
as
begin transaction
declare @callerRole int
select top 1 @callerRole = RoleRef from SchoolPerson where PersonRef = @personId

declare @announcementId int
--declare @markingPeriodClassId int;
--if(@classId is not null)
--	set @markingPeriodClassId = (select Id from MarkingPeriodClass where ClassRef = @classId and MarkingPeriodRef = @markingPeriodId)

declare @isDraft bit = 0

if @state = 0
begin
	select top 1 @announcementId = Id 
	from Announcement
	where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId) and [State] = 0
		  and (@classAnnouncementTypeId is null or ClassAnnouncementTypeRef = @classAnnouncementTypeId)
	order by Created desc

	if @announcementId is null
		select top 1 @announcementId = Id from Announcement
		where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId) and [State] = 0
		order by Created desc

	if @announcementId is not null
	update Announcement set [State] = -1 where Id = @announcementId
end

/*DELETE REMINDER*/
delete from AnnouncementReminder where AnnouncementRef IN (SELECT Id FROM Announcement 
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0)

/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef IN (SELECT Id FROM Announcement 
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0)

/*DELETE AnnouncementApplication*/
delete from AnnouncementApplication where AnnouncementRef IN (SELECT Id FROM Announcement
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0)

/*DELETE AnnouncementRecipientData*/
delete from AnnouncementRecipientData
where AnnouncementRef in (Select id from announcement 
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [state] = 0)

/*DELETE AnnouncementRecipient*/
delete from AnnouncementRecipient
where AnnouncementRef in (Select id from announcement 
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [state] = 0)

/*DELETE AnnouncementStandard*/
delete from AnnouncementStandard
where AnnouncementRef in (Select id from announcement 
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [state] = 0)

/*DELETE Announcement*/
delete from Announcement
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0

/*RESTORE STATE FOR DRAFT*/
if @announcementId is not null
begin
	update Announcement set [State] = 0 where Id = @announcementId
	set @isDraft = 1
end
else begin
	/*INSERT TO ANNOUNCEMENT*/
	insert into Announcement (Created, Expires, ClassAnnouncementTypeRef, [State],GradingStyle,[Order], ClassRef, Dropped, SchoolRef, MayBeDropped, VisibleForStudent)
	values(@created, @expires, @classAnnouncementTypeId, @state, @gradingStyle, 1, @classId, 0, @schoolId, 0, 1);
	set @announcementId = SCOPE_IDENTITY()
end


/*GET CONTENT FROM PREV ANNOUNCEMENT*/
declare @prevContent nvarchar(1024)
select top 1
@prevContent = Content from Announcement
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
	  and ClassAnnouncementTypeRef = @classAnnouncementTypeId
	  and [State] = 1
  	  and Content is not null
order by Created desc

update Announcement set Content = @prevContent where Id = @announcementId

commit 

if(@classAnnouncementTypeId is not null and @classId is not null)
begin 
	declare @schoolYearId int
	select @schoolYearId = Id from SchoolYear where @created between StartDate and EndDate
	declare @resT table (res int)
	insert into @resT
	exec [spReorderAnnouncements] @schoolYearId, @classAnnouncementTypeId,  @classId
end
exec spGetAnnouncementDetails @announcementId, @personId, @callerRole, @schoolId
GO


--Get Classes Procedure
ALTER procedure [dbo].[spGetClasses] @schoolId int, @schoolYearId int, @markingPeriodId int, @callerId int, @callerRoleId int,
										@personId int, @start int, @count int, @classId int, 
										@filter1 nvarchar(max), @filter2 nvarchar(max), @filter3 nvarchar(max)
as

declare @callerSchoolId int
declare @roleId int = (select top 1 RoleRef from SchoolPerson where PersonRef = @personId and SchoolRef = @schoolId)

declare @class table
(
	Class_Id int,
	Class_ClassNumber nvarchar(max),
	Class_Name nvarchar(max),
	Class_Description  nvarchar(max),
	Class_SchoolYearRef int,
	Class_PrimaryTeacherRef int,
	Class_GradeLevelRef int,
	Class_ChalkableDepartmentRef uniqueidentifier,
	Class_SchoolRef  int,
	Class_CourseRef int,
	GradeLevel_Id int,
	GradeLevel_Name nvarchar(max),
	GradeLevel_Number int,
	Person_Id int,
	Person_FirstName nvarchar(max),
	Person_LastName nvarchar(max),
	Person_Gender nvarchar(max),
	Person_Salutation nvarchar(max),
	Person_Email nvarchar(max),
	Person_AddressRef int,
	Person_RoleRef int,
	Class_SchoolId int,
	Class_StudentsCount int
)

select Count(*) as SourceCount
from vwClass
where (@classId is null or Class_Id = @classId)
and (@schoolYearId is null or Class_SchoolYearRef = @schoolYearId)
and (@markingPeriodId is null or exists(select * from MarkingPeriodClass where ClassRef = Class_Id and MarkingPeriodRef = @markingPeriodId))

and (@callerRoleId = 1 or ((Class_SchoolId = @schoolId) and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2
	or (@callerRoleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @callerId))
)))
and (@personId is null or (@roleId = 2 and exists(select * from ClassTeacher where ClassRef = Class_Id and PersonRef = @personId))
		or @roleId = 5 or  @roleId = 7 or  @roleId = 8 or @roleId = 1
		or (@roleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @personId))
	)
and
(
	(@filter1 is null and @filter2 is null and @filter3 is null) or
	(@filter1 is not null and Class_Name like @filter1 or
		@filter2 is not null and Class_Name like @filter2 or
		@filter3 is not null and Class_Name like @filter3)
)


insert into @class
select vwClass.*, (select count(*) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
from vwClass
where
(@classId is null or Class_Id = @classId)
and (@schoolYearId is null or Class_SchoolYearRef = @schoolYearId)
and (@markingPeriodId is null or exists(select * from MarkingPeriodClass where ClassRef = Class_Id and MarkingPeriodRef = @markingPeriodId))

and (@callerRoleId = 1 or ((Class_SchoolId = @schoolId) and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2
	or (@callerRoleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @callerId))
)))
and (@personId is null or (@roleId = 2 and exists(select * from ClassTeacher where ClassRef = Class_Id and PersonRef = @personId))
or @roleId = 5 or  @roleId = 7 or  @roleId = 8 or @roleId = 1
or (@roleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @personId))

)
and
(
(@filter1 is null and @filter2 is null and @filter3 is null) or
(@filter1 is not null and Class_Name like @filter1 or
@filter2 is not null and Class_Name like @filter2 or
@filter3 is not null and Class_Name like @filter3)
)
order by vwClass.Class_Id
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY


select * from @class

select mpc.* from MarkingPeriodClass mpc
join @class c on c.Class_Id = mpc.ClassRef
GO

--------------------

ALTER PROCEDURE [dbo].[spGetPersonsForApplicationInstall]
	@applicationId uniqueidentifier, @callerId int, @personId int, @roleIds nvarchar(2048), @departmentIds nvarchar(2048)
	, @gradeLevelIds nvarchar(2048), @classIds nvarchar(2048), @callerRoleId int
	, @hasAdminMyApps bit, @hasTeacherMyApps bit, @hasStudentMyApps bit, @canAttach bit, @schoolYearId int
as
declare @roleIdsT table(value int);
declare @departmentIdsT table(value uniqueidentifier);
declare @gradeLevelIdsT table(value int);
declare @classIdsT table(value int);


PRINT('application id');
PRINT(@applicationId);
PRINT('caller id');
PRINT(@callerId);
PRINT('person id');
PRINT(@personId );
PRINT('caller role id');
PRINT(@callerRoleId);

if(@roleIds is not null)
begin
	insert into @roleIdsT(value)
	select cast(s as int) from dbo.split(',', @roleIds) 
end
if(@departmentIds is not null)
begin
	insert into @departmentIdsT(value)
	select cast(s as uniqueidentifier) from dbo.split(',', @departmentIds)
end
if(@gradeLevelIds is not null)
begin
	insert into @gradeLevelIdsT(value)
	select cast(s as int) from dbo.split(',', @gradeLevelIds)
end
if(@classIds is not null)
begin
	insert into @classIdsT(value)
	select cast(s as int) from dbo.split(',', @classIds)
end

declare @canInstallForTeahcer bit = @hasTeacherMyApps | @canAttach
declare @canInstallForStudent bit = @hasStudentMyApps | @canAttach

declare @canInstall bit = 0
if (
	(@callerRoleId = 3 and @hasStudentMyApps = 1) 
	or (@callerRoleId = 2 and (@canInstallForStudent = 1 or @canInstallForTeahcer = 1))
	or ((@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8) 
		 and (@hasAdminMyApps = 1 or @canInstallForStudent = 1 or @canInstallForTeahcer = 1)
	   )
   )
set @canInstall = 1


declare @schoolId int = (select SchoolRef from SchoolYear where Id = @schoolYearId)

declare @preResult table
(
	[Type] int not null,
	GroupId nvarchar(256) not null,
	PersonId int not null
);
declare @localPersons table
(
	Id int not null,
	RoleRef int not null
);
--select sp due to security
if @canInstall = 1
begin
	if @callerRoleId = 3
	begin
		insert into @localPersons (Id, RoleRef)
		select PersonRef, RoleRef 
		from SchoolPerson
		where PersonRef = @callerId and SchoolRef = @schoolId and @hasStudentMyApps = 1
	end
	if  @callerRoleId = 2
	begin
		insert into @localPersons (Id, RoleRef)
		select PersonRef, RoleRef 
		from SchoolPerson
		where ((@canInstallForStudent = 1 and PersonRef in (select ClassPerson.PersonRef
															from ClassPerson
															join ClassTeacher on ClassPerson.ClassRef = ClassTeacher.ClassRef
															where ClassTeacher.PersonRef = @callerId
									  					   )
			   ) 
			   or (PersonRef = @callerId and @canInstallForTeahcer = 1))
			   and SchoolRef = @schoolId
	end
	if @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8
	begin
		insert into @localPersons (Id, RoleRef)
		select PersonRef, RoleRef 
		from SchoolPerson
		where ((RoleRef = 2 and @canInstallForTeahcer = 1)
				or ((RoleRef = 5 or RoleRef = 7 or RoleRef = 8) and @hasAdminMyApps = 1)
				or (RoleRef = 3 and @canInstallForStudent = 1))
			  and SchoolRef = @schoolId
	end
end

delete from @localPersons where Id in (select PersonRef from ApplicationInstall where ApplicationRef = @applicationId and Active = 1)
/*
TYPES:
0 - role
1 - department
2 - grade level
3 - class
4 - person
*/
--insert by roles
if @roleIds is not null
Insert into @preResult
([type], groupId, PersonId)
select 0, cast(RoleRef as nvarchar(256)), Id
from @localPersons
where RoleRef in (select Value from @roleIdsT)
--insert by departments
if @departmentIds is not null
begin
	Insert into @preResult ([type], groupId, PersonId)
	select distinct 1, cast(d.Value as nvarchar(256)), sp.Id
	from @localPersons as sp
	join ClassPerson as csp on sp.Id = csp.PersonRef
	join Class c on csp.ClassRef = c.Id
	join @departmentIdsT d on c.ChalkableDepartmentRef = d.value


	Insert into @preResult ([type], groupId, PersonId)
	select distinct 1, cast(d.Value as nvarchar(256)), sp.Id
	from @localPersons as sp
	join ClassTeacher as ct on ct.PersonRef = sp.Id
	join Class c on c.Id = ct.ClassRef
	join @departmentIdsT d on c.ChalkableDepartmentRef = d.value
end
--insert by grade level
if @gradeLevelIds is not null
begin
	Insert into @preResult
	([type], groupId, PersonId)
	select distinct 2, cast(gl.Value as nvarchar(256)), sp.Id
	from @localPersons as sp
	join StudentSchoolYear as ssy on sp.Id = ssy.StudentRef and ssy.SchoolYearRef = @schoolYearId
	join @gradeLevelIdsT gl on ssy.GradeLevelRef = gl.value 
	
	Insert into @preResult
	([type], groupId, PersonId)
	select distinct 2, cast(gl.Value as nvarchar(256)), sp.Id
	from @localPersons as sp
	join ClassTeacher as ct on ct.PersonRef = sp.Id
	join Class c on c.Id = ct.ClassRef
	join @gradeLevelIdsT gl on c.GradeLevelRef = gl.value
end
--insert by class
if @classIds is not null
begin
	Insert into @preResult
	([type], groupId, PersonId)
	select distinct 3, cast(cc.Value as nvarchar(256)), sp.Id
	from @localPersons as sp
	join ClassPerson csp on csp.PersonRef = sp.Id
	join @classIdsT cc on csp.ClassRef = cc.value

	if @callerRoleId <> 2
	begin
		Insert into @preResult ([type], groupId, PersonId)
		select distinct 3, cast(cc.Value as nvarchar(256)), sp.Id
		from @localPersons as sp
		join ClassTeacher ct on sp.Id = ct.PersonRef
		join @classIdsT cc on ct.PersonRef = cc.value
	end
end

declare @isSinglePerson bit = 0
if @personId is null and @callerRoleId = 2
begin
	set @personId = @callerId
	set @isSinglePerson = 1
end

--insert by sp
if @personId is not null
	Insert into @preResult([type], groupId, PersonId)
	select 4, cast(Id as nvarchar(256)), Id
	from @localPersons
	where id = @personId



if @roleIds is null and @departmentIds is null and @gradeLevelIds is null 
	and @classIds is null and (@isSinglePerson = 1 or @personId is null)
	
	Insert into @preResult
	([type], groupId, PersonId)
	select 4, cast(Id as nvarchar(256)), Id
	from @localPersons

select * 
from @preResult
GO


ALTER PROCEDURE [dbo].[spGetStudentCountToAppInstallByClass]
@applicationId uniqueidentifier, @schoolYearId int, @personId int, @roleId int
as

declare @emptyResult bit = 0;

select
	Class.Id as ClassId,
	Class.Name as ClassName,
	Count(*) as NotInstalledStudentCount
from Class
join ClassPerson on ClassPerson.ClassRef = Class.Id
left join (select * from ApplicationInstall where ApplicationRef = @applicationId and Active = 1) x
on x.PersonRef = ClassPerson.PersonRef and x.SchoolYearRef = Class.SchoolYearRef
where
	@emptyResult = 0
	and Class.SchoolYearRef = @schoolYearId
	and (@roleId = 8 or @roleId = 7 or @roleId = 5 or (@roleId = 2 and (Class.PrimaryTeacherRef = @personId or 
																		exists(select * from ClassTeacher ct 
																			   where ct.PersonRef = @personId and ct.ClassRef = Class.Id) 
																		)
													   )
		)
	and x.Id is null
group by
Class.Id, Class.Name
GO

---------------
----GetPersons
ALTER PROCEDURE [dbo].[spGetPersons] @schoolId int, @personId int, @callerId int, @roleId int, @start int, @count int, @startFrom nvarchar(255)
	, @teacherId int, @classId int, @filter1 nvarchar(255), @filter2 nvarchar(255), @filter3 nvarchar(255)
	, @gradeLevelIds nvarchar(1024), @sortType int, @callerRoleId int, @markingPeriodId int

with recompile
as


declare @glIds table 
(
	id int not null
) 
if @gradeLevelIds is not null
begin
	insert into @glIds
	select cast(s as int) from dbo.split(',', @gradeLevelIds)
end

declare @callerGradeLevelId int = null;
if(@callerRoleId = 3)
begin
	-- todo : needs currentSchoolYearId for getting right grade level 
	set @callerGradeLevelId = (select  top 1 GradeLevelRef from StudentSchoolYear where StudentRef = @callerId)
end


select COUNT(*) as AllCount from vwPerson
where
	(@personId is null or Id = @personId)
	and (@roleId is null or RoleRef = @roleId)
	and (@startFrom is null or LastName >= @startFrom)
	and (@teacherId is null or 
		Id in (select ClassPerson.PersonRef from ClassPerson 
				join ClassTeacher on ClassPerson.ClassRef = ClassTeacher.ClassRef 
				where ClassTeacher.PersonRef = @teacherId and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)
							))
	and (@classId is null or ((@roleId is null or @roleId = 3) and Id in (select PersonRef from ClassPerson 
																	      where ClassPerson.ClassRef = @classId
																			    and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)
																		 )
							  )
						  or ((@roleId is null or @roleId = 2) and Id in (select PersonRef from ClassTeacher where ClassRef = @classId))
		)
	and (@callerRoleId = 1 or (vwPerson.SchoolRef = @schoolId and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
			or(@callerRoleId = 3 and (Id = @callerId   or (RoleRef = 2 or RoleRef = 5 or RoleRef = 7 or RoleRef = 8 
											   or (RoleRef = 3 and exists(select * from StudentSchoolYear where StudentRef = vwPerson.Id and GradeLevelRef = @callerGradeLevelId))))
			   )
			or(@callerRoleId = 6 and (Id = @callerId or RoleRef = 3))
		)))	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
		 or FirstName like @filter2 or LastName like @filter2
		 or FirstName like @filter3 or LastName like @filter3)
	and (
			@gradeLevelIds is null 
			or (vwPerson.RoleRef = 3 and exists(select * from StudentSchoolYear ssy
												join @glIds gl on gl.id = ssy.GradeLevelRef
												where ssy.StudentRef = vwPerson.Id))
			or (vwPerson.RoleRef = 2 and exists
				(select * from Class 
				 join ClassTeacher ct on ct.ClassRef = Class.Id
				 where ct.PersonRef = vwPerson.Id and Class.GradeLevelRef in (select id from @glIds))
			)
		)

-- Sort Type
-- 0 : by FisrtName
-- 1 : by LastName
--------------------			
					
select vwPerson.*
from vwPerson
where
	(@personId is null or  Id = @personId)
	and (@roleId is null or RoleRef = @roleId)
	and (@startFrom is null or LastName >= @startFrom)
	and (@teacherId is null or 
		Id in (select ClassPerson.PersonRef from ClassPerson 
			   join ClassTeacher on ClassPerson.ClassRef = ClassTeacher.ClassRef 
			   where ClassTeacher.PersonRef = @teacherId and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)
							))
	and (@classId is null or ((@roleId is null or @roleId = 3) and Id in (select PersonRef from ClassPerson 
																		 where ClassPerson.ClassRef = @classId 
																		 and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)))
						  or ((@roleId is null or @roleId = 2) and Id in (select PersonRef from ClassTeacher where ClassRef = @classId))
		)
	and (@callerRoleId = 1 or (vwPerson.SchoolRef = @schoolId and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
			or(@callerRoleId = 3 and (Id = @callerId 
											or (RoleRef = 2 or RoleRef = 5 or RoleRef = 7 or RoleRef = 8 
												or (RoleRef = 3 and exists(select * from StudentSchoolYear where StudentRef = vwPerson.Id and GradeLevelRef = @callerGradeLevelId))))
				)
			or(@callerRoleId = 6 and (Id = @callerId or RoleRef = 3))
		)))	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
			or FirstName like @filter2 or LastName like @filter2
			or FirstName like @filter3 or LastName like @filter3)
	and (
			@gradeLevelIds is null 
			or (vwPerson.RoleRef = 3 and exists(select * from StudentSchoolYear ssy
												join @glIds gl on gl.id = ssy.GradeLevelRef
												where ssy.StudentRef = vwPerson.Id))
			or (vwPerson.RoleRef = 2 and exists
				(select * from Class 
				 join ClassTeacher ct on ct.ClassRef = Class.Id
				 where ct.PersonRef = vwPerson.Id and Class.GradeLevelRef in (select id from @glIds))
			)
		)
						
order by  case when @sortType = 0 then FirstName  else LastName end
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
GO



