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
	from Announcement
	where AdminRef = @personId and [State] = 0
	order by Created desc

	if @announcementId is not null
	update Announcement set [State] = -1 where Id = @announcementId
end

/*DELETE AdminAnnouncementData*/
Delete From AdminAnnouncementData Where AnnouncementRef IN (SELECT Id FROM Announcement 
where AdminRef = @personId AND [State] = 0)

/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef IN (SELECT Id FROM Announcement 
where AdminRef = @personId AND [State] = 0)
/*DELETE AutoGrades*/
delete from AutoGrade
where AnnouncementApplicationRef IN (SELECT AnnouncementApplication.Id from AnnouncementApplication
		join Announcement on Announcement.Id = AnnouncementRef
		where AdminRef = @personId AND [State] = 0)
/*DELETE AnnouncementApplication*/
delete from AnnouncementApplication where AnnouncementRef IN (SELECT Id FROM Announcement
where AdminRef = @personId AND [State] = 0)
/*DELETE AdminAnnouncementRecipient*/
delete from AdminAnnouncementRecipient
where AnnouncementRef in (Select id from announcement 
where AdminRef = @personId AND [State] = 0)
/*DELETE AnnouncementStandard*/
delete from AnnouncementStandard
where AnnouncementRef in (Select id from announcement 
where AdminRef = @personId AND [State] = 0)
/*DELETE Announcement*/
delete from Announcement
where AdminRef = @personId AND [State] = 0

if @announcementId is not null
begin
	update Announcement set [State] = 0 where Id = @announcementId
	set @isDraft = 1
end
else begin
	/*INSERT TO ANNOUNCEMENT*/
	insert into Announcement (Created, Expires, ClassAnnouncementTypeRef, [State],GradingStyle,[Order], ClassRef, Dropped, SchoolRef, MayBeDropped, VisibleForStudent, AdminRef)
	values(@created, @expires, null, @state, 0, 1, null, 0, null, 0, 1, @personId);
	set @announcementId = SCOPE_IDENTITY()

	/*GET CONTENT FROM PREV ANNOUNCEMENT*/
	declare @prevContent nvarchar(1024)
	select top 1
	@prevContent = Content from Announcement
	where AdminRef = @personId and [State] = 1 and Content is not null
	order by Created desc
	
	update Announcement set Content = @prevContent where Id = @announcementId
end

exec spGetAnnouncementDetails @announcementId, @personId, @callerRole, null
commit

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
left join Class c on c.Id = a.ClassRef
where (@id is null or a.Id = @id)
	and (@personId is null or c.PrimaryTeacherRef = @personId or a.AdminRef = @personId
		 or exists(select * from ClassTeacher ct where ct.PersonRef = @personId and ct.ClassRef = c.Id))
	and (@classId is null or a.ClassRef = @classId)
	and (@classAnnouncementTypeId is null or ClassAnnouncementTypeRef = @classAnnouncementTypeId)
	and (a.ClassRef is not null or a.ClassAnnouncementTypeRef is null  or a.[State] = 0)
	and (@state is null or a.[State] = @state)


/*DELETE AdminAnnouncementData*/
delete from AdminAnnouncementData where AnnouncementRef in (select Id from @ann)

/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef in (select Id from @ann)
/*DELETE AnnouncementApplication*/

/*DELETE AnnouncementRecipient*/
delete from AdminAnnouncementRecipient
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


