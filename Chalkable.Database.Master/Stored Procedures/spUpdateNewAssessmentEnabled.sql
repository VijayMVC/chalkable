CREATE Procedure [dbo].[spUpdateNewAssessmentEnabled]
	@districtId uniqueidentifier,
	@schoolId uniqueidentifier,
	@enabled bit
as
	if @districtId is not null
	begin
		Update 
			School 
		Set
			IsNewAssessmentEnabled = @enabled
		where 
			DistrictRef = @districtId
	end
	if @schoolId is not null
	begin
		Update 
			School 
		Set
			IsNewAssessmentEnabled = @enabled
		where 
			Id = @schoolId
	end
GO