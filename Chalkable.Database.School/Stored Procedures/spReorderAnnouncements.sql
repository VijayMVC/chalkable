
CREATE procedure [dbo].[spReorderAnnouncements] @schoolYearId int, @classAnnType int, @classId int
as
with AnnView as
(
select a.Id, Row_Number() over(order by ClassAnnouncement.Expires, a.[Created]) as [Order]
from Announcement a
join ClassAnnouncement on ClassAnnouncement.Id = a.Id
join Class c on c.Id = ClassAnnouncement.ClassRef
where c.SchoolYearRef = @schoolYearId and ClassAnnouncement.ClassAnnouncementTypeRef = @classAnnType and ClassAnnouncement.ClassRef = @classId
)
update ClassAnnouncement
set [Order] = AnnView.[Order]
from AnnView
where AnnView.Id = ClassAnnouncement.Id
select  1