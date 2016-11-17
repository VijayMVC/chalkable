CREATE PROCEDURE [dbo].[spAdjustClassAnnouncementDates_NEW]
	@ids     TInt32 ReadOnly,
	@shift   int,
	@classId int
As

Declare @schoolYearId int;
Set	@schoolYearId = (Select SchoolYearRef From Class Where Id = @classId);

--need to get all class days, to obtain right result
--when start date is out of begining of School Year
Declare @classDays TDate;
Insert Into @classDays
	exec spGetClassDays @classId

Begin Transaction

	Update ClassAnnouncement
	Set Expires = dbo.CalcAnnouncementDate(@classDays, Expires, @shift)
	From ClassAnnouncement Join @ids caIds
		On ClassAnnouncement.Id = caIds.Value

	select SisActivityId, Expires
	From ClassAnnouncement Join @ids caIds
		On ClassAnnouncement.Id = caIds.Value

Commit

GO