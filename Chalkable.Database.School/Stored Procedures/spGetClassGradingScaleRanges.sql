CREATE PROCEDURE spGetClassGradingScaleRanges
	@classId int
As

Select 
	GradingScaleRange.*
From
	Class join GradingScaleRange
		On Class.GradingScaleRef = GradingScaleRange.GradingScaleRef
Where
	Class.Id = @classId
Order By LowValue