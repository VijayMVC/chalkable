
CREATE Procedure [dbo].[spSelectAnnouncementAddionalData] @announcementId int, @ownerId int, @callerId int, @schoolId int
As
select top 1 * from vwPerson
where Id = @ownerId and (@schoolId is null or SchoolRef = @schoolId)

exec spGetAnnouncementsQnA @callerId, null, @announcementId, null, null, @schoolId

select * from vwAnnouncementAssignedAttribute attr
where AnnouncementAssignedAttribute_AnnouncementRef = @announcementId 

select aa.*
from AnnouncementApplication aa
where aa.AnnouncementRef = @announcementId and @announcementId is not null and aa.Active = 1

select * from AnnouncementStandard 
join [Standard] on [Standard].Id = AnnouncementStandard.StandardRef
where AnnouncementStandard.AnnouncementRef = @announcementId