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
If 
(
	Object_Id('[dbo].[School]') Is Not Null
	And Exists(Select * From sys.columns Where Name = 'IsNewAssessmentEnabled') 
)
Begin
	Alter Table [dbo].[School] 
	DROP COLUMN[IsNewAssessmentEnabled]
End
Go