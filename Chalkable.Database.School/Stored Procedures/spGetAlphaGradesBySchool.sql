Create Procedure spGetAlphaGradesBySchool @schoolId int
as 
Select 
	*
From
	AlphaGrade
Where
	Schoolref = @schoolId
	and exists(select * from GradingScaleRange where GradingScaleRange.AlphaGradeRef=AlphaGrade.Id)
