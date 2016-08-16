CREATE PROCEDURE [dbo].[spAdjustLessonPlanDates]
	@ids	   TInt32 ReadOnly,
	@startDate datetime2,
	@classId   int
As

Declare @schoolYearId int;
Set		@schoolYearId = (Select SchoolYearRef From Class Where Id = @classId);

Declare @classDays table ( [Day] datetime2 );
Insert Into @classDays
	Select Distinct [Day] From [Date]
	Where SchoolYearRef = @schoolYearId and IsSchoolDay = 1 
		  And Exists(Select * From ClassPeriod Where ClassPeriod.DayTypeRef = [Date].DayTypeRef And ClassPeriod.ClassRef = @classId)
		  And [Day] >= @startDate
	Order by [Day]

Declare @toAdjust table
(
	[Id]			  int,
	[StartDate]		  datetime2,
	[EndDate]		  datetime2,
	[TotalSchoolDays] int		--Total school days in date range
);

Insert Into @toAdjust
	Select [Id], [StartDate], [EndDate],(
		  Select Count(*) 
		  From [Date]					   
		  Where [Day] Between LessonPlan.StartDate and LessonPlan.EndDate 
			    and exists(
							Select *
							From ClassPeriod 
							Where ClassRef = @classId and ClassPeriod.DayTypeRef  = [Date].DayTypeRef
						  )
				and IsSchoolDay = 1
			    and [Date].SchoolYearRef = @schoolYearId
		) As [TotalSchoolDays]
	From LessonPlan Where [Id] in(Select * From @lpIds)

Declare @minAnnDate datetime2;
Set		@minAnnDate = (Select Min([StartDate]) From @toAdjust);

Update @toAdjust
Set StartDate = DateAdd(d, DateDiff(d, @minAnnDate, StartDate), @startDate)

Update lp
Set lp.EndDate = IsNull((Select Max(x.[Day]) From (
								Select top(IIF(lp.TotalSchoolDays = 0, 1, lp.TotalSchoolDays)) cd.[Day] as [Day] 
								From @classDays cd 
								Where cd.[Day] >= lp.StartDate
								Order By [Day] Asc
						 ) x), lp.StartDate)
From @toAdjust lp

--Getting last school day of School Year
Declare @schoolYearEndDate datetime2;
Set		@schoolYearEndDate = (Select Max([day]) From @classDays)

--Lps where EndDate is out of School Year date range
Declare		@LPsOutOfSchoolYear TInt32;
Insert Into @LPsOutOfSchoolYear
	Select Id From @toAdjust Where EndDate > @schoolYearEndDate

--Fixing End and Start date for announcement
Update @toAdjust
Set EndDate = @schoolYearEndDate
Where Id in (Select * From @LPsOutOfSchoolYear)

Update @toAdjust
Set StartDate =  IsNull((Select Min(x.[day]) From (
							Select top(IIF(TotalSchoolDays = 0, 1, TotalSchoolDays)) [day] 
							From @classDays cd 
							Where cd.[day] <= EndDate 
							order by [day] desc
						 ) x
						), EndDate)
Where Id in(Select * From @LPsOutOfSchoolYear)

Update LessonPlan
Set LessonPlan.StartDate = t.StartDate,
	LessonPlan.EndDate   = t.EndDate
From  @toAdjust t
Where LessonPlan.Id in(Select [Id] From @toAdjust)

GO