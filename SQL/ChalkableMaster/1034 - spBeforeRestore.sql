Create Procedure spBeforeRestore 
	@districtId uniqueidentifier
as
	
	ALTER TABLE ApplicationRating NOCHECK CONSTRAINT ALL 
	ALTER TABLE Fund NOCHECK CONSTRAINT ALL
	ALTER TABLE FundRequest NOCHECK CONSTRAINT ALL
	ALTER TABLE FundRequestRoleDistribution NOCHECK CONSTRAINT ALL

	Delete from [SchoolUser] where DistrictRef = @districtId
	Delete from [UserLoginInfo] where Id in
		(select id from [User] where DistrictRef = @districtId)
	Delete from [School] where DistrictRef = @districtId
	Delete from [User] where DistrictRef = @districtId



	