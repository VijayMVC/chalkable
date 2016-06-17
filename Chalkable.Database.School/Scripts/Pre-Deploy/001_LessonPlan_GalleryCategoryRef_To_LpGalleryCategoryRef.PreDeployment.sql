If 
(
	Object_Id('[dbo].[LessonPlan]') Is Not Null
	And Exists(Select * From sys.columns Where Name = 'GalleryCategoryRef') 
)
Begin
	exec sys.sp_rename N'dbo.LessonPlan.GalleryCategoryRef', 'LpGalleryCategoryRef'
End