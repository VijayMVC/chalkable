ALTER procedure [dbo].[spApplyStarringAnnouncementForTeacher] @personId uniqueidentifier, @currentDate date
as
DECLARE @id uniqueidentifier, @ExpDay date, @starredA int
DECLARE @currentDay date = @currentDate
DECLARE @nextDay date = DATEADD(Day, 1, @currentDay)
DECLARE AnnouncementCursorToUnstar CURSOR FOR
select 
	x.AnnouncmentId, x.AnExpires, ard.StarredAutomatically
from 
	AnnouncementRecipientData ard
	right join (select an.Id as AnnouncmentId, an.Expires as AnExpires , @personId as AnSchoolPerson
		from Announcement an
		left join AnnouncementRecipient ar on ar.AnnouncementRef = an.Id
		where  an.PersonRef = @personId or (ar.Id is not null and
		(ar.PersonRef = @personId  or ar.ToAll = 1 or ar.RoleRef = 2)))x
	on x.AnnouncmentId = ard.AnnouncementRef and x.AnSchoolPerson = ard.PersonRef
where  
	x.AnExpires < @currentDay
	and ard.StarredAutomatically = 1
	and (ard.Id is null or ard.Starred <> 0)
	and ard.LastModifiedDate < @currentDay

OPEN AnnouncementCursorToUnstar
FETCH NEXT FROM AnnouncementCursorToUnstar
INTO @id, @ExpDay, @starredA
WHILE @@FETCH_STATUS = 0
BEGIN
	exec spUpdateAnnouncemetRecipientData @personId, @id, 0, 0, @currentDay

	FETCH NEXT FROM AnnouncementCursorToUnstar
	INTO @id, @ExpDay, @starredA
END
CLOSE AnnouncementCursorToUnstar;
DEALLOCATE AnnouncementCursorToUnstar;


DECLARE AnnouncementCursorToStar CURSOR FOR
select 
	x.AnnouncmentId, x.AnExpires, ard.StarredAutomatically
from 
	AnnouncementRecipientData ard
	right join (select an.Id as AnnouncmentId, an.Expires as AnExpires , @personId as AnSchoolPerson
		from Announcement an
		left join AnnouncementRecipient ar on ar.AnnouncementRef = an.Id
		where an.PersonRef = @personId or (ar.Id is not null and
		(ar.PersonRef = @personId  or ar.ToAll = 1 or ar.RoleRef = 2)))x
	on x.AnnouncmentId = ard.AnnouncementRef and x.AnSchoolPerson = ard.PersonRef
where 
	x.AnExpires = @nextDay
	and (ard.StarredAutomatically < 1 or ard.StarredAutomatically is null)
	and (ard.Id is null or ard.Starred <> 1)
	and (ard.LastModifiedDate is null or ard.LastModifiedDate < @currentDay)

OPEN AnnouncementCursorToStar
FETCH NEXT FROM AnnouncementCursorToStar
INTO @id, @ExpDay, @starredA
WHILE @@FETCH_STATUS = 0
BEGIN
	exec spUpdateAnnouncemetRecipientData @personId, @id, 1, 1, @currentDay

	FETCH NEXT FROM AnnouncementCursorToStar
	INTO @id, @ExpDay, @starredA
END
CLOSE AnnouncementCursorToStar;
DEALLOCATE AnnouncementCursorToStar;


GO


