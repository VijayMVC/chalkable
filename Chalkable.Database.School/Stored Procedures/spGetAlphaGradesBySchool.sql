Create Procedure spGetAlphaGradesBySchool @schoolId int
as 
Select 
	*
From
	AlphaGrade
Where
	SchoolRef = @schoolId
	and exists(select * from GradingScaleRange where GradingScaleRange.AlphaGradeRef=AlphaGrade.Id)
