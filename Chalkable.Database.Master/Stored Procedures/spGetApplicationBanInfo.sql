CREATE PROCEDURE spGetApplicationsBanInfo
	@applicationIds TGuid Readonly,
	@districtId uniqueidentifier,
	@schoolId uniqueidentifier
As

Declare @ApplicationBanInfoT table 
(
	ApplicationId uniqueidentifier,
	UnBannedSchoolCount int,
	BannedSchoolCount int,
	BannedForCurrentSchool bit
)

Insert Into @ApplicationBanInfoT
	Select 
		AppIds.value,
		SUM(Case When Banned is null or Banned = 0 Then 1 Else 0 End) As UnBannedSchoolCount,
		SUM(Case When Banned = 1 Then 1 Else 0 End) As BannedSchoolCount,
		0 As BannedForCurrentSchool
	From
		@applicationIds As AppIds
		left join ApplicationSchoolOption
			On ApplicationSchoolOption.ApplicationRef = AppIds.value
		left join School
			On ApplicationSchoolOption.SchoolRef = School.Id
	Where
		School.DistrictRef is null or School.DistrictRef = @districtId
	Group By
		AppIds.value

Update @ApplicationBanInfoT
Set BannedForCurrentSchool = 1
Where Exists(Select * From ApplicationSchoolOption Where ApplicationRef = ApplicationId And SchoolRef = @schoolId)

select * from @ApplicationBanInfoT