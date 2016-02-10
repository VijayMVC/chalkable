/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

If (Object_Id('[dbo].[Application]') Is Not Null)
	And Not Exists(Select * From sys.columns Where Name = 'ProvidesRecomendedContent' And [object_id] = Object_Id('[dbo].[Application]'))
Begin
	Alter Table [dbo].[Application] Add [ProvidesRecomendedContent] Bit Null

	Update [dbo].[Application]
	Set [ProvidesRecomendedContent] = 0

	Alter Table [dbo].[Application] Alter Column [ProvidesRecomendedContent] Bit Not Null
End