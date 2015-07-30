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
Begin Transaction 
as
	if @districtId is not null
	begin
		Update 
			School 
		Set
			IsMessaginDisabled = @disabled
		where 
			DistrictRef = @districtId
	end
	if @schoolId is not null
	begin
		Update 
			School 
		Set
			IsMessaginDisabled = @disabled
		where 
			Id = @schoolId
	end
Commit Transaction
GO


