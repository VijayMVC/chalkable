create View [dbo].[vwAnnouncement] 
as 
Select 
	Announcement.Id as Id,
	Announcement.Created as Created,
	Announcement.Expires as Expires,
	Announcement.[State] as [State],
	Announcement.[Order] as [Order],
	Announcement.Content as Content,
	Announcement.[Subject] as [Subject],
	Announcement.GradingStyle as GradingStyle,
	Announcement.Dropped as Dropped,
	Announcement.AnnouncementTypeRef as AnnouncementTypeRef,
	AnnouncementType.Name as AnnouncementTypeName,
	Announcement.PersonRef as PersonRef,
	Announcement.MarkingPeriodClassRef as MarkingPeriodClassRef,
	Person.FirstName + ' ' + Person.LastName as PersonName,
	Person.Gender as PersonGender,
	Class.Name as ClassName,
	Class.GradeLevelRef as GradeLevelId,  
	Class.CourseInfoRef as CourseId,
	MarkingPeriodClass.ClassRef as ClassId,
	MarkingPeriodClass.MarkingPeriodRef as MarkingPeriodId,
	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = Announcement.Id) as QnACount,	
	(Select COUNT(*) from ClassPerson where ClassRef = MarkingPeriodClass.ClassRef) as StudentsCount,
	(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id) as AttachmentsCount,
	(select count(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id and PersonRef = Announcement.PersonRef) as OwnerAttachmentsCount,
	(	select COUNT(*) from
			(Select distinct PersonRef from AnnouncementAttachment 
			where AnnouncementRef = Announcement.Id
			and PersonRef <> Announcement.PersonRef) as x
	) as StudentsCountWithAttachments,
	(Select COUNT(*) from StudentAnnouncement where AnnouncementRef = Announcement.Id and GradeValue is not null) as GradingStudentsCount, 
	(Select AVG(GradeValue) from StudentAnnouncement where AnnouncementRef = Announcement.Id and GradeValue is not null) as [Avg], 
	(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = Announcement.Id and Active = 1) as ApplicationCount
	--(Select top 1 (case when COUNT(aa.Id) = 1 then a.Name else null end) 
	--		from AnnouncementApplication aa 
	--		join Application a on  aa.ApplicationRef = a.Id 
	--		where aa.AnnouncementRef = Announcement.Id and aa.Active = 1
	--		group by a.Name) as ApplicationName
	--(select count(aa.Id) from AnnouncementApplication aa where aa.AnnouncementRef = Announcement.Id) as ApplicationName

from 
	Announcement
	join AnnouncementType on Announcement.AnnouncementTypeRef = AnnouncementType.Id
	left join MarkingPeriodClass on MarkingPeriodClass.Id = Announcement.MarkingPeriodClassRef
	left join Class on Class.Id = MarkingPeriodClass.ClassRef
	left join Person on Person.Id = Announcement.PersonRef
GO

create procedure [dbo].[spGetStudentAnnouncements]  
	@id uniqueidentifier, @personId uniqueidentifier, @classId uniqueidentifier,  @roleId int, @staredOnly bit, @ownedOnly bit,  @gradedOnly bit
	, @fromDate DateTime2, @toDate DateTime2, @markingPeriodId uniqueidentifier
	, @start int, @count int, @now DateTime2
as 

exec spApplyStarringAnnouncementForStudent @personId, @now;
declare @gradeLevelRef uniqueidentifier = (select GradeLevelRef from StudentInfo where Id = @personId)

declare @allCount int = (select COUNT(*) from
	vwAnnouncement	
	left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
		 on vwAnnouncement.Id = ard.AnnouncementRef
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and(@classId is null or ClassId = @classId)
	and (vwAnnouncement.PersonRef = @personId 
		or (AnnouncementTypeRef = 11 
			and exists
				(
						select * from AnnouncementRecipient 
						where AnnouncementRef = vwAnnouncement.Id 
						and (ToAll = 1 or PersonRef = @personId or (RoleRef = @roleId and (@roleId <> 3 or GradeLevelRef is null or GradeLevelRef = @gradeLevelRef)))
				)
			)
		or exists(select * from ClassPerson where ClassRef = ClassId and PersonRef = @personId)
	)			
	and (@staredOnly = 0 or Starred = 1)
	and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or MarkingPeriodId = @markingPeriodId)
	and (@gradedOnly = 0 or (GradingStudentsCount > 0 and vwAnnouncement.Id in (select sa.AnnouncementRef from StudentAnnouncement sa 
																					 join ClassPerson csp on csp.Id = sa.ClassPersonRef
																					 where  csp.PersonRef = @personId and sa.GradeValue is not null)))
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
		and(@classId is null or ClassId = @classId)
		and (vwAnnouncement.PersonRef = @personId 
			or (AnnouncementTypeRef = 11 
				and exists
					(
							select * from AnnouncementRecipient 
							where AnnouncementRef = vwAnnouncement.Id 
							and (ToAll = 1 or PersonRef = @personId or (RoleRef = @roleId and (@roleId <> 3 or GradeLevelRef is null or GradeLevelRef = @gradeLevelRef)))
					)
				)
			or exists(select * from ClassPerson where ClassRef = ClassId and PersonRef = @personId)
		)			
		and (@staredOnly = 0 or Starred = 1)
		and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or MarkingPeriodId = @markingPeriodId)
		and (@gradedOnly = 0 or (GradingStudentsCount > 0 and vwAnnouncement.Id in (select sa.AnnouncementRef from StudentAnnouncement sa 
																					 join ClassPerson csp on csp.Id = sa.ClassPersonRef
																					 where  csp.PersonRef = @personId and sa.GradeValue is not null)))
		)


select *
from
	(
	Select 
		vwAnnouncement.*,
		cast((case vwAnnouncement.PersonRef when @personId then 1 else 0 end) as bit) as IsOwner,
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
		and(@classId is null or ClassId = @classId)
		and (vwAnnouncement.PersonRef = @personId 
			or (AnnouncementTypeRef = 11 
				and exists
					(
							select * from AnnouncementRecipient 
							where AnnouncementRef = vwAnnouncement.Id 
							and (ToAll = 1 or PersonRef = @personId or (RoleRef = @roleId and (@roleId <> 3 or GradeLevelRef is null or GradeLevelRef = @gradeLevelRef)))
					)
				)
			or exists(select * from ClassPerson where ClassRef = ClassId and PersonRef = @personId)
		)			
		and (@staredOnly = 0 or Starred = 1)
		and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or MarkingPeriodId = @markingPeriodId)
		and (@gradedOnly = 0 or (GradingStudentsCount > 0 and vwAnnouncement.Id in (select sa.AnnouncementRef from StudentAnnouncement sa 
																					 join ClassPerson csp on csp.Id = sa.ClassPersonRef
																					 where  csp.PersonRef = @personId and sa.GradeValue is not null)))
	union 
	Select 
		vwAnnouncement.*,
		cast((case vwAnnouncement.PersonRef when @personId then 1 else 0 end) as bit) as IsOwner,
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
		and(@classId is null or ClassId = @classId)
		and (vwAnnouncement.PersonRef = @personId 
			or (AnnouncementTypeRef = 11 
				and exists
					(
							select * from AnnouncementRecipient 
							where AnnouncementRef = vwAnnouncement.Id 
							and (ToAll = 1 or PersonRef = @personId or (RoleRef = @roleId and (@roleId <> 3 or GradeLevelRef is null or GradeLevelRef = @gradeLevelRef)))
					)
				)
			or exists(select * from ClassPerson where ClassRef = ClassId and PersonRef = @personId)
		)			
		and (@staredOnly = 0 or Starred = 1)
		and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or MarkingPeriodId = @markingPeriodId)	
		and (@gradedOnly = 0 or (GradingStudentsCount > 0 and vwAnnouncement.Id in (select sa.AnnouncementRef from StudentAnnouncement sa 
																					 join ClassPerson csp on csp.Id = sa.ClassPersonRef
																					 where  csp.PersonRef = @personId and sa.GradeValue is not null)))
	) x
where RowNumber > @start and RowNumber <= @start + @count
order by RowNumber
GO

create procedure [dbo].[spGetTeacherAnnouncements]  
	@id uniqueidentifier, @personId uniqueidentifier, @classId uniqueidentifier, @roleId int, @staredOnly bit, @ownedOnly bit, @gradedOnly bit
	,@fromDate DateTime2, @toDate DateTime2, @markingPeriodId uniqueidentifier, @start int, @count int, @now DateTime2, @allSchoolItems bit
as 

exec spApplyStarringAnnouncementForTeacher @personId, @now;

declare @gradeLevelsT table(Id uniqueidentifier)
insert into @gradeLevelsT(Id)
select GradeLevelRef from Class
where TeacherRef = @personId
group by GradeLevelRef 

declare @allCount int;
set @allCount = (select COUNT(*) from
	vwAnnouncement	
	left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
		 on vwAnnouncement.Id = ard.AnnouncementRef
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@classId is null or ClassId = @classId)
	and ((@allSchoolItems = 1 and @roleId = 2) or vwAnnouncement.PersonRef = @personId 
		or (AnnouncementTypeRef = 11 
				and exists(select AnnouncementRecipient.Id from AnnouncementRecipient
						   where AnnouncementRef = vwAnnouncement.Id  and (ToAll = 1 or PersonRef = @personId 
								or (RoleRef = @roleId and (@roleId <> 2 or GradeLevelRef is null or GradeLevelRef in (select Id from @gradeLevelsT))))
						   )
		    )
		)			
	and (@staredOnly = 0 or Starred = 1)
	and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or MarkingPeriodId = @markingPeriodId)
)

select * from
	(
	Select 
		vwAnnouncement.*,
		cast((case vwAnnouncement.PersonRef when @personId then 1 else 0 end) as bit) as IsOwner,
		ard.PersonRef as RecipientDataPersonId,
		ard.Starred as Starred,
		ROW_NUMBER() OVER(ORDER BY vwAnnouncement.Created desc) as RowNumber,
		--@starredCount as StarredCount,
	 	 @allCount as AllCount
	from 
		vwAnnouncement	
		left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
			 on vwAnnouncement.Id = ard.AnnouncementRef
	where
		(@id is not null  or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@classId is null or ClassId = @classId)
		and ((@allSchoolItems = 1 and @roleId = 2) or vwAnnouncement.PersonRef = @personId 
			or (AnnouncementTypeRef = 11 
				and exists(select AnnouncementRecipient.Id from AnnouncementRecipient
						   where AnnouncementRef = vwAnnouncement.Id  and (ToAll = 1 or PersonRef = @personId 
								or (RoleRef = @roleId and (@roleId <> 2 or GradeLevelRef is null or GradeLevelRef in (select Id from @gradeLevelsT))))
						   )
				)
			)			
		and (@staredOnly = 0 or Starred = 1)
		and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or MarkingPeriodId = @markingPeriodId)
		and (@gradedOnly = 0 or GradingStudentsCount > 0)		
	) x
where RowNumber > @start and RowNumber <= @start + @count
order by Created desc

GO

create procedure [dbo].[spGetAdminAnnouncements]  
	@id uniqueidentifier, @personId uniqueidentifier, @classId uniqueidentifier, @roleId int, @staredOnly bit, @ownedOnly bit
	,@fromDate DateTime2, @toDate DateTime2, @markingPeriodId uniqueidentifier, @start int, @count int, @now DateTime2
	,@gradeLevelsIds nvarchar(256) 
as 

declare @allCount int;
declare @gradeLevelsIdsT table(value uniqueidentifier);
if(@gradeLevelsIds is not null)
begin
	insert into @gradeLevelsIdsT(value)
	select cast(s as uniqueidentifier) from [Split](',', @gradeLevelsIds)
end


set @allCount = (select COUNT(*) from
	vwAnnouncement	
	left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
		 on vwAnnouncement.Id = ard.AnnouncementRef
where
	(@id is not null  or [State] = 1) and
	(@id is null or vwAnnouncement.Id = @id)
	and (@classId is null or ClassId = @classId)
	and (@staredOnly = 0 or Starred = 1)
	and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@markingPeriodId is null or MarkingPeriodId = @markingPeriodId)
	and (@gradeLevelsIds is null or vwAnnouncement.Id in (
														  select ar.AnnouncementRef from AnnouncementRecipient ar
														  left join @gradeLevelsIdsT glT on glT.value = ar.GradeLevelRef
														  where  glT.value is not null or ar.ToAll = 1 or ar.RoleRef = 3 or ar.RoleRef = 2
														  )
		)
)
select * from
	(
	Select 
		vwAnnouncement.*,
		cast((case vwAnnouncement.PersonRef when @personId then 1 else 0 end) as bit) as IsOwner,
		ard.PersonRef as RecipientDataPersonId,
		ard.Starred as Starred,
		ROW_NUMBER() OVER(ORDER BY vwAnnouncement.Created desc) as RowNumber,
		--0 as StarredCount,
		@allCount as AllCount
	from 
		vwAnnouncement	
		left join (select * from AnnouncementRecipientData where AnnouncementRecipientData.PersonRef = @personId) ard
			 on vwAnnouncement.Id = ard.AnnouncementRef
	where
		(@id is not null or [State] = 1) and
		(@id is null or vwAnnouncement.Id = @id)
		and (@classId is null or ClassId = @classId)
		and (@staredOnly = 0 or Starred = 1)
		and (@ownedOnly = 0 or vwAnnouncement.PersonRef = @personId)
		and (@fromDate is null or Expires >= @fromDate)
		and (@toDate is null or Expires <= @toDate)
		and (@markingPeriodId is null or MarkingPeriodId = @markingPeriodId)
		and (@gradeLevelsIds is null or vwAnnouncement.Id in (
															  select ar.AnnouncementRef from AnnouncementRecipient ar
															  left join @gradeLevelsIdsT glT on glT.value = ar.GradeLevelRef
															  where glT.value is not null or ar.ToAll = 1 or ar.RoleRef = 3 or ar.RoleRef = 2
														     )
		)		
	) x
where RowNumber > @start and RowNumber <= @start + @count
order by Created desc
GO

