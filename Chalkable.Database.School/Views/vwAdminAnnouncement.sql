
Create View vwAdminAnnouncement
As
Select
	Announcement.Id as Id,
	Announcement.Created as Created,
	Announcement.[State] as [State],
	Announcement.Content as Content,
	Announcement.Title as [Title],
	Announcement.DiscussionEnabled as [DiscussionEnabled],
	Announcement.PreviewCommentsEnabled as [PreviewCommentsEnabled],
	Announcement.RequireCommentsEnabled as [RequireCommentsEnabled],

	AdminAnnouncement.Expires as Expires,
	AdminAnnouncement.AdminRef as AdminRef,
	Person.FirstName + ' ' + Person.LastName as AdminName,
	Person.Gender as AdminGender

From AdminAnnouncement
	Join Announcement 
		on Announcement.Id = AdminAnnouncement.Id
	Join Person 
		on Person.Id = AdminAnnouncement.AdminRef