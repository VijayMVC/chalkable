


---------------------------------------
-- Adding SchoolYearRef to vwLessonPlan
---------------------------------------



CREATE View [dbo].[vwLessonPlan]
As
Select
Announcement.Id as Id,
Announcement.Created as Created,
Announcement.[State] as [State],
Announcement.Content as Content,
Announcement.Title as [Title],
LessonPlan.ClassRef as ClassRef,
LessonPlan.SchoolYearRef as SchoolYearRef,
LessonPlan.StartDate as StartDate,
LessonPlan.EndDate as EndDate,
LessonPlan.LPGalleryCategoryRef as LPGalleryCategoryRef,
LessonPlan.VisibleForStudent as VisibleForStudent,
LessonPlan.InGallery as InGallery,
LessonPlan.GalleryOwnerRef as GalleryOwnerRef,

Staff.FirstName + ' ' + Staff.LastName as PrimaryTeacherName,
Staff.Gender as PrimaryTeacherGender,
Class.Name as ClassName,
Class.Name + ' ' + (case when Class.ClassNumber is null then '' else Class.ClassNumber end) as FullClassName,
Class.MinGradeLevelRef as MinGradeLevelId,
Class.MaxGradeLevelRef as MaxGradeLevelId,
Class.PrimaryTeacherRef as PrimaryTeacherRef,
Class.ChalkableDepartmentRef as DepartmentId

From LessonPlan
Join Announcement on Announcement.Id = LessonPlan.Id
Left Join Class on Class.Id = LessonPlan.ClassRef
Left Join Staff on Staff.Id = Class.PrimaryTeacherRef