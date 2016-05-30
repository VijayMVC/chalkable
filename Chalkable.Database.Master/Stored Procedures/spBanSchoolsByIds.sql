CREATE PROCEDURE [dbo].[spBanSchoolsByIds]
	@applicationId uniqueidentifier,
	@schoolIds TGUID ReadOnly
AS

--UnBan all schools not in list
Update ApplicationSchoolOption
Set Banned = 0
Where 
	ApplicationRef = @applicationId
	And SchoolRef not in(Select * From @schoolIds)

--Creating list of row needed to insert
Declare @toInsert TApplicationSchoolOption

Insert into @toInsert
Select @applicationId, schools.value, 1 From @schoolIds As schools
Where not exists(Select * From ApplicationSchoolOption Where SchoolRef = schools.value And ApplicationRef = @applicationId)

--Banning applications for schools
Update ApplicationSchoolOption
Set Banned = 1
Where 
	ApplicationRef = @applicationId
	And SchoolRef in(Select * From @schoolIds)

Insert Into ApplicationSchoolOption
	Select * From @toInsert