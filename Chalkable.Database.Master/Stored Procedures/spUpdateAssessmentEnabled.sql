CREATE Procedure [dbo].[spUpdateAssessmentEnabled]
	@districtId uniqueidentifier,
	@schoolId uniqueidentifier,
	@enabled bit
as
	if @districtId is not null
	begin
		Update 
			School 
		Set
			IsAssessmentEnabled = @enabled
		where 
			DistrictRef = @districtId
	end
	if @schoolId is not null
	begin
		Update 
			School 
		Set
			IsAssessmentEnabled = @enabled
		where 
			Id = @schoolId
	end
GO
