

Create Procedure [dbo].[spUpdateMessagingDisabled]
	@districtId uniqueidentifier,
	@schoolId uniqueidentifier,
	@disabled bit
as
Begin Transaction 
	if @districtId is not null
	begin
		Update 
			School 
		Set
			IsMessagingDisabled = @disabled
		where 
			DistrictRef = @districtId
	end
	if @schoolId is not null
	begin
		Update 
			School 
		Set
			IsMessagingDisabled = @disabled
		where 
			Id = @schoolId
	end
Commit Transaction