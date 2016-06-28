If (Object_Id('[dbo].[LessonPlan]') Is Not Null And Exists(Select * From sys.columns Where Name = '[GalleryCategoryRef]'))
Begin
	ALTER TABLE [dbo].[LessonPlan]
	DROP COLUMN [GalleryCategoryRef]
End