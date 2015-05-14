Create Procedure spBeforeRestore 
	@districtId uniqueidentifier
as
	
	ALTER TABLE ApplicationRating NOCHECK CONSTRAINT ALL 
	ALTER TABLE Fund NOCHECK CONSTRAINT ALL
	ALTER TABLE FundRequest NOCHECK CONSTRAINT ALL
	ALTER TABLE FundRequestRoleDistribution NOCHECK CONSTRAINT ALL

	Delete from [SchoolUser] where DistrictRef = @districtId
	Delete from [School] where DistrictRef = @districtId	



	