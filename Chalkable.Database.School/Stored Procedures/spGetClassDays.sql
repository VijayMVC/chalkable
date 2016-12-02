CREATE Procedure [dbo].[spGetClassDays]
	@classId int
As

Declare @schoolYearId int = (Select SchoolYearRef From Class Where Id = @classId)
Declare @markingPeriodIds TInt32;

insert into @markingPeriodIds 
	Select MarkingPeriodRef 
	From MarkingPeriodClass Join MarkingPeriod
		On MarkingPeriodClass.MarkingPeriodRef = MarkingPeriod.Id
	Where ClassRef = @classId And SchoolYearRef = @schoolYearId
	Order By MarkingPeriod.EndDate Desc


Select Distinct * From [Date]
Where SchoolYearRef = @schoolYearId and IsSchoolDay = 1 
	  And Exists(Select * From ClassPeriod 
				 Where ClassPeriod.DayTypeRef = [Date].DayTypeRef And ClassPeriod.ClassRef = @classId)
	  And Exists(
				 Select * From MarkingPeriod
				 Where  Id in (Select * From @markingPeriodIds)
					   And [Date].[Day] Between StartDate And EndDate
				)
Order by [Day]

GO