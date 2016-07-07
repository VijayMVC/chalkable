CREATE PROCEDURE spGetApplicationsBanInfo
	@applicationIds TGUID Readonly,
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

Declare @totalSchools int = (Select count(*) From School Where DistrictRef = @districtId)

Insert Into @ApplicationBanInfoT
	Select 
		AppIds.value,
		0 As UnBannedSchoolCount,
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
Set UnBannedSchoolCount = @totalSchools - BannedSchoolCount

Update @ApplicationBanInfoT
Set BannedForCurrentSchool = 1
Where Exists(Select * From ApplicationSchoolOption Where ApplicationRef = ApplicationId And SchoolRef = @schoolId And Banned = 1)

select * from @ApplicationBanInfoT