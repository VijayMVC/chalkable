CREATE Procedure [dbo].[spGetClassDays]
	@classId int
As

Declare @schoolYearId int;
Set		@schoolYearId = (Select SchoolYearRef From Class Where Id = @classId);
Declare @markingPeriodId TInt32;

insert into @markingPeriodId  
	Select MarkingPeriodRef 
	From MarkingPeriodClass Join MarkingPeriod
		On MarkingPeriodClass.MarkingPeriodRef = MarkingPeriod.Id
	Where ClassRef = @classId Order By MarkingPeriod.EndDate Desc


Declare @markingPeriodStartDate datetime2 = (Select Top 1 MP.StartDate From MarkingPeriod MP Where Id in (select * from @markingPeriodId) order by MP.StartDate)
Declare @markingPeriodEndDate datetime2 = (Select Top 1 MP.EndDate From MarkingPeriod MP Where Id in (select * from @markingPeriodId) order by MP.EndDate DESC)

Select Distinct * From [Date]
Where SchoolYearRef = @schoolYearId and IsSchoolDay = 1 
	  And Exists(Select * From ClassPeriod 
				 Where ClassPeriod.DayTypeRef = [Date].DayTypeRef And ClassPeriod.ClassRef = @classId)
	  And [Day] Between @markingPeriodStartDate And @markingPeriodEndDate
Order by [Day]

GO