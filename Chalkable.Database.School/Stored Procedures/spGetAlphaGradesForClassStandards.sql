Create Procedure spGetAlphaGradesForClassStandards @classIds TInt32 readonly
as

Select 
	AlphaGrade.*,
	cId.value as ClassId
From
	AlphaGrade
join 
	GradingScaleRange on GradingScaleRange.AlphaGradeRef = AlphaGrade.Id
join 
	ClassroomOption on ClassroomOption.StandardsGradingScaleRef = GradingScaleRange.GradingScaleRef
join 
	@classIds cId on cId.value = ClassroomOption.id