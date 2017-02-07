CREATE PROCEDURE [dbo].[spAdjustSupplementalAnnouncementDates]
	@ids     TInt32 ReadOnly,
	@shift   int,
	@classId int
As

--need to get all class days, to obtain right result
--when start date is out of begining of School Year
Declare @classDays TDate;
Insert Into @classDays
	exec spGetClassDays @classId


Update SupplementalAnnouncement
Set Expires = dbo.CalcAnnouncementDate(@classDays, Expires, @shift)
From SupplementalAnnouncement Join @ids caIds
	On SupplementalAnnouncement.Id = caIds.Value


GO