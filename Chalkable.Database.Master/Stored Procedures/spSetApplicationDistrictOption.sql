
Create Procedure spSetApplicationDistrictOption
	@applicationId uniqueidentifier,
	@districtId uniqueidentifier,
	@ban bit
as
	if not exists (
		select * from 
			ApplicationDistrictOption
		where ApplicationRef = @applicationId
			and DistrictRef = @districtId
		)
		Insert into ApplicationDistrictOption
			(ApplicationRef, DistrictRef, Ban)
		values
			(@applicationId, @districtId, @ban)
	else
		update ApplicationDistrictOption
		set Ban = @ban
		where
			ApplicationRef = @applicationId
			and DistrictRef = @districtId