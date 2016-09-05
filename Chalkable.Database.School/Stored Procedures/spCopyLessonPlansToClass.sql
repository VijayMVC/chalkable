CREATE PROCEDURE [dbo].[spCopyLessonPlansToClass]
	@created datetime2,
	@lessonPlanIds TInt32 Readonly,
	@toClassId int,
	@startDate datetime2
As

Declare @toSchoolYearId int = (Select Top 1 SchoolYearRef From Class Where Id = @toClassId),
		@minAnnDate datetime2;

Declare @classDays table([day] datetime2)

Insert Into @classDays
Select distinct [Day] 
From [Date]
Where SchoolYearRef = @toSchoolYearId and IsSchoolDay = 1 
	  And exists(Select * From ClassPeriod Where ClassPeriod.DayTypeRef = [Date].DayTypeRef And ClassPeriod.ClassRef = @toClassId)
	  And [Day] >= @startDate
Order by [Day]
---------------------Creating little of LPs to be copied--------------------

If ((Select count(*) From @classDays) = 0)
Begin
	Insert Into @classDays
	values (@startDate)  
End

Declare @toCopy table 
(
	Id  int,
	Content nvarchar(512),
	StartDate datetime2,
	EndDate datetime2,
	VisibleForStudent bit,
	Title nvarchar(200),
	SchoolYearRef int,
	InGallery bit,
	GalleryOwnerRef int,
	DiscussionEnabled bit,
	PreviewCommentsEnabled bit,
	RequireCommentsEnabled bit,
	TotalSchoolDays int
)

Insert Into @toCopy
	Select 
		Id, Content, StartDate, EndDate, VisibleForStudent, Title, SchoolYearRef, InGallery, GalleryOwnerRef,
		DiscussionEnabled, PreviewCommentsEnabled, RequireCommentsEnabled,
		(
		  Select Count(*) 
		  From [Date]					   
		  Where [Day] Between vwLessonPlan.StartDate and vwLessonPlan.EndDate 
			    and exists(
							Select *
							From ClassPeriod 
							Where ClassRef = vwLessonPlan.ClassRef and ClassPeriod.DayTypeRef  = [Date].DayTypeRef
						  )
				and IsSchoolDay = 1
			    and [Date].SchoolYearRef = @toSchoolYearId
		) As TotalSchoolDays 
	From 
		vwLessonPlan 
	Where 
		Id in(Select * From @lessonPlanIds)

Set @minAnnDate = (Select Min(StartDate) From @toCopy)

------------------------Preparing data range for new LPs--------------------------

Update @toCopy
Set StartDate = DateAdd(d, DateDiff(d, @minAnnDate, StartDate), @startDate)

Update lp
Set lp.EndDate = IsNull((Select Max(x.[day]) 
					    From (
								Select top(IIF(lp.TotalSchoolDays = 0, 1, lp.TotalSchoolDays)) cd.[day] as [day] 
								From @classDays cd 
								Where cd.[day] >= lp.StartDate
								Order by [Day] Asc
						     ) x
					 ), lp.StartDate)
From @toCopy lp

--Getting last school day of School Year
Declare @schoolYearEndDate datetime2;
Set @schoolYearEndDate = (Select Max([day]) From @classDays)

--Lps where EndDate is out of School Year date range
Declare @newLPsOutOfSchoolYear TInt32;
Insert Into @newLPsOutOfSchoolYear
	Select Id From @toCopy Where EndDate > @schoolYearEndDate

--Fixing End and Start date for announcement
Update @toCopy
Set EndDate = @schoolYearEndDate
Where Id in (Select * From @newLPsOutOfSchoolYear)

Update @toCopy
Set StartDate =  IsNull((Select Min(x.[day]) 
						From (
								Select top(IIF(TotalSchoolDays = 0, 1, TotalSchoolDays)) [day] 
								From @classDays cd 
								Where cd.[day] <= EndDate 
								order by [day] desc
							  ) x
						), EndDate)

Where Id in(Select * From @newLPsOutOfSchoolYear)

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
	Insert (Content, Created, [State], Title, DiscussionEnabled, PreviewCommentsEnabled, RequireCommentsEnabled)
	Values (ToCopy.Content, @created, 1, ToCopy.Title, ToCopy.DiscussionEnabled, ToCopy.PreviewCommentsEnabled, ToCopy.RequireCommentsEnabled)
Output Inserted.Id, ToCopy.Id
	Into @newAnnIds;

Insert Into LessonPlan
	Select 
		NewLp.[ToAnnouncementId],
		ToCopy.StartDate,
		ToCopy.EndDate,
		@toClassId,
		null,
		null,
		ToCopy.VisibleForStudent,
		@toSchoolYearId,
		ToCopy.InGallery,
		ToCopy.GalleryOwnerRef
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
GO
