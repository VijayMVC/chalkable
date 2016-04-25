Create Procedure spGetAlphaGradesForClasses @classIds TInt32 readonly
as
Select
	AlphaGrade.*,
	cid.value as ClassId
From
	AlphaGrade
Join 
	GradingScaleRange on GradingScaleRange.AlphaGradeRef = AlphaGrade.Id
Join
	Class on GradingScaleRange.GradingScaleRef = Class.GradingScaleRef
Join 
	@classIds cid on Class.Id = cid.value
order by
	GradingScaleRange.HighValue desc