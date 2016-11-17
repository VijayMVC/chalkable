CREATE PROCEDURE [dbo].[spAdjustSupplementalAnnouncementDates]
	@ids   TInt32 ReadOnly,
	@startDate datetime2,
	@classId   int
As

Declare @schoolYearId int;
Set		@schoolYearId = (Select SchoolYearRef From Class Where Id = @classId);

--need to get all class days, to obtain right result
--when start date is out of begining of School Year
Declare @classDays TDate;
Insert Into @classDays
	exec spGetClassDays @classId

Declare @toAdjust table
(
	[Id]		  int,
	[ExpiresDate] datetime2
);

Insert Into @toAdjust
	Select [Id], [Expires] From SupplementalAnnouncement 
	Where [Id] in(Select * From @ids)

Declare @minAnnDate datetime2;
Set		@minAnnDate = (Select Min([ExpiresDate]) From @toAdjust);

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
Declare @suppAnnOutOfSchoolYearEndDate TInt32;
Insert Into @suppAnnOutOfSchoolYearEndDate
	Select [Id] From @toAdjust Where [ExpiresDate] is null or [ExpiresDate] > @schoolYearEndDate

Declare @suppAnnOutOfSchoolYearStartDate TInt32;
Insert Into @suppAnnOutOfSchoolYearStartDate
	Select [Id] From @toAdjust Where [ExpiresDate] < @schoolYearStartDate

--Fixing date for announcement
Update @toAdjust
Set [ExpiresDate] = @schoolYearEndDate
Where Id in (Select * From @suppAnnOutOfSchoolYearEndDate)

Update @toAdjust
Set [ExpiresDate] = @schoolYearStartDate
Where Id in (Select * From @suppAnnOutOfSchoolYearStartDate)

------------------------------------------------------------------
Update SupplementalAnnouncement
Set Expires = t.ExpiresDate
From @toAdjust t
Where SupplementalAnnouncement.Id = t.Id


GO


