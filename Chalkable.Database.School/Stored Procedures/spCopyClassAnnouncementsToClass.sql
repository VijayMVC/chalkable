Create Procedure [dbo].[spCopyClassAnnouncementsToClass]
	@sisCopyResult TSisActivityCopyResult Readonly,
	@toClassId int,
	@created datetime2
As

Declare @schoolYearId int;
Set @schoolYearId = (Select SchoolYearRef From Class Where Id = @toClassId)

Declare @newAnnIds table 
(
	[FromAnnouncementId] int,
	[FromActivityId] int,
	[ToAnnouncementId] int,
	[ToActivityId] int
)

Declare @toCopy table 
(
	[Id] int, 
	[FromActivityId] int, 
	[ToActivityId] int
);

Insert Into @toCopy
	Select [Id], [SisActivityId], [ToActivityId]
	From ClassAnnouncement join @sisCopyResult as sisCopyRes
		on ClassAnnouncement.[SisActivityId] = sisCopyRes.[FromActivityId]


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
	Insert (Content, Created, [State], Title)
	Values (null, @created, 1, null)
Output ToCopy.[Id], ToCopy.[FromActivityId], Inserted.[Id], ToCopy.[ToActivityId]
	Into @newAnnIds;

Insert Into ClassAnnouncement
(Id, Expires, ClassRef, MayBeDropped, VisibleForStudent, 
	[Order], Dropped, SisActivityId, SchoolYearRef, IsScored)
	Select 
		[ToAnnouncementId], getDate(), @toClassId,
		0, 0, 0, 0, 
		[ToActivityId], --SisActivityId
		@schoolYearId,  --SchoolYearRef
		0
	From
		@newAnnIds

Select 
	[FromAnnouncementId],
	[ToAnnouncementId]
From
	@newAnnIds
GO


