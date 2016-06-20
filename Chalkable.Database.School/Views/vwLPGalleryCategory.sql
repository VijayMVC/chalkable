

Create View vwLPGalleryCategory
as
Select LPGalleryCategory.*,
(Select Count(*) From LessonPlan Where GalleryCategoryRef = LPGalleryCategory.Id) as LessonPlansCount
From LPGalleryCategory