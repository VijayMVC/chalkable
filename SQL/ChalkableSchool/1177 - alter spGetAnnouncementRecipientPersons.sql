Alter Procedure [dbo].[spGetAnnouncementRecipientPersons] @announcementId int, @callerId int
as
declare @classId int, @schoolId int, @adminId int
select top 1
@classId = ann.ClassRef,
@schoolId = ann.SchoolRef,
@adminId = ann.AdminRef
From (
select LessonPlan.Id, LessonPlan.ClassRef, LessonPlan.SchoolRef, null as AdminRef
from LessonPlan where LessonPlan.Id = @announcementId
union
select ClassAnnouncement.Id, ClassAnnouncement.ClassRef, ClassAnnouncement.SchoolRef, null as AdminRef
from ClassAnnouncement where ClassAnnouncement.Id = @announcementId
union
select AdminAnnouncement.Id, null as ClassRef, null as SchoolRef, AdminAnnouncement.AdminRef as AdminRef
from AdminAnnouncement where AdminAnnouncement.Id = @announcementId
) ann


if(@classId is not null)
begin
select * from vwPerson
where exists(select * from ClassPerson where ClassRef = @classId and PersonRef = vwPerson.Id) and vwPerson.SchoolRef = @schoolId
end
else
begin
select * from vwPerson
join StudentGroup on StudentGroup.StudentRef = vwPerson.Id
join AnnouncementGroup on AnnouncementGroup.GroupRef = StudentGroup.GroupRef and AnnouncementGroup.AnnouncementRef = @announcementId
end

GO


