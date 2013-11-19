ALTER PROCEDURE [dbo].[spDeleteAnnouncement] @id uniqueidentifier, @personId uniqueidentifier, @classId uniqueidentifier,
@state int, @announcementTypeId int
AS


declare @ann table
(
Id uniqueidentifier,
SchoolYearId uniqueidentifier,
AnnouncementTypeId int,
PersonId uniqueidentifier,
[State] int,
ClassId uniqueidentifier
)

insert into @ann(Id, SchoolYearId, AnnouncementTypeId, PersonId, [State], ClassId)
select a.Id, mp.SchoolYearRef, a.AnnouncementTypeRef, a.PersonRef, a.State, mpc.ClassRef
from Announcement a
left join MarkingPeriodClass mpc  on mpc.Id = a.MarkingPeriodClassRef
left join MarkingPeriod mp on mp.Id = mpc.MarkingPeriodRef
where (@id is null or a.Id = @id)
and (@personId is null or PersonRef = @personId)
and (@classId is null or mpc.ClassRef = @classId)
and (@announcementTypeId is null or AnnouncementTypeRef = @announcementTypeId)
and (mpc.Id is not null or a.AnnouncementTypeRef = 11  or a.[State] = 0)
and (@state is null or a.State = @state)
delete from AnnouncementReminder where AnnouncementRef in (select Id from @ann)
/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef in (select Id from @ann)
/*DELETE AnnouncementApplication*/
delete from StudentAnnouncement
where AnnouncementRef in (select Id from @ann)
/*DELETE AnnouncementRecipient*/
delete from AnnouncementRecipient
where AnnouncementRef in (select Id from @ann)
/*DELETE AnnouncementRecipientData*/
delete from AnnouncementRecipientData
where AnnouncementRef in (select Id from @ann)

/*DELETE NOTIFICATIONS*/
--delete from [Notification]
--where AnnouncementRef = @Id

/*DELETE ANOUNCEMENTQNA*/
delete from AnnouncementQnA
where AnnouncementRef in (select Id from @ann)

/*DELETE AnnouncementApplication*/
delete from AnnouncementApplication
where AnnouncementRef in (select Id from @ann)


--select @annTypeId = a.AnnouncementTypeRef, @ownerId = a.PersonRef, @schoolYearId =  mp.SchoolYearRef, @classId = mpc.ClassRef
--from Announcement a
--join MarkingPeriodClass mpc on mpc.Id = a.MarkingPeriodClassRef
--join MarkingPeriod mp on mp.Id = mpc.MarkingPeriodRef
--where a.Id in (select Id from @ann)


/*DELETE Announcement*/
delete from Announcement where Id in (select Id from @ann)


declare @schoolYearId uniqueidentifier

declare  AnnCursor cursor for
select SchoolYearId, AnnouncementTypeId, PersonId,  ClassId
from @ann

open AnnCursor
fetch next from AnnCursor
into @schoolYearId, @announcementTypeId, @personId, @classId

declare @oldSchoolYear uniqueidentifier, @oldannouncementTypeId int,
@oldPersonId uniqueidentifier, @oldClassId uniqueidentifier

while @@FETCH_STATUS = 0
begin

if(@schoolYearId <> @oldSchoolYear or @announcementTypeId <> @oldannouncementTypeId
or @personId <> @oldPersonId or @classId <> @oldClassId
or (@oldSchoolYear is null and @oldannouncementTypeId is null
and @oldPersonId is null and @oldClassId is null))
begin
/*Reordering Process*/
exec spReorderAnnouncements @schoolYearId, @announcementTypeId, @personId, @classId

set @oldannouncementTypeId = @announcementTypeId
set @oldClassId = @classId
set @oldPersonId = @personId
set @oldSchoolYear = @schoolYearId

end
fetch next from AnnCursor
into @schoolYearId, @announcementTypeId, @personId, @classId
end
CLOSE AnnCursor;
DEALLOCATE AnnCursor;


GO


