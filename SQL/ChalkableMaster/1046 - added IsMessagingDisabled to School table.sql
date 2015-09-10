Alter Table [School]
Add IsMessagingDisabled bit null
Go

Update [School]
set IsMessagingDisabled = 0
Go

Alter Table [School]
Alter Column IsMessagingDisabled bit not null
Go


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
GO


