


---------------
----GetPersons
---------------
alter PROCEDURE [dbo].[spGetPersons] @schoolId int, @personId int, @callerId int, @roleId int, @start int, @count int, @startFrom nvarchar(255)
	, @teacherId int, @classId int, @filter1 nvarchar(255), @filter2 nvarchar(255), @filter3 nvarchar(255)
	, @gradeLevelIds nvarchar(1024), @sortType int, @callerRoleId int, @markingPeriodId int, @schoolYearId int, @isEnrolled bit, @onlyMyTeachers bit

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

-------Enrollment Status--------------
-- CurrentlyEnrolled = 0,
-- PreviouslyEnrolled = 1
--------------------------------------

declare @enrollmentStatus int;
if @isEnrolled is not null
	if @isEnrolled = 1 set @enrollmentStatus = 0
	else set @enrollmentStatus = 1

declare @callerGradeLevelId int = null;
if(@callerRoleId = 3)
begin
	-- todo : needs currentSchoolYearId for gettin\g right grade level 
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
				where ClassTeacher.PersonRef = @teacherId 
					  and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)
					  and (@isEnrolled is null or ClassPerson.IsEnrolled = @isEnrolled)
							))
	and (@classId is null or ((@roleId is null or @roleId = 3) and Id in (select PersonRef from ClassPerson 
																	      where ClassPerson.ClassRef = @classId
																			    and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)
																				and (@isEnrolled is null or ClassPerson.IsEnrolled = @isEnrolled)
																		 )
							  )
						  or ((@roleId is null or @roleId = 2) and Id in (select PersonRef from ClassTeacher where ClassRef = @classId))
		)
	and (@callerRoleId = 1 or (vwPerson.SchoolRef = @schoolId and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
			or(@callerRoleId = 3 and (Id = @callerId   or (
											   (RoleRef = 2 and (@onlyMyTeachers = 0 or exists(select * from ClassTeacher 
																							   join ClassPerson on ClassPerson.ClassRef = ClassTeacher.ClassRef
																							   where ClassPerson.PersonRef = @callerId and ClassTeacher.PersonRef = vwPerson.Id)))
											   or RoleRef = 5 or RoleRef = 7 or RoleRef = 8 
											   or (RoleRef = 3 and exists(select * from StudentSchoolYear
																		  where StudentRef = vwPerson.Id and GradeLevelRef = @callerGradeLevelId and (@schoolYearId is null or SchoolYearRef = @schoolYearId)
																		  and (@enrollmentStatus is null or EnrollmentStatus = @enrollmentStatus)))))
			   )
			or(@callerRoleId = 6 and (Id = @callerId or RoleRef = 3))
		)))	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
		 or FirstName like @filter2 or LastName like @filter2
		 or FirstName like @filter3 or LastName like @filter3)
	and (
			(vwPerson.RoleRef = 3 and exists(select * from StudentSchoolYear ssy 
												where ssy.StudentRef = vwPerson.Id  
													and (@schoolYearId is null or SchoolYearRef = @schoolYearId)
													and (@gradeLevelIds is null or ssy.GradeLevelRef in (select gl.id from @glIds gl))
													and (@enrollmentStatus is null or EnrollmentStatus = @enrollmentStatus)
												)
			)
			or (vwPerson.RoleRef = 2 and
				(@gradeLevelIds is null or exists (select * from Class 
												   join ClassTeacher ct on ct.ClassRef = Class.Id
												   where ct.PersonRef = vwPerson.Id and Class.GradeLevelRef in (select id from @glIds))
				))
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
				    and (@isEnrolled is null or ClassPerson.IsEnrolled = @isEnrolled)))

	and (@classId is null or ((@roleId is null or @roleId = 3) and Id in (select PersonRef from ClassPerson 
																		 where ClassPerson.ClassRef = @classId 
																		 and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)
																		 and (@isEnrolled is null or ClassPerson.IsEnrolled = @isEnrolled)
																		 ))
						  or ((@roleId is null or @roleId = 2) and Id in (select PersonRef from ClassTeacher where ClassRef = @classId))
		)
	and (@callerRoleId = 1 or (vwPerson.SchoolRef = @schoolId and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
			or(@callerRoleId = 3 and (Id = @callerId 
											or ((RoleRef = 2 and (@onlyMyTeachers = 0 or exists(select * from ClassTeacher 
																		join ClassPerson on ClassPerson.ClassRef = ClassTeacher.ClassRef
																		where ClassPerson.PersonRef = @callerId and ClassTeacher.PersonRef = vwPerson.Id)))
											    or RoleRef = 5 or RoleRef = 7 or RoleRef = 8 
												or (RoleRef = 3 and exists(select * from StudentSchoolYear
																		  where StudentRef = vwPerson.Id and GradeLevelRef = @callerGradeLevelId 
																				 and (@schoolYearId is null or SchoolYearRef = @schoolYearId)
																				 and (@enrollmentStatus is null or EnrollmentStatus = @enrollmentStatus)
																		  ))))
				)
			or(@callerRoleId = 6 and (Id = @callerId or RoleRef = 3))
		)))	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
			or FirstName like @filter2 or LastName like @filter2
			or FirstName like @filter3 or LastName like @filter3)
	and (
			(vwPerson.RoleRef = 3 and exists(select * from StudentSchoolYear ssy 
												where ssy.StudentRef = vwPerson.Id  
													and (@schoolYearId is null or SchoolYearRef = @schoolYearId)
													and (@gradeLevelIds is null or ssy.GradeLevelRef in (select gl.id from @glIds gl))
													and (@enrollmentStatus is null or EnrollmentStatus = @enrollmentStatus)
												)
			)
			or (vwPerson.RoleRef = 2 and
				(@gradeLevelIds is null or exists (select * from Class 
												   join ClassTeacher ct on ct.ClassRef = Class.Id
												   where ct.PersonRef = vwPerson.Id and Class.GradeLevelRef in (select id from @glIds))
				))
		)
						
order by  case when @sortType = 0 then FirstName  else LastName end
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
GO

alter PROCEDURE [dbo].[spGetPersonDetails] @schoolId int, @personId int, @callerId int, @callerRoleId int
as
exec spGetPersons @schoolId, @personId, @callerId, null, 0, 1, null, null, null, null,null,null,null, 0, @callerRoleId, null, null, null, 0

select top 1 a.* from [Address] a
join Person p on p.AddressRef = a.Id
where p.Id = @personId

select * from Phone
where PersonRef = @personId

select *
from StudentSchoolYear 
join GradeLevel on GradeLevel.Id = StudentSchoolYear.GradeLevelRef
where StudentSchoolYear.StudentRef = @personId
GO


--------------------------
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

exec spGetPersons @schoolId, @primaryTeacherId, @callerId, null, 0, 1, null,null,null,null,null,null,null, 1, @callerRole, @markingPeriodId, null, null, 0

select * from AnnouncementStandard 
join [Standard] on [Standard].Id = AnnouncementStandard.StandardRef
where AnnouncementStandard.AnnouncementRef = @id
end

GO


