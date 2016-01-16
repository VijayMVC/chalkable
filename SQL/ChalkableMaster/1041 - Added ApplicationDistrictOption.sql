Create Table ApplicationDistrictOption
(
	ApplicationRef uniqueidentifier not null Constraint FK_ApplicationDistrictOption_Application Foreign Key References Application(Id),
	DistrictRef uniqueidentifier not null Constraint FK_ApplicationDistrictOption_District Foreign Key References District(Id),
	Ban bit not null
)

GO

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
