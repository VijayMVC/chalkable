alter procedure [dbo].[spReorderAnnouncements] @schoolYearId int, @classAnnType int, @classId int
as
with AnnView as
               (
                select a.Id, Row_Number() over(order by a.[Created], a.Expires) as [Order]  
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
