CREATE PROCEDURE [dbo].[spAdjustLessonPlanDates]
	@ids	 TInt32 ReadOnly,
	@shift	 int,
	@classId int
As

Declare @classDays TDate;
Insert Into @classDays
	exec spGetClassDays @classId

Declare @toAdjust table (
	[Id]			  int,
	[StartDate]		  datetime2,
	[EndDate]		  datetime2,
	[TotalSchoolDays] int		--Total school days in date range
);

Insert Into @toAdjust
	Select [Id], [StartDate], [EndDate], (
			Select Count(*)-1 From @classDays					   
			Where [Day] Between LessonPlan.StartDate and LessonPlan.EndDate 
		) As [TotalSchoolDays]
	From LessonPlan Join @ids as lpIds On LessonPlan.Id = lpIds.Value

---------------------------------------------------------------------------------------
Update @toAdjust
Set [StartDate] = dbo.CalcAnnouncementDate(@classDays, [StartDate], @shift)

Update @toAdjust
Set [EndDate] = dbo.CalcAnnouncementDate(@classDays, [StartDate], TotalSchoolDays)
---------------------------------------------------------------------------------------

Update LessonPlan
Set LessonPlan.StartDate = t.StartDate,
	LessonPlan.EndDate   = t.EndDate
From  LessonPlan Join @toAdjust t
	On LessonPlan.Id = t.Id

GO