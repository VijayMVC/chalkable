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
	From LessonPlan Where [Id] in(Select * From @ids)

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
Declare @schoolYearStartDate datetime2;
Set		@schoolYearEndDate = (Select Max([day]) From @classDays)
Set		@schoolYearStartDate = (Select Min([day]) From @classDays)

--Lps where EndDate is out of School Year end date
Declare		@LPsOutOfSchoolYearEndDate TInt32;
Insert Into @LPsOutOfSchoolYearEndDate
	Select Id From @toAdjust Where [EndDate] is null or EndDate > @schoolYearEndDate

--Fixing End and Start date for announcement 
--if it is out of school year end date
Update @toAdjust
Set EndDate = @schoolYearEndDate
Where Id in (Select * From @LPsOutOfSchoolYearEndDate)

Update @toAdjust
Set StartDate =  IsNull((Select Min(x.[day]) From (
							Select top(IIF(TotalSchoolDays = 0, 1, TotalSchoolDays)) [day] 
							From @classDays cd 
							Where cd.[day] <= EndDate 
							order by [day] desc
						 ) x
						), EndDate)
Where Id in(Select * From @LPsOutOfSchoolYearEndDate)

--Lps where StartDate is out of School Year start date
Declare		@LPsOutOfSchoolYearStartDate TInt32;
Insert Into @LPsOutOfSchoolYearStartDate
	Select Id From @toAdjust Where StartDate < @schoolYearStartDate

--Fixing End and Start date for announcement 
--if it is out of school year start date
Update @toAdjust
Set StartDate =  @schoolYearStartDate
Where Id in(Select * From @LPsOutOfSchoolYearStartDate)

Update @toAdjust
Set EndDate = IsNull((Select Max(x.[day]) From (
							Select top(IIF(TotalSchoolDays = 0, 1, TotalSchoolDays)) [day] 
							From @classDays cd 
							Where cd.[day] >= StartDate
							order by [day] asc
						 ) x
						), EndDate)
Where Id in (Select * From @LPsOutOfSchoolYearStartDate)

-------------------------

Update LessonPlan
Set LessonPlan.StartDate = t.StartDate,
	LessonPlan.EndDate   = t.EndDate
From  @toAdjust t
Where LessonPlan.Id = t.Id


GO


