Create View [dbo].[vwSupplementalAnnouncement]
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

	SupplementalAnnouncement.ClassRef as ClassRef,
	SupplementalAnnouncement.SchoolYearRef as SchoolYearRef,
	SupplementalAnnouncement.Expires as Expires,
	SupplementalAnnouncement.ClassAnnouncementTypeRef as ClassAnnouncementTypeRef,
	SupplementalAnnouncement.VisibleForStudent as VisibleForStudent,

	Staff.FirstName + ' ' + Staff.LastName as PrimaryTeacherName,
	Staff.Gender as PrimaryTeacherGender,
	Class.Name as ClassName,
	Class.Name + ' ' + (case when Class.ClassNumber is null then '' else Class.ClassNumber end) as FullClassName,
	Class.PrimaryTeacherRef as PrimaryTeacherRef

 From SupplementalAnnouncement
	 Join Announcement 
		on Announcement.Id = SupplementalAnnouncement.Id
	 Join Class 
		on Class.Id = SupplementalAnnouncement.ClassRef
	 Left Join Staff 
		on Staff.Id = Class.PrimaryTeacherRef
GO