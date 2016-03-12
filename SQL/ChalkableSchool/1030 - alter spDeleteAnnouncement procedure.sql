Alter PROCEDURE [dbo].[spDeleteAnnouncement] @id int, @personId int, @classId int, @state int, @classAnnouncementTypeId int
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


