CREATE PROCEDURE spAdjustSupplementalAnnouncementDates
	@suppIds   TInt32 ReadOnly,
	@startDate datetime2,
	@classId   int
As

Declare @schoolYearId int;
Set		@schoolYearId = (Select SchoolYearRef From Class Where Id = @classId);

Declare @classDays table ( [Day] datetime2 );
Insert Into @classDays
	Select Distinct [Day] From [Date]
	Where SchoolYearRef = @schoolYearId 
		  And IsSchoolDay = 1 
		  And Exists(Select * From ClassPeriod Where ClassPeriod.DayTypeRef = [Date].DayTypeRef And ClassPeriod.ClassRef = @classId)
		  And [Day] >= @startDate
	Order By [Day]

Declare @toAdjust table
(
	[Id]		  int,
	[ExpiresDate] datetime2
);

Insert Into @toAdjust
	Select [Id], [Expires] From SupplementalAnnouncement 
	Where [Id] in(Select * From @suppIds)

Declare @minAnnDate datetime2;
Set		@minAnnDate = (Select Min([ExpiresDate]) From @toAdjust);

Update @toAdjust
Set [ExpiresDate] = DateAdd(d, DateDiff(d, @minAnnDate, [ExpiresDate]), @startDate)

--Getting last school day of School Year
Declare @schoolYearEndDate datetime2;
Set		@schoolYearEndDate = (Select Max([day]) From @classDays)

--supplemental announcement out of school year
Declare @suppAnnOutOfSchoolYear TInt32;
Insert Into @suppAnnOutOfSchoolYear
	Select [Id] From @toAdjust Where [ExpiresDate] > @schoolYearEndDate

--Fixing date for announcement
Update @toAdjust
Set [ExpiresDate] = @schoolYearEndDate
Where Id in (Select * From @suppAnnOutOfSchoolYear)

Update SupplementalAnnouncement
Set Expires = t.ExpiresDate
From @toAdjust t
Where SupplementalAnnouncement.Id in(select Id from @toAdjust)

GO