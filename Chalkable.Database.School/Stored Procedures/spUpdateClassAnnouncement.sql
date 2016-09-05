CREATE Procedure [dbo].[spUpdateClassAnnouncement]
@classAnnouncements TInputClassAnnouncement readonly
as
update 
	[Announcement]
set
	[Content] = [CAs].[Content],
	[Created] = [CAs].[Created],
	[State] = [CAs].[State],
	[Title] = [CAs].[Title],
	[DiscussionEnabled] = [CAs].[DiscussionEnabled],
	[PreviewCommentsEnabled] = [CAs].PreviewCommentsEnabled,
	[RequireCommentsEnabled] = [CAs].RequireCommentsEnabled
	
from
	 @classAnnouncements [CAs]
where 
	[Announcement].[Id]= [CAs].[Id]

update 
	[ClassAnnouncement] 
set
	[Expires] = [CAs].[Expires],
	[ClassAnnouncementTypeRef] = [CAs].[ClassAnnouncementTypeRef],
	[ClassRef] = [CAs].[ClassRef],
	[SchoolYearRef] = [CAs].[SchoolYearRef],
	[Dropped] = [CAs].[Dropped],
	[SisActivityId] = [CAs].[SisActivityId],
	[MaxScore] = [CAs].[MaxScore],
	[WeightAddition] = [CAs].[WeightAddition],
	[WeightMultiplier] = [CAs].[WeightMultiplier],
	[VisibleForStudent] = [CAs].[VisibleForStudent],
	[MayBeDropped] = [CAs].[MayBeDropped],
	[IsScored] = [CAs].[IsScored]
from
	 @classAnnouncements [CAs]
where 
	[ClassAnnouncement].[Id]= [CAs].[Id] 


GO