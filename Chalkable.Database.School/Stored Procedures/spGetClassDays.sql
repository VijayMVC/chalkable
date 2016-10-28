CREATE Procedure [dbo].[spGetClassDays]
	@classId int
As

Declare @schoolYearId int;
Set		@schoolYearId = (Select SchoolYearRef From Class Where Id = @classId);
Declare @markingPeriodId int = (Select MarkingPeriodRef From MarkingPeriodClass Where ClassRef = @classId)
Declare @markingPeriodStartDate datetime2 = (Select StartDate From MarkingPeriod Where Id = @markingPeriodId)
Declare @markingPeriodEndDate datetime2 = (Select EndDate From MarkingPeriod Where Id = @markingPeriodId)

Select Distinct * From [Date]
Where SchoolYearRef = @schoolYearId and IsSchoolDay = 1 
	  And Exists(Select * From ClassPeriod 
				 Where ClassPeriod.DayTypeRef = [Date].DayTypeRef And ClassPeriod.ClassRef = @classId)
	  And [Day] Between @markingPeriodStartDate And @markingPeriodEndDate
Order by [Day]

GO