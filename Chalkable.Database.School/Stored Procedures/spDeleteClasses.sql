CREATE PROCEDURE [dbo].[spDeleteClasses]
	@classIds TInt32 READONLY
AS

DELETE ca FROM dbo.ClassAnnouncement ca WHERE EXISTS (SELECT *  FROM @ClassIds ci WHERE ca.ClassRef= ci.Value)
DELETE lp FROM dbo.LessonPlan lp WHERE EXISTS (SELECT *  FROM @ClassIds ci WHERE lp.ClassRef= ci.Value)
DELETE sa FROM dbo.SupplementalAnnouncement sa WHERE EXISTS (SELECT *  FROM @ClassIds ci WHERE sa.ClassRef= ci.Value)

DECLARE @annToDelete Table (id int)
INSERT INTO @annToDelete
SELECT
	Announcement.Id
FROM
	dbo.Announcement
	LEFT JOIN dbo.ClassAnnouncement ON Announcement.Id = ClassAnnouncement.Id
	LEFT JOIN dbo.LessonPlan ON Announcement.Id = LessonPlan.Id
	LEFT JOIN dbo.AdminAnnouncement ON Announcement.Id = AdminAnnouncement.Id
	LEFT JOIN dbo.SupplementalAnnouncement ON Announcement.Id = SupplementalAnnouncement.Id
WHERE
	ClassAnnouncement.id IS NULL
	AND LessonPlan.id IS NULL
	AND AdminAnnouncement.id IS NULL
	AND SupplementalAnnouncement.id IS NULL



DELETE a FROM dbo.AnnouncementStandard a WHERE EXISTS(SELECT * FROM @annToDelete atd WHERE atd.id= a.AnnouncementRef)
DELETE a FROM dbo.AnnouncementApplication a WHERE EXISTS(SELECT * FROM @annToDelete atd WHERE atd.id= a.AnnouncementRef)
DELETE n FROM dbo.[Notification] n WHERE EXISTS(SELECT * FROM @annToDelete atd WHERE atd.id= n.AnnouncementRef)
DELETE a FROM dbo.AnnouncementAttachment a WHERE EXISTS(SELECT * FROM @annToDelete atd WHERE atd.id= a.AnnouncementRef)
DELETE a FROM dbo.AnnouncementQnA a WHERE EXISTS(SELECT * FROM @annToDelete atd WHERE atd.id= a.AnnouncementRef)
DELETE a FROM dbo.AnnouncementRecipientData a WHERE EXISTS(SELECT * FROM @annToDelete atd WHERE atd.id= a.AnnouncementRef)
DELETE a FROM dbo.AnnouncementAssignedAttribute a WHERE EXISTS(SELECT * FROM @annToDelete atd WHERE atd.id= a.AnnouncementRef)
DELETE a FROM dbo.AnnouncementApplication a WHERE EXISTS(SELECT * FROM @annToDelete atd WHERE atd.id= a.AnnouncementRef)
DELETE a FROM dbo.AnnouncementComment a WHERE EXISTS(SELECT * FROM @annToDelete atd WHERE atd.id= a.AnnouncementRef)

DELETE a FROM dbo.Announcement a WHERE EXISTS(SELECT * FROM @annToDelete atd WHERE atd.id= a.Id)

DELETE c FROM dbo.Class c WHERE EXISTS (SELECT * FROM @ClassIds ci WHERE c.Id = ci.Value)


