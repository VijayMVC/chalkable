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


/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef in (select Id from @ann)
/*DELETE AnnouncementApplication*/

/*DELETE AnnouncementRecipient*/
delete from AnnouncementRecipient
where AnnouncementRef in (select Id from @ann)

delete from [Notification]
where AnnouncementRef in (select Id from @ann)

/*DELETE ANOUNCEMENTQNA*/
delete from AnnouncementQnA
where AnnouncementRef in (select Id from @ann)

/*DELETE AutoGrade*/
delete from AutoGrade
where AnnouncementApplicationRef IN 
	(SELECT AnnouncementApplication.Id from AnnouncementApplication
	 where AnnouncementRef in (select Id from @ann))


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


-------------------------
-- CREATE ANNOUNCEMENT 
-----------------------------
alter procedure [dbo].[spCreateAnnouncement] @schoolId int, @classAnnouncementTypeId int, @personId int, @created datetime2,
											  @expires datetime2, @state int, @gradingStyle int, @classId int
as
begin transaction
--- Only teacher can create announcement ---
--- Maybe we will change this in future ---
declare @callerRole int = 2

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


/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef IN (SELECT Id FROM Announcement 
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0)

/*DELETE AutoGrades*/
delete from AutoGrade
where AnnouncementApplicationRef IN (SELECT AnnouncementApplication.Id from AnnouncementApplication
		join Announcement on Announcement.Id = AnnouncementRef
		where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
		AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
		AND [State] = 0)

/*DELETE AnnouncementApplication*/
delete from AnnouncementApplication where AnnouncementRef IN (SELECT Id FROM Announcement
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0)


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
end

if(@classAnnouncementTypeId is not null and @classId is not null)
begin 
	declare @schoolYearId int
	select @schoolYearId = Id from SchoolYear where @created between StartDate and EndDate
	declare @resT table (res int)
	insert into @resT
	exec [spReorderAnnouncements] @schoolYearId, @classAnnouncementTypeId,  @classId
end
exec spGetAnnouncementDetails @announcementId, @personId, @callerRole, @schoolId

commit
GO


