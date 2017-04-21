Create Procedure spBeforeRestore 
	@districtId uniqueidentifier
as
	
	Delete from [SchoolUser] where DistrictRef = @districtId
	
	Delete from [ApplicationSchoolOption] 
	where SchoolRef in (Select Id from [School] where DistrictRef = @districtId)

	Delete from [School] where DistrictRef = @districtId
