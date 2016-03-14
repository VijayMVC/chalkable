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

IF OBJECT_ID (N'PersonSetting', N'U') IS NOT NULL 
Begin 
	Update PersonSetting set Value = '0' where [Key] = 'feedsort' and Value = 'False'
	Update PersonSetting set Value = '1' where [Key] = 'feedsort' and Value = 'True'
End