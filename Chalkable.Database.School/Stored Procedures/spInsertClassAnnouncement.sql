CREATE Procedure [dbo].[spInsertClassAnnouncement]
@classAnnouncements TInputClassAnnouncement readonly

as

declare
	@classAnnouncementsFiltered TInputClassAnnouncement

insert into @classAnnouncementsFiltered
select * from @classAnnouncements

delete from @classAnnouncementsFiltered
where SisActivityId in (select CA.SisActivityId from ClassAnnouncement CA)

declare @pointedClassAnnouncement table
(
	[Pointer] int NOT NULL identity(1,1),
	[Created] [datetime2](7) NOT NULL,
	[State] [int] NOT NULL,
	[Content] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
	[DiscussionEnabled]			BIT	 NOT NULL,
	[PreviewCommentsEnabled]	BIT	 NOT NULL,
	[RequireCommentsEnabled]	BIT	 NOT NULL,
	[Expires] [datetime2](7) NOT NULL,
	[Order] [int] NOT NULL,
	[Dropped] [bit] NOT NULL,
	[ClassAnnouncementTypeRef] [int] NULL,
	[SchoolYearRef] [int] NULL,
	[SisActivityId] [int] NULL,
	[MaxScore] [decimal](18, 0) NULL,
	[WeightAddition] [decimal](9, 6) NULL,
	[WeightMultiplier] [decimal](9, 6) NULL,
	[MayBeDropped] [bit] NULL,
	[VisibleForStudent] [bit] NULL,
	[ClassRef] [int] NOT NULL,
	[IsScored] [bit] NOT NULL
)

declare @pointedAnnouncementIds table 
(
	[Pointer] int NOT NULL identity(1,1),
	[AnnouncementId] [int] NOT NULL
)

insert into @pointedClassAnnouncement
(
	[Created],
	[State],
	[Content],
	[Title],
	[Expires],
	[Order],
	[Dropped],
	[ClassAnnouncementTypeRef],
	[SchoolYearRef],
	[SisActivityId],
	[MaxScore],
	[WeightAddition],
	[WeightMultiplier],
	[MayBeDropped],
	[VisibleForStudent],
	[ClassRef],
	[IsScored]
)
select 
	[Created],
	[State],
	[Content],
	[Title],
	[Expires],
	[Order],
	[Dropped],
	[ClassAnnouncementTypeRef],
	[SchoolYearRef],
	[SisActivityId],
	[MaxScore],
	[WeightAddition],
	[WeightMultiplier],
	[MayBeDropped],
	[VisibleForStudent],
	[ClassRef],
	[IsScored]
from 
	@classAnnouncementsFiltered

Insert into Announcement 
(
	[Content], 
	[Created], 
	[State], 
	[Title]
)
OUTPUT Inserted.Id into @pointedAnnouncementIds
select 
	[Content], 
	[Created], 
	[State], 
	[Title] 
from 
	@pointedClassAnnouncement 
order by 
	[Pointer] ASC

insert into ClassAnnouncement 
(
	[Id],
	[Expires],
	[ClassRef],
	[ClassAnnouncementTypeRef],
	[MayBeDropped],
	[VisibleForStudent],
	[Order],
	[Dropped],
	[MaxScore],
	[WeightAddition],
	[WeightMultiplier],
	[SisActivityId],
	[SchoolYearRef],
	[IsScored]
)
select 
	[AnnouncementId],
	[Expires],
	[ClassRef],
	[ClassAnnouncementTypeRef],
	[MayBeDropped],
	[VisibleForStudent],
	[Order],
	[Dropped],
	[MaxScore],
	[WeightAddition],
	[WeightMultiplier],
	[SisActivityId],
	[SchoolYearRef],
	[IsScored]
from 
	@pointedClassAnnouncement PCA
	join @pointedAnnouncementIds PAI 
		on PCA.Pointer = PAI.Pointer


GO