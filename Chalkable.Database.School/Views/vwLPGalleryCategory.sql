

Create View vwLPGalleryCategory
as
Select LPGalleryCategory.*,
(Select Count(*) From LessonPlan Where LpGalleryCategoryRef = LPGalleryCategory.Id) as LessonPlansCount
From LPGalleryCategory