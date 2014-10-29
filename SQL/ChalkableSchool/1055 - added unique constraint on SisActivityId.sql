delete from Notification
where AnnouncementRef in (select Id from Announcement
where (select Count(*) from Announcement a2
	   where a2.SisActivityId is not null and a2.SisActivityId = Announcement.SisActivityId) > 1)



delete from AnnouncementApplication
where AnnouncementRef in (select Id from Announcement
where (select Count(*) from Announcement a2
	   where a2.SisActivityId is not null and a2.SisActivityId = Announcement.SisActivityId) > 1)


delete from AnnouncementAttachment
where AnnouncementRef in (select Id from Announcement
where (select Count(*) from Announcement a2
	   where a2.SisActivityId is not null and a2.SisActivityId = Announcement.SisActivityId) > 1)



delete from Announcement
where (select Count(*) from Announcement a2
	   where a2.SisActivityId is not null and a2.SisActivityId = Announcement.SisActivityId) > 1


CREATE UNIQUE NONCLUSTERED INDEX [UQ_Announcement_SisActivityId]
ON Announcement ([SisActivityId])
WHERE [SisActivityId] IS NOT NULL