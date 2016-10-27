CREATE PROCEDURE [dbo].[spAdjustClassAnnouncementDates]
	@ids   TInt32 ReadOnly,
	@startDate datetime2,
	@classId   int
As

Declare @schoolYearId int;
Set		@schoolYearId = (Select SchoolYearRef From Class Where Id = @classId);

--need to get all class days, to obtain right result
--when start date is out of begining of School Year
Declare @classDays table ( [Day] datetime2 );
Insert Into @classDays
	Select Distinct [Day] From [Date]
	Where SchoolYearRef = @schoolYearId 
		  And IsSchoolDay = 1 
		  And Exists(Select * From ClassPeriod Where ClassPeriod.DayTypeRef = [Date].DayTypeRef And ClassPeriod.ClassRef = @classId)
		  --And [Day] >= @startDate
	Order By [Day]

Declare @toAdjust table
(
	[Id]		  int,
	[SisActivityId] int, 
	[ExpiresDate] datetime2
);

Insert Into @toAdjust
	Select [Id], [SisActivityId], [Expires] From ClassAnnouncement 
	Where [Id] in(Select * From @ids)

Declare @minAnnDate datetime2;
Set		@minAnnDate = (Select Min([ExpiresDate]) From @toAdjust);

print @minAnnDate

--to save distance beween supp announcements
Update @toAdjust
Set [ExpiresDate] = DateAdd(d, DateDiff(d, @minAnnDate, [ExpiresDate]), @startDate)

--move to nearest class day
Update @toAdjust
Set [ExpiresDate] =(Select Min([Day]) From @classDays Where [Day] >= [ExpiresDate])

--Getting last school day of School Year
Declare @schoolYearEndDate datetime2;
Declare @schoolYearStartDate datetime2;
Set		@schoolYearEndDate   = (Select Max([day]) From @classDays)
Set		@schoolYearStartDate = (Select Min([Day]) From @classDays)

--supplemental announcement out of school year
Declare @annOutOfSchoolYearEndDate TInt32;
Insert Into @annOutOfSchoolYearEndDate
	Select [Id] From @toAdjust Where [ExpiresDate] is null or [ExpiresDate] > @schoolYearEndDate

Declare @annOutOfSchoolYearStartDate TInt32;
Insert Into @annOutOfSchoolYearStartDate
	Select [Id] From @toAdjust Where [ExpiresDate] < @schoolYearStartDate

--Fixing date for announcement
Update @toAdjust
Set [ExpiresDate] = @schoolYearEndDate
Where Id in (Select * From @annOutOfSchoolYearEndDate)

Update @toAdjust
Set [ExpiresDate] = @schoolYearStartDate
Where Id in (Select * From @annOutOfSchoolYearStartDate)

------------------------------------------------------------------
Update ClassAnnouncement
Set Expires = t.ExpiresDate
From @toAdjust t
Where ClassAnnouncement.Id = t.Id

select SisActivityId, ExpiresDate from @toAdjust


GO