CREATE Procedure spGetClassDays
	@classId int
As

Declare @schoolYearId int;
Set		@schoolYearId = (Select SchoolYearRef From Class Where Id = @classId);

Select Distinct * From [Date]
Where SchoolYearRef = @schoolYearId and IsSchoolDay = 1 
	  And Exists(Select * From ClassPeriod Where ClassPeriod.DayTypeRef = [Date].DayTypeRef And ClassPeriod.ClassRef = @classId)
Order by [Day]

Go