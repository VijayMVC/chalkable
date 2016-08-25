Create Procedure [dbo].[spCopyClassAnnouncementsToClass]
	@sisCopyResult TSisActivityCopyResult Readonly,
	@created datetime2
As

--Declare @syClassIds table (ClassId int, SchoolYearId int)

--Insert Into @syClassIds
--Select Id, SchoolYearRef 
--From Class Where  Id in (Select ToClassId From @sisCopyResult)


--Declare @schoolYearId int;
--Set @schoolYearId = (Select SchoolYearRef From Class Where Id = @toClassId)

Declare @newAnnIds table 
(
	[FromAnnouncementId] int,
	[FromActivityId] int,
	[ToAnnouncementId] int,
	[ToActivityId] int,
	[ToClassId] int,
	[ToSchoolYearId] int
)

Declare @toCopy table 
(
	[Id] int, 
	[FromActivityId] int, 
	[ToActivityId] int,
	DiscussionEnabled bit,
	PreviewCommentsEnabled bit,
	RequireCommentsEnabled bit,
	[ToClassId] int,
	[ToSchoolYearId] int
);

Insert Into @toCopy
	Select vwClassAnnouncement.[Id], 
		   [SisActivityId], 
		   [ToActivityId],  
		   DiscussionEnabled, 
		   PreviewCommentsEnabled, 
		   RequireCommentsEnabled,
		   Class.Id,
		   Class.SchoolYearRef
	From vwClassAnnouncement 
	Join  @sisCopyResult as sisCopyRes
		on vwClassAnnouncement.[SisActivityId] = sisCopyRes.[FromActivityId]
	Join Class on Class.Id = sisCopyRes.ToClassId

Delete From @toCopy
Where [ToActivityId] in (Select SisActivityId From ClassAnnouncement)

----Need to filter this
--Insert Into @toCopy

--	Select * from @toCopy
--	Where [ToActivityId] not in(Select SisActivityId From ClassAnnouncement)

Merge Into Announcement
Using @toCopy as ToCopy
	On 1 = 0
When Not Matched Then
	Insert (Content, Created, [State], Title, DiscussionEnabled, PreviewCommentsEnabled, RequireCommentsEnabled)
	Values (null, @created, 1, null, ToCopy.DiscussionEnabled, ToCopy.PreviewCommentsEnabled, ToCopy.RequireCommentsEnabled)
Output ToCopy.[Id], ToCopy.[FromActivityId], Inserted.[Id], ToCopy.[ToActivityId], ToCopy.ToClassId, ToCopy.ToSchoolYearId
	Into @newAnnIds;

Insert Into ClassAnnouncement
(Id, Expires, ClassRef, MayBeDropped, VisibleForStudent, 
	Dropped, SisActivityId, SchoolYearRef, IsScored)
	Select 
		[ToAnnouncementId], getDate(), ToClassId,
		0, 0, 0,
		[ToActivityId], --SisActivityId
		ToSchoolYearId,  --SchoolYearRef
		0
	From
		@newAnnIds

Select 
	[FromAnnouncementId],
	[ToAnnouncementId]
From
	@newAnnIds
GO


