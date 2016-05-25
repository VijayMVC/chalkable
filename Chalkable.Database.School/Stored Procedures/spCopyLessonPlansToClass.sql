CREATE PROCEDURE spCopyLessonPlansToClass
	@created datetime2,
	@lessonPlanIds TInt32 Readonly,
	@toClassId int,
	@startDate datetime2
As

Begin transaction

Declare @toSchoolYearId int = (Select Top 1 SchoolYearRef From Class Where Id = @toClassId),
		@minAnnDate datetime2;

---------------------Creating little of LPs to be copied--------------------

Declare @toCopy table 
(
	Id  int,
	Content nvarchar(512),
	StartDate datetime2,
	EndDate datetime2,
	VisibleForStudent bit,
	Title nvarchar(200),
	SchoolYearRef int,
	TotalSchoolDays int
)

Insert Into @toCopy
	Select 
		Id, Content, StartDate, EndDate, VisibleForStudent, Title, SchoolYearRef,
		(Select Sum(Cast(IsSchoolDay As int)) From [Date]					   --|Need for correct
		 Where [Day] Between vwLessonPlan.StartDate and vwLessonPlan.EndDate   --|data range in new
			   and [Date].SchoolYearRef = @toSchoolYearId) As TotalSchoolDays  --|LPs
	From 
		vwLessonPlan 
	Where 
		Id in(Select * From @lessonPlanIds)

Set @minAnnDate = (Select Min(StartDate) From @toCopy)

------------------------Preparing data range for new LPs--------------------------

Update @toCopy
Set StartDate = DateAdd(d, DateDiff(d, @minAnnDate, StartDate), @startDate)

Update @toCopy
Set EndDate = IsNull((Select Top(TotalSchoolDays) Max([Day]) From [Date] 
					  Where SchoolYearRef = @toSchoolYearId And [Day] >= StartDate And IsSchoolDay = 1
					  Group By [Day] Order By [Day] ), StartDate)

--------------Needed because LessonPlan.Id is FK on Announcement.Id----------------

declare @newAnnIds table
( 
	[ToAnnouncementId] int, 
	[FromAnnouncementId] int
);

Merge Into Announcement
Using @toCopy as ToCopy
	On 1 = 0
When Not Matched Then
	Insert (Content, Created, [State], Title)
	Values (ToCopy.Content, @created, 1, ToCopy.Title)
Output Inserted.Id, ToCopy.Id
	Into @newAnnIds;

Insert Into LessonPlan
	Select 
		NewLp.[ToAnnouncementId],
		ToCopy.StartDate,
		ToCopy.EndDate,
		@toClassId,
		null,
		ToCopy.VisibleForStudent,
		@toSchoolYearId
	From 
		@toCopy as ToCopy join @newAnnIds as NewLp
			on ToCopy.Id = NewLp.[FromAnnouncementId]

--------------------------Adding AnnouncementStandards------------------------------

Insert Into AnnouncementStandard
	Select StandardRef, [ToAnnouncementId]
	From @newAnnIds join AnnouncementStandard
		on [FromAnnouncementId] = AnnouncementStandard.AnnouncementRef

--Returning copied LPs. Source ids and new ids.
Select
	[FromAnnouncementId],
	[ToAnnouncementId]
From @newAnnIds

Commit
