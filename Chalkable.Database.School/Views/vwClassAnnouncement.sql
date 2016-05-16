

---------------------------------------------
-- added SchoolYearRef to vwClassAnnouncement
---------------------------------------------

CREATE View [dbo].[vwClassAnnouncement]
As
Select
Announcement.Id as Id,
Announcement.Created as Created,
Announcement.[State] as [State],
Announcement.Content as Content,
Announcement.Title as [Title],
ClassAnnouncement.Expires as Expires,
ClassAnnouncement.[Order] as [Order],
ClassAnnouncement.Dropped as Dropped,
ClassAnnouncement.ClassAnnouncementTypeRef as ClassAnnouncementTypeRef,
ClassAnnouncement.SchoolYearRef as SchoolYearRef,
ClassAnnouncement.SisActivityId as SisActivityId,
ClassAnnouncement.IsScored as IsScored,
ClassAnnouncement.MaxScore as MaxScore,
ClassAnnouncement.WeightAddition as WeightAddition,
ClassAnnouncement.WeightMultiplier as WeightMultiplier,
ClassAnnouncement.MayBeDropped as MayBeDropped,
ClassAnnouncement.VisibleForStudent as VisibleForStudent,
ClassAnnouncement.ClassRef as ClassRef,

Staff.FirstName + ' ' + Staff.LastName as PrimaryTeacherName,
Staff.Gender as PrimaryTeacherGender,
Class.Name as ClassName,
Class.Name + ' ' + (case when Class.ClassNumber is null then '' else Class.ClassNumber end) as FullClassName,
Class.MinGradeLevelRef as MinGradeLevelId,
Class.MaxGradeLevelRef as MaxGradeLevelId,
Class.PrimaryTeacherRef as PrimaryTeacherRef,
Class.ChalkableDepartmentRef as DepartmentId

From ClassAnnouncement
Join Announcement on Announcement.Id = ClassAnnouncement.Id
Join Class on Class.Id = ClassAnnouncement.ClassRef
Left Join Staff on Staff.Id = Class.PrimaryTeacherRef