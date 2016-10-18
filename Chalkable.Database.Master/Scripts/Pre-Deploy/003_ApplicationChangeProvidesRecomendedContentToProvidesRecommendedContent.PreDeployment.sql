/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
If 
(
	Object_Id('[dbo].[Application]') Is Not Null
	And Exists(Select * From sys.columns Where Name = 'ProvidesRecomendedContent') 
)
Begin
	exec sys.sp_rename N'dbo.Application.ProvidesRecomendedContent', 'ProvidesRecommendedContent'
End