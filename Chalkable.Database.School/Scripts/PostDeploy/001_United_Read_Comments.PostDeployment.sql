--001_Drop_GalleryCategoryRef_From_LessonPlan.PostDeploy.sql

If (Object_Id('[dbo].[LessonPlan]') Is Not Null And Exists(Select * From sys.columns Where Name = '[GalleryCategoryRef]'))
Begin
	ALTER TABLE [dbo].[LessonPlan]
	DROP COLUMN [GalleryCategoryRef]
End

--002_Delete_Obsolate_Functions.sql

IF OBJECT_ID (N'dbo.parseJSON') IS NOT NULL 
	DROP FUNCTION dbo.parseJSON
GO
IF OBJECT_ID (N'dbo.ToJSON') IS NOT NULL 
	DROP FUNCTION dbo.ToJSON
GO
IF OBJECT_ID (N'dbo.JSONEscaped') IS NOT NULL  
	DROP FUNCTION dbo.JSONEscaped
GO
IF TYPE_ID(N'dbo.THierarchy') IS NOT NULL
	DROP TYPE dbo.THierarchy
GO