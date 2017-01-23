Create Procedure spBeforeRestore 
	@districtId uniqueidentifier
as
	
	Delete from [SchoolUser] where DistrictRef = @districtId
	Delete from [School] where DistrictRef = @districtId