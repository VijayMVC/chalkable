Create Procedure spUpdateStudyCenterEnabled
	@districtId uniqueidentifier,
	@schoolId uniqueidentifier,
	@enabledTill DateTime2
as
	if @districtId is not null
	begin
		Update 
			School 
		Set
			StudyCenterEnabledTill = @enabledTill
		where 
			DistrictRef = @districtId
	end
	if @schoolId is not null
	begin
		Update 
			School 
		Set
			StudyCenterEnabledTill = @enabledTill
		where 
			Id = @schoolId
	end