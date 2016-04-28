CREATE PROCEDURE [dbo].[spGetAlphaGradesForSchoolStandards]
	@schoolIds TInt32 ReadOnly
AS

Select 
	AlphaGrade.*,
	sIds.value as SchoolId
From
	AlphaGrade
join 
	GradingScaleRange on GradingScaleRange.AlphaGradeRef = AlphaGrade.Id
join 
	SchoolOption on SchoolOption.StandardsGradingScaleRef = GradingScaleRange.GradingScaleRef
join 
	@schoolIds sIds on sIds.value = SchoolOption.Id
