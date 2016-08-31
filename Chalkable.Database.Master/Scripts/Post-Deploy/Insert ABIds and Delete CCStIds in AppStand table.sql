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

Declare @appAbStandard Table (AppId uniqueidentifier, StandardId uniqueidentifier)

---get all children for each appStandard
;With AppStandardRec As
(
	Select Distinct
		StandardRef, 
		StandardRef as ChildRef, 
		1 as [TreeLevel],
	   cast((case when exists(Select * From CommonCoreStandard Where ParentStandardRef = StandardRef) then 0 else 1 end) as bit) as IsLeaf 
	From ApplicationStandard
	Union ALL
	Select 
		AppStandardRec.StandardRef,
		CommonCoreStandard.Id as ChildRef,
		AppStandardRec.[TreeLevel]  + 1  as TreeLevel,
		cast((case when exists(Select * From CommonCoreStandard ccs Where ccs.ParentStandardRef = CommonCoreStandard.Id) then 0 else 1 end) as bit) as IsLeaf 
	From AppStandardRec
	Join CommonCoreStandard 
		on  AppStandardRec.ChildRef = CommonCoreStandard.ParentStandardRef 
)
Insert Into @appAbStandard
Select distinct
	ApplicationStandard.ApplicationRef,
	mapping.AcademicBenchmarkId
From 
	AppStandardRec
Join ApplicationStandard on ApplicationStandard.StandardRef = AppStandardRec.StandardRef
Join AbTOCCMapping mapping on mapping.CCStandardRef = AppStandardRec.ChildRef
Where AppStandardRec.IsLeaf = 1 

---Delete All App with CCStandards
Delete From ApplicationStandard
Where StandardRef in(Select Id From CommoncoreStandard)

--- Insert Apps with ABstandards ids

Insert Into ApplicationStandard(ApplicationRef, StandardRef)
Select AppId, StandardId 
From @appAbStandard