CREATE procedure [dbo].[spGetStudentAnnouncements]
@id int, @schoolId int, @personId int, @classId int
, @fromDate DateTime2, @toDate DateTime2, @start int, @count int
, @complete bit, @adminOnly bit
as

declare @allCount int = (select COUNT(*) from
vwAnnouncement
left join (select * from AdminAnnouncementData where PersonRef = @personId) adminAnnData on adminAnnData.AnnouncementRef = vwAnnouncement.Id
where
(@id is not null  or [State] = 1) and
(@id is null or vwAnnouncement.Id = @id)
and (@adminOnly = 0 or AdminRef is not null)
and (AdminRef is not null or SchoolRef = @schoolId)
and (@classId is null or ClassRef = @classId)
and (@complete is null or adminAnnData.Complete = @complete or (@complete = 0 and adminAnnData.Complete is null))
and (
(ClassRef is not null and exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))
or (AdminRef is not null and exists(
select * from AdminAnnouncementRecipient
join StudentGroup on StudentGroup.GroupRef = AdminAnnouncementRecipient.GroupRef
where AdminAnnouncementRecipient.AnnouncementRef = vwAnnouncement.Id and StudentGroup.StudentRef = @personId
)
)
)
and (@fromDate is null or Expires >= @fromDate)
and (@toDate is null or Expires <= @toDate)
)

Select
vwAnnouncement.*,
cast(0 as bit) as IsOwner,
cast((case when adminAnnData.Complete is null then 0 else adminAnnData.Complete end) as bit) as Complete,
ROW_NUMBER() OVER(ORDER BY  vwAnnouncement.Expires) as RowNumber,
@allCount as AllCount
from
vwAnnouncement
left join (select * from AdminAnnouncementData where PersonRef = @personId) adminAnnData on adminAnnData.AnnouncementRef = vwAnnouncement.Id
where
(@id is not null  or [State] = 1) and
(@id is null or vwAnnouncement.Id = @id)
and (@adminOnly = 0 or AdminRef is not null)
and (AdminRef is not null or SchoolRef = @schoolId)
and (@classId is null or ClassRef = @classId)
and (@complete is null or adminAnnData.Complete = @complete or (@complete = 0 and adminAnnData.Complete is null))
and (
(ClassRef is not null and exists(select * from ClassPerson where ClassRef = ClassRef and PersonRef = @personId))
or (AdminRef is not null and exists(
select * from AdminAnnouncementRecipient
join StudentGroup on StudentGroup.GroupRef = AdminAnnouncementRecipient.GroupRef
where AdminAnnouncementRecipient.AnnouncementRef = vwAnnouncement.Id and StudentGroup.StudentRef = @personId
)
)
)
and (@fromDate is null or Expires >= @fromDate)
and (@toDate is null or Expires <= @toDate)

order by Expires desc
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY