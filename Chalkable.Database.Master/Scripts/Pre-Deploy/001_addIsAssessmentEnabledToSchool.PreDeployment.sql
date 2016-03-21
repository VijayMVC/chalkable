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
	Object_Id('[dbo].[School]') Is Not Null
	And Not Exists(Select * From sys.columns Where Name = 'IsAssessmentEnabled') 
)
Begin
	Alter Table [dbo].[School] 
	Add [IsAssessmentEnabled] Bit
End
Go

If 
(
	Object_Id('[dbo].[School]') Is Not Null
	And Exists(Select * From sys.columns Where Name = 'IsAssessmentEnabled' and is_nullable = 1) 
)
Begin
	Update [dbo].[School]
	Set [IsAssessmentEnabled] = 1
	Where IsAssessmentEnabled is null

	Alter Table [dbo].[School] Alter Column [IsAssessmentEnabled] Bit Not Null
End